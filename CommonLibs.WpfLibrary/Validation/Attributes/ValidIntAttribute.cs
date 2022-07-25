using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CommonLibs.WpfLibrary.Validation.Attributes
{
    public class ValidIntAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return true;

            var str = value.ToString();
            return string.IsNullOrWhiteSpace(str) || int.TryParse(str, out int result);
        }
    }
}
