using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeApp.Models
{
    public class EmployeesData
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }
        [Display(Name = "Employee Name")]
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        [EmailAddress]
        [Display(Name = "Employee Email")]
        [Required(ErrorMessage = "Address is required.")]
        public string Email { get; set; }

        [Display(Name = "Employee Gender")]
        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }

        [Display(Name = "Date of Birth")]
        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        public DateOnly? DOB { get; set; }

        [Display(Name = "Profile Picture")]
        [Required(ErrorMessage = "Profile picture is required.")]
        [NotMapped]
        public IFormFile? Photo { get; set; }
        public string? PhotoPath { get; set; }
    }
}
