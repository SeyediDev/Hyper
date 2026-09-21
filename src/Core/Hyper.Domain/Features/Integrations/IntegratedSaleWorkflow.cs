namespace Hyper.Domain.Features.Integrations;

public enum SaleChannel : byte { Booth = 1, Store = 2 }
public enum SalePaymentStatus : byte { Pending = 0, Paid = 1, PartiallyPaid = 2, Credit = 3, Failed = 4 }
public enum SaleWorkflowStatus : byte { Draft = 0, AwaitingPayment = 1, Confirmed = 2, Reserved = 3, Preparing = 4, HandedToLogistics = 5, Shipped = 6, Delivered = 7, Closed = 8, Cancelled = 9, Returned = 10 }

public sealed record SaleCustomerIdentity(long CustomerId, string? BasalamUserId, string? Mobile, string? NationalCode, bool IsPublic);
public sealed record SaleLine(int ProductId, decimal Quantity, decimal UnitPrice);
public sealed record CreateIntegratedSale(
    string EventId, SaleChannel Channel, SaleCustomerIdentity? Customer, string? ExternalOrderId,
    IReadOnlyCollection<SaleLine> Lines, decimal TotalAmount, SalePaymentStatus PaymentStatus);

public sealed record IntegratedSale(
    string SaleId, string EventId, SaleChannel Channel, SaleCustomerIdentity Customer,
    string? ExternalOrderId, IReadOnlyCollection<SaleLine> Lines, decimal TotalAmount,
    SalePaymentStatus PaymentStatus, SaleWorkflowStatus Status, DateTime CreatedAtUtc);

public sealed record SaleWorkflowEvent(string EventId, string SaleId, string Type, string PayloadJson, DateTime OccurredAtUtc);

/// <summary>Pure business rules for booth/store sales. Persistence and transport are supplied by the host.</summary>
public sealed class IntegratedSaleWorkflow
{
    public const long PublicCustomerId = -1;
    private readonly Dictionary<string, IntegratedSale> _salesByEvent = new(StringComparer.Ordinal);
    private readonly List<SaleWorkflowEvent> _events = [];

    public IReadOnlyCollection<SaleWorkflowEvent> Events => _events.AsReadOnly();

    public IntegratedSale Create(CreateIntegratedSale command, DateTime? nowUtc = null)
    {
        if (_salesByEvent.TryGetValue(command.EventId, out var existing)) return existing; // idempotency
        if (string.IsNullOrWhiteSpace(command.EventId) || command.EventId.Length > 128)
            throw new ArgumentException("InvalidSaleEventId", nameof(command));
        if (command.Lines.Count == 0 || command.Lines.Any(x => x.ProductId <= 0 || x.Quantity <= 0 || x.UnitPrice < 0))
            throw new ArgumentException("InvalidSaleLines", nameof(command));
        if (command.TotalAmount < 0) throw new ArgumentException("InvalidSaleAmount", nameof(command));
        var customer = ResolveCustomer(command);
        var status = command.Channel == SaleChannel.Booth
            ? (command.PaymentStatus == SalePaymentStatus.Paid ? SaleWorkflowStatus.Confirmed : SaleWorkflowStatus.AwaitingPayment)
            : SaleWorkflowStatus.Confirmed;
        var sale = new IntegratedSale(Guid.NewGuid().ToString("N"), command.EventId, command.Channel, customer,
            command.ExternalOrderId, command.Lines.ToArray(), command.TotalAmount, command.PaymentStatus, status, nowUtc ?? DateTime.UtcNow);
        _salesByEvent.Add(command.EventId, sale);
        Emit(sale, "SaleCreated");
        if (status == SaleWorkflowStatus.Confirmed) Emit(sale, "SaleConfirmed");
        return sale;
    }

    public IntegratedSale ConfirmPayment(string eventId)
    {
        var sale = Get(eventId);
        if (sale.Channel != SaleChannel.Booth) return sale;
        sale = sale with { PaymentStatus = SalePaymentStatus.Paid, Status = SaleWorkflowStatus.Confirmed };
        _salesByEvent[eventId] = sale; Emit(sale, "PaymentConfirmed"); Emit(sale, "SaleConfirmed");
        return sale;
    }

    public IntegratedSale Transition(string eventId, SaleWorkflowStatus next)
    {
        var sale = Get(eventId);
        if (sale.Channel == SaleChannel.Booth && next is SaleWorkflowStatus.Reserved or SaleWorkflowStatus.Preparing or SaleWorkflowStatus.HandedToLogistics or SaleWorkflowStatus.Shipped
            && sale.PaymentStatus != SalePaymentStatus.Paid) throw new InvalidOperationException("BoothSaleMustBePaidBeforeFulfilment");
        if (next < sale.Status && next is not (SaleWorkflowStatus.Cancelled or SaleWorkflowStatus.Returned))
            throw new InvalidOperationException("InvalidSaleTransition");
        sale = sale with { Status = next }; _salesByEvent[eventId] = sale;
        Emit(sale, next switch { SaleWorkflowStatus.HandedToLogistics => "DeliveryCreated", SaleWorkflowStatus.Delivered => "SaleDelivered", SaleWorkflowStatus.Closed => "SaleClosed", _ => "SaleStatusChanged" });
        return sale;
    }

    private static SaleCustomerIdentity ResolveCustomer(CreateIntegratedSale command)
    {
        if (command.Channel == SaleChannel.Booth && (command.Customer is null || command.Customer.IsPublic || command.Customer.CustomerId <= 0))
            throw new InvalidOperationException("BoothSaleRequiresIdentifiedCustomer");
        return command.Customer ?? new SaleCustomerIdentity(PublicCustomerId, null, null, null, true);
    }
    private IntegratedSale Get(string eventId) => _salesByEvent.TryGetValue(eventId, out var sale) ? sale : throw new KeyNotFoundException("SaleNotFound");
    private void Emit(IntegratedSale sale, string type) => _events.Add(new SaleWorkflowEvent($"{sale.EventId}:{type}", sale.SaleId, type, $"{{\"saleId\":\"{sale.SaleId}\",\"customerId\":{sale.Customer.CustomerId}}}", DateTime.UtcNow));
}
