namespace Hyper.Domain.Entities.Customers.Enums;

public enum CustomerTransactionType 
{ 
    [Description("بدهکار")]
    Debit = 1, 
    
    [Description("بستانکار")]
    Credit = 2,
}
