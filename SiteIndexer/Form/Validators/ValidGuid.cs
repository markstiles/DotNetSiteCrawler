using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace SiteIndexer.Form.Validators
{
    public class ValidGuid : ValidationAttribute
    {
        protected override ValidationResult IsValid(Object value, ValidationContext validationContext)
        {
            var passes = ValuePassesValidation(value);

            return passes
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage);
        }

        public override bool IsValid(object value)
        {
            var passes = ValuePassesValidation(value);

            return passes;
        }

        public bool ValuePassesValidation(object value)
        {
            var isCorrectValue = false;

            try
            {
                var g = (Guid)value;
                isCorrectValue = !g.Equals(Guid.Empty);
            }
            catch
            {
                // ignored
            }

            return isCorrectValue;
        }
    }
}