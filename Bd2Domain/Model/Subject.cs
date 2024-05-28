using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bd2Domain.Model
{
    public partial class Subject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва є обов'язковою")]
        [DisplayName("Назва")]
        public string Name { get; set; }

        public virtual ICollection<Publication> Publications { get; set; } = new List<Publication>();
    }
}
