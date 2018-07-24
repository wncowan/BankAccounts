using System;

namespace BankAccounts.Models
{

    public class Transaction : BaseEntity{
        public int TransactionId {get; set;}
        public string TransactionType {get;set;}
        public int Amount {get;set;}
        public int CurrentBalance {get;set;}
        public DateTime created_at {get;set;}
        public DateTime updated_at {get;set;}
        public int UserId {get;set;}
        public User User {get;set;}
    }
}