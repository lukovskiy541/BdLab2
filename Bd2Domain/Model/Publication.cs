using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bd2Domain.Model
{
    public partial class Publication
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва є обов'язковою")]
        [DisplayName("Назва")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Опис є обов'язковим")]
        [DisplayName("Опис")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Дата публікації є обов'язковою")]
        [DisplayName("Дата публікації")]
        public DateOnly PublicationDate { get; set; }

        [Required(ErrorMessage = "Ідентифікатор автора є обов'язковим")]
        [DisplayName("Автор")]
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "Ідентифікатор тематики є обов'язковим")]
        [DisplayName("Тематика")]
        public int SubjectId { get; set; }

        [Required(ErrorMessage = "Ключові слова є обов'язковими")]
        [DisplayName("Ключові слова")]
        public string Keywords { get; set; }

        [Required(ErrorMessage = "Мова є обов'язковою")]
        [DisplayName("Мова")]
        public string Language { get; set; }

        [Required(ErrorMessage = "URL PDF є обов'язковим")]
        [DisplayName("URL PDF")]
        public string PdfUrl { get; set; }

        [Required(ErrorMessage = "Кількість цитувань є обов'язковою")]
        [DisplayName("Кількість цитувань")]
        public int CitationsNumber { get; set; }

        [Required(ErrorMessage = "Статус є обов'язковим")]
        [DisplayName("Статус")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Ідентифікатор журналу є обов'язковим")]
        [DisplayName("Журнал")]
        public int JournalId { get; set; }

        [Required(ErrorMessage = "Ідентифікатор типу публікації є обов'язковим")]
        [DisplayName("Тип публікації")]
        public int TypeId { get; set; }


        [DisplayName("Журнал")]
        public virtual Journal Journal { get; set; }

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        [DisplayName("Тип публікації")]
        public virtual PublicationType Type { get; set; }
        [DisplayName("Автори")]
        public virtual ICollection<Author> Authors { get; set; } = new List<Author>();
        [DisplayName("Тематики")]
        public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    }
}
