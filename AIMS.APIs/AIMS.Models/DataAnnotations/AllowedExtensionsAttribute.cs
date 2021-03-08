using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;

namespace AIMS.Models.DataAnnotations
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly List<string> extensions;
        public AllowedExtensionsAttribute(string[] exts)
        {
            foreach(var ext in exts)
            {
                extensions.Add(ext);
            }
        }

        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!extensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"This photo extension is not allowed!";
        }
    }
}
