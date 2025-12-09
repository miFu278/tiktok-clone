namespace TikTok.Shared.Common.Abstractions.Entities
{
    public abstract class SoftDeletableEntity : AuditableEntity
    {
        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public Guid? DeletedBy { get; private set; }

        protected SoftDeletableEntity() : base()
        {
            IsDeleted = false;
        }

        protected SoftDeletableEntity(Guid id) : base(id)
        {
            IsDeleted = false;
        }

        public void SoftDelete(Guid? userId)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedBy = userId;
        }

        public void Restore()
        {
            IsDeleted = false;
            DeletedAt = null;
            DeletedBy = null;
        }
    }
}
