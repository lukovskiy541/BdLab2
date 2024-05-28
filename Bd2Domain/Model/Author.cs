using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations; // Додано простір імен

namespace Bd2Domain.Model
{
    public partial class Author
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Біографія є обов'язковою")]
        [DisplayName("Біографія")]
        public string Biography { get; set; }

        [Required(ErrorMessage = "Прізвище є обов'язковим")]
        [DisplayName("Прізвище")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Ім'я є обов'язковим")]
        [DisplayName("Ім'я")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Країна є обов'язковою")]
        [DisplayName("Країна")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Електронна пошта є обов'язковою")]
        [DisplayName("Електронна пошта")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Поле є обов'язковою")]
        [DisplayName("Відділ")]
        public int DepartmentId { get; set; }
        [Required(ErrorMessage = "Поле є обов'язковою")]
        [DisplayName("Відділ")]
        public virtual Department Department { get; set; }

        public virtual ICollection<Publication> Publications { get; set; } = new List<Publication>();
    }
}
