using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OchronaDanych.Attributes
{
    public class CheckPasswordAttribute : ValidationAttribute
    {
        public CheckPasswordAttribute()
        {

        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string strValue = (string)value;
            if(strValue.Any(char.IsUpper) && strValue.Any(char.IsLower) && strValue.Any(char.IsDigit))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(GetErrorMessage());
            }
        }

        public string GetErrorMessage()
        {
            return $"Your password is not strong enough. It must contain a lowercase character, uppercase character and a digit.";
        }
    }
}
