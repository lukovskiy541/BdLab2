using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bd2Domain.Model
{
    public partial class PublicationType
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва типу є обов'язковою")]
        [DisplayName("Назва типу")]
        public string TypeName { get; set; }

        public virtual ICollection<Publication> Publications { get; set; } = new List<Publication>();
    }
}
