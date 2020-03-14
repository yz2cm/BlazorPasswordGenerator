using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BlazorPasswordGenerator.Data
{
    public class PasswordGeneratorContext
    {
        [Required(ErrorMessage = "Password count is required.")]
        public string passwordCount { get; set; } = string.Empty;
        public string passwordLength { get; set; } = string.Empty;
        public char[] availableCharacters = new char[0];
    }
}
