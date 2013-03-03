using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using NTier.Common.Domain.Model;

namespace IntegrationTest.Common.Domain.Model.Northwind
{
    partial class DynamicContentEntity
    {
        #region Dynamic property support

        protected override object GetDynamicValue(string propertyName)
        {
            return DynamicPropertyHelper.GetDynamicProperty(_dynamicContent, propertyName);
        }

        protected override void SetDynamicValue(string propertyName, object value)
        {
            var dynamicContent = DynamicContent;
            DynamicPropertyHelper.SetDynamicProperty(ref dynamicContent, propertyName, value);
            DynamicContent = dynamicContent;
        }

        #endregion Dynamic property support
    }
}
