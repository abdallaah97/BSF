using System.ComponentModel.DataAnnotations;

namespace Domain.Entittes
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [StringLength(500)]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        public string Email { get; set; }
        [Required]
        [StringLength(20)]
        public string PhonNumber { get; set; }
        [Required]
        public string Password { get; set; }
        public string? PersonalPhoto { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
