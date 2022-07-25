using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CommonLibs.WpfLibrary.Validation.Attributes
{
    public class ValidFloatAttribute : System.ComponentModel.DataAnnotations.ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return true;

            var str = value.ToString();

            return string.IsNullOrWhiteSpace(str) || float.TryParse(str, out float result);
        }
    }
}
