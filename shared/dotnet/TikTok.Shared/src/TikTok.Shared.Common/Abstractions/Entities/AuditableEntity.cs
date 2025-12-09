namespace TikTok.Shared.Common.Abstractions.Entities
{
    public abstract class AuditableEntity : BaseEntity
    {
        public DateTime CreatedAt { get; protected set; }
        public Guid? CreatedBy { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }
        public Guid? UpdatedBy { get; protected set; }

        protected AuditableEntity() : base()
        {
            CreatedAt = DateTime.UtcNow;
        }

        protected AuditableEntity(Guid id) : base(id)
        {
            CreatedAt = DateTime.UtcNow;
        }

        public void SetCreated(Guid? userId)
        {
            CreatedAt = DateTime.UtcNow;
            CreatedBy = userId;
        }

        public void SetUpdated(Guid? userId)
        {
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = userId;
        }
    }
}
