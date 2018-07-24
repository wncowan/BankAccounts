
using System.ComponentModel.DataAnnotations;

namespace BankAccounts.Models
{
    public class TransactionViewModel : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "You must enter amount")]
        public int Amount { get; set; }

    }
}