using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace SiteIndexer.Models.FormModels.Validators
{
    public class ValidLongList : ValidationAttribute
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
                var list = (List<long>)value;
                if (list.Count > 0 && list.All(a => a > 0))
                    isCorrectValue = true;
            }
            catch
            {
                // ignored
            }

            return isCorrectValue;
        }
    }
}