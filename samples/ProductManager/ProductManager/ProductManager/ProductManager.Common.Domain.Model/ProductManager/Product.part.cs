using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using NTier.Common.Domain.Model;

namespace ProductManager.Common.Domain.Model.ProductManager
{
    partial class Product
    {
        #region Addtitional Properties

        [DataMember]
        [SimpleProperty]
        public string Notes
        {
            get { return _notes; }
            set
            {
                if (_notes != value)
                {
                    OnPropertyChanging("Notes", value);
                    var previousValue = _notes;
                    _notes = value;
                    OnPropertyChanged("Notes", previousValue, value, trackInChangeTracker: true);
                }
            }
        }
        private string _notes;

        #endregion Addtitional Properties

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
