using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class PasswordComplexityAttribute : ValidationAttribute
{
    public PasswordComplexityAttribute() : base("The password must be alphanumeric, include at least one uppercase letter, and be 6-15 characters long.")
    { }

    public override bool IsValid(object value)
    {
        if (value == null)
            return false;

        string password = value as string;

       
        return !string.IsNullOrEmpty(password) &&
               password.Length >= 6 && password.Length <= 15 &&
               Regex.IsMatch(password, @"^[a-zA-Z0-9]+$") && 
               Regex.IsMatch(password, @"[A-Z]");
    }
}
