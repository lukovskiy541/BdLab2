using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bd2Domain.Model
{
    public partial class Reviewer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ім'я є обов'язковим")]
        [DisplayName("Ім'я")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Прізвище є обов'язковим")]
        [DisplayName("Прізвище")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Електронна пошта є обов'язковою")]
        [EmailAddress(ErrorMessage = "Введіть дійсну електронну пошту")]
        [DisplayName("Електронна пошта")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Ідентифікатор кафедри є обов'язковим")]
        [DisplayName("Департамент")]
        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; }

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
