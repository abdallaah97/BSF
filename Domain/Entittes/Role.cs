using Domain.Enums;

namespace Domain.Entittes
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SytemRole Code { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
