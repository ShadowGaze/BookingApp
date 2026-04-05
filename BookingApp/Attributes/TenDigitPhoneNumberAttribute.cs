using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class TenDigitPhoneNumberAttribute : ValidationAttribute
{
    public TenDigitPhoneNumberAttribute() : base("The phone number must start with +91 followed by exactly 10 digits.")
    { }

    public override bool IsValid(object value)
    {
        if (value == null)
            return false;

        string phoneNumber = value as string;

       
        return !string.IsNullOrEmpty(phoneNumber) &&
               phoneNumber.StartsWith("+91") &&
               phoneNumber.Length == 13 &&
               Regex.IsMatch(phoneNumber.Substring(3), @"^\d{10}$");
    }
}
