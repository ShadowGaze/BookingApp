using System;
using System.ComponentModel.DataAnnotations;


namespace BookingApp.Models
{
    public class Login
    {
        [Key]       
        public string Username { get; set; }
        public string Password { get; set; }
        
    }
}
