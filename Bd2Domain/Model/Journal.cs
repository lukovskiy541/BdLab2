using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bd2Domain.Model
{
    public partial class Journal
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва є обов'язковою")]
        [DisplayName("Назва")]
        public string Name { get; set; }

        [Required(ErrorMessage = "URL є обов'язковим")]
        [DisplayName("URL")]
        public string Url { get; set; }

        [Required(ErrorMessage = "ISSN є обов'язковим")]
        [DisplayName("ISSN")]
        public string Issn { get; set; }

        public virtual ICollection<Publication> Publications { get; set; } = new List<Publication>();
    }
}
