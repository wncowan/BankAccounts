using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace BankAccounts.Models 
{
    public class User : BaseEntity
    {
        public int UserId {get; set;}

        [Required(ErrorMessage="Enter First Name")]
        [Display(Name="First Name")]
        [MinLength(2)]
        public string FirstName {get; set;}
        
        [Required(ErrorMessage="Enter Last Name")]
        [Display(Name="Last Name")]
        public string LastName {get; set;}

        [Required(ErrorMessage="Enter Email")]
        [EmailAddress]
        [Display(Name="Email Address")]
        public string Email {get; set;}

        [Required(ErrorMessage="Enter Password")]
        [MinLength(8)]
        [DataType(DataType.Password)]
        [Display(Name="Password")]
        public string Password { get; set; }
        public int Balance {get;set;}

        // [Required(ErrorMessage="Confirm Password")]
        // [DataType(DataType.Password)]
        // [Compare("Password", ErrorMessage="Error confirming Password")]
        // [Display(Name="Confirm Password")]
        // public string ConfirmPassword { get; set; }

        public DateTime created_at {get; set;}

        public List<Transaction> Transactions {get; set;}
        public User(){
            Transactions = new List<Transaction>();
            Balance = 0;
        }
    }


}