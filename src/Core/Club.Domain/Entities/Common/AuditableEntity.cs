namespace Hyper.Domain.Entities.Common;

public abstract class HyperBaseCoreConfigAuditableEntity<TKey> : BaseCoreConfigAuditableEntity<TKey>
    where TKey : struct
{
    [NotMapped]
    [DisplayName("ایجاد کننده")]
    public User? CreatedBy { get; set; }
    
    [NotMapped]
    [DisplayName("تغییر دهنده")]
    public User? LastModifiedBy { get; set; }
}

public abstract class HyperBaseCoreAuditableEntity<TKey> : BaseCoreAuditableEntity<TKey>
    where TKey : struct
{
    [NotMapped]
    [DisplayName("ایجاد کننده")]
    public User? CreatedBy { get; set; }
    
    [NotMapped]
    [DisplayName("تغییر دهنده")]
    public User? LastModifiedBy { get; set; }
}

public abstract class HyperBaseCoreLogAuditableEntity<TKey> : BaseCoreLogAuditableEntity<TKey>
    where TKey : struct
{
    [NotMapped]
    [DisplayName("ایجاد کننده")]
    public User? CreatedBy { get; set; }
    
    [NotMapped]
    [DisplayName("تغییر دهنده")]
    public User? LastModifiedBy { get; set; }
}
