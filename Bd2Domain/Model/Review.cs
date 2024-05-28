using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bd2Domain.Model
{
    public partial class Review
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Рецензія є обов'язковою")]
        [DisplayName("Рецензія")]
        public string Comment { get; set; }

        [Required(ErrorMessage = "Дата подання є обов'язковою")]
        [DisplayName("Дата подання")]
        public DateOnly SubmissionDate { get; set; }

        [Required(ErrorMessage = "Ідентифікатор рецензента є обов'язковим")]
        [DisplayName("Рецензент")]
        public int ReviewerId { get; set; }

        [Required(ErrorMessage = "Ідентифікатор публікації є обов'язковим")]
        [DisplayName("Публікація")]
        public int PublicationId { get; set; }
        [Required(ErrorMessage = "Поле є обов'язковим")]
        [DisplayName("Публікація")]
        public virtual Publication Publication { get; set; }
        [Required(ErrorMessage = "Поле є обов'язковим")]
        [DisplayName("Рецензент")]
        public virtual Reviewer Reviewer { get; set; }
    }
}
