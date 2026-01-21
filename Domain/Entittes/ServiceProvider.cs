namespace Domain.Entittes
{
    public class ServiceProvider
    {
        public int Id { get; set; }
        public int ServiceCategoryId { get; set; }
        public ServiceCategory ServiceCategory { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public bool IsAvailable { get; set; }
    }
}
