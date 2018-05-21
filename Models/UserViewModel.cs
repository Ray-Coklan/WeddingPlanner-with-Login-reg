using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System;

namespace WeddingPlanner.Models
{
    public abstract class BaseEntity
    {
        // EMPTY!!!
    }
    public class RegUser : BaseEntity
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [MinLength(2, ErrorMessage="First Name must be at least 2 characters!")]
        public string FirstName { get;set; }
    
        [Required]
        [MinLength(2, ErrorMessage="Last Name must be at least 2 characters!")]
        public string LastName { get;set; }

        [Required]
        [EmailAddress]
        public string Email { get;set; }

        [Required]
        [MinLength(8, ErrorMessage="Password must be at least 8 characters!")]
        [DataType(DataType.Password)]
        public string Password { get;set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get;set; }
    }
    public class LoginUser : BaseEntity
    {
        [Required]
        [EmailAddress]
        public string Email { get;set; }

        [Required]
        [MinLength(8, ErrorMessage="Password must be at least 8 characters!")]
        [DataType(DataType.Password)]
        public string Password { get;set; }
    }

    public class CreateWedding : BaseEntity
    {
        [Required]
        [MinLength(2, ErrorMessage="Name must be at least 2 characters!")]
        public string WedderOne {get;set;}

        [Required]
        [MinLength(2, ErrorMessage="Name must be at least 2 characters!")]
        public string WedderTwo {get;set;}

        [Required]
        [MinLength(2, ErrorMessage="Address must be filled out!")]
        public string Address {get;set;}

        [Required]
        [FutureDate(ErrorMessage="Date must be in the future")]
        public DateTime WeddingDate {get;set;}

    }
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return value != null && (DateTime) value > DateTime.Now;
        }
    }
}