using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace BookingApp.Models
{
    public class Register
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        [TenDigitPhoneNumber]
        public string Number { get; set; }

        [Required]
        [EmailAddress]
        public string EmailId { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Username { get; set; }

        
        [Required]
        //[StringLength(15, MinimumLength = 6)]
        [PasswordComplexity]
        public string Password { get; set; }


        public Register()
        {
        }
     }

       
}


