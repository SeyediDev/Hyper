namespace Hyper.Integration.Contracts;

public enum ProductPreparationMode : byte { Hold = 1, Adapt = 2 }
public enum ProductTransferDirection : byte { ToPlatform = 1, ToAccounting = 2 }
public sealed record ProductPreparationPolicy(ProductPreparationMode Mode = ProductPreparationMode.Hold,
    bool TrimDescription = false, bool TruncateDescription = false);
public sealed record ProductPreparationInput(ProductTransferDirection Direction, string SourceProductId,
    int? CategoryId = null, int? PreparationDays = null, int? PackageWeight = null,
    string? Description = null, long? PhotoId = null);
public sealed record ProductPreparationIssue(string Field, string Code);
public sealed record ProductPreparationChange(string Field, string Rule, string? Before, string? After);
public sealed record ProductPreparationRevision(int Revision, string Actor, DateTime AtUtc,
    ProductPreparationInput Input, ProductPreparationInput Prepared, ProductPreparationPolicy Policy,
    IReadOnlyList<ProductPreparationIssue> Issues, IReadOnlyList<ProductPreparationChange> Changes);
public sealed record ProductPreparationItem(long Id, int Revision, ProductPreparationInput Input,
    ProductPreparationInput Prepared, string Status, IReadOnlyList<ProductPreparationIssue> Issues,
    IReadOnlyList<ProductPreparationChange> Changes, Guid? DraftRequestId, long? JobId,
    string? ExternalProductId, IReadOnlyList<ProductPreparationRevision> History);
public sealed record ProductPreparationBoard(int Total, int NeedsAttention, int Queued,
    int Completed, int AdaptedCompleted, IReadOnlyList<ProductPreparationItem> Items);
public interface IIntegrationProductReadinessApi
{
    Task<ProductPreparationPolicy?> PolicyAsync(IntegrationConnectionCommandRequest scope, ProductTransferDirection direction, CancellationToken ct);
    Task<bool> SetPolicyAsync(IntegrationConnectionCommandRequest scope, ProductTransferDirection direction, ProductPreparationPolicy policy, string actor, CancellationToken ct);
    Task<ProductPreparationItem?> PrepareAsync(IntegrationConnectionCommandRequest scope, ProductPreparationInput input, int expectedRevision, string actor, CancellationToken ct);
    Task<ProductPreparationBoard?> BoardAsync(IntegrationConnectionCommandRequest scope, int skip, int take, CancellationToken ct);
}
