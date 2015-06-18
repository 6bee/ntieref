using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ProductManager.Common.Domain.Model.ProductManager
{
    // this code containes validation logic and was mannually copied from .net version
    public sealed class ProductMetadata
    {
        private static readonly DateTime MinValidDate = new DateTime(1753, 01, 01);
        private static readonly DateTime MaxValidDate = new DateTime(9999, 12, 31, 23, 59, 59, 997);
        public static ValidationResult CheckDateTimeRange(DateTime? value)
        {
            if (!value.HasValue || (value >= MinValidDate && value <= MaxValidDate))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(null);
        }

        public static ValidationResult CheckSellEndDate(DateTime? sellEndDate, ValidationContext validationContext)
        {
            var sellStartDate = ((Product)validationContext.ObjectInstance).SellStartDate;
            if (!sellEndDate.HasValue || sellEndDate >= sellStartDate)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(null);
        }
    }
}
