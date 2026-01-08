namespace Domain.Entittes
{
    public class ClientUser
    {
        public int Id { get; set; }
        public DateTime BarthDate { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
