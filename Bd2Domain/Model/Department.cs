using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bd2Domain.Model
{
    public partial class Department
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва є обов'язковою")]
        [DisplayName("Назва")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Ідентифікатор організації є обов'язковим")]
        [DisplayName("Організація")]
        public int OrganizationId { get; set; }

        public virtual ICollection<Author> Authors { get; set; } = new List<Author>();
        [Required(ErrorMessage = "Ідентифікатор організації є обов'язковим")]
        [DisplayName("Організація")]
        public virtual Organization Organization { get; set; }

        public virtual ICollection<Reviewer> Reviewers { get; set; } = new List<Reviewer>();
    }
}
