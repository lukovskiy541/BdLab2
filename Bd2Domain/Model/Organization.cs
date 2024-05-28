using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bd2Domain.Model
{
    public partial class Organization
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва є обов'язковою")]
        [DisplayName("Назва")]
        public string Name { get; set; }

        [Required(ErrorMessage = "URL є обов'язковим")]
        [DisplayName("URL")]
        public string Url { get; set; }

        public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}
