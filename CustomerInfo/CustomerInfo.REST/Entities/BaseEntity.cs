namespace CustomerInfo.REST.Entities
{
    public class BaseEntity
    {
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }

        public BaseEntity()
        {
            DateCreated = DateTime.Now;
            DateModified = DateTime.Now;
            IsDeleted = false;
        }

    }
}
