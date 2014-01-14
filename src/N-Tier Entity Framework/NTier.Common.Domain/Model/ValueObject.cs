// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Reflection;

namespace NTier.Common.Domain.Model
{
    [DataContract(IsReference = true)]
    public abstract class ValueObject : INotifyPropertyChanging, INotifyPropertyChanged //, IDataErrorInfo
    {
        #region Indexer

        /// <summary>
        /// Gets or sets the value of a property. 
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        [Display(AutoGenerateField = false)]
        public virtual object this[string propertyName]
        {
            get { return GetProperty(propertyName); }
            set { SetProperty(propertyName, value); }
        }

        private object GetProperty(string propertyName, bool includePrivateProperties = false)
        {
            if (includePrivateProperties)
            {
                return GetType().InvokeMember(propertyName, BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, this, null);
            }
            else
            {
                return GetType().InvokeMember(propertyName, BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public, null, this, null);
            }
        }

        private void SetProperty(string propertyName, object value, bool includePrivateProperties = false)
        {
            try
            {
                if (includePrivateProperties)
                {
                    GetType().InvokeMember(propertyName, BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, this, new[] { value });
                }
                else
                {
                    GetType().InvokeMember(propertyName, BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public, null, this, new[] { value });
                }
            }
            catch (TargetInvocationException ex)
            {
                // unwrap validation exception
                var validationException = ex.InnerException as ValidationException;
                if (validationException != null) throw validationException;
                throw;
            }
        }

        #endregion Indexer

        #region IsDeserializing

        [Display(AutoGenerateField = false)]
        protected bool IsDeserializing
        {
            get { return _isDeserializing; }
        }
#if !SILVERLIGHT
        [NonSerialized]
#endif
        private bool _isDeserializing;

        #endregion IsDeserializing

        #region OnDeserializing / OnDeserialized

        [OnDeserializing]
#if SILVERLIGHT
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public
#endif
        void OnDeserializingMethod(StreamingContext context)
        {
            _isDeserializing = true;
        }

        [OnDeserialized]
#if SILVERLIGHT
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public
#endif
        void OnDeserializedMethod(StreamingContext context)
        {
            _isDeserializing = false;
        }

        #endregion OnDeserializing / OnDeserialized

        #region INotifyPropertyChanged

#if !SILVERLIGHT
        [field: NonSerialized]
#endif
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if (_isDeserializing) return;

            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedWithValuesEventArgs(propertyName, oldValue, newValue));
            }
        }

        #endregion INotifyPropertyChanged

        #region INotifyPropertyChanging

#if !SILVERLIGHT
        [field: NonSerialized]
#endif
        public event PropertyChangingEventHandler PropertyChanging;

        protected virtual void OnPropertyChanging(string propertyName, object oldValue, object newValue)
        {
            if (_isDeserializing) return;

            var propertyChanging = PropertyChanging;
            if (propertyChanging != null)
            {
                propertyChanging(this, new PropertyChangingWithValuesEventArgs(propertyName, oldValue, newValue));
            }
        }

        #endregion INotifyPropertyChanging

        #region Operator overloading

        public static bool operator ==(ValueObject obj1, ValueObject obj2)
        {
            return (ReferenceEquals(obj1, null) && ReferenceEquals(obj2, null)) || (!ReferenceEquals(obj1, null) && obj1.Equals(obj2));
        }

        public static bool operator !=(ValueObject obj1, ValueObject obj2)
        {
            return !(obj1 == obj2);
        }

        //public abstract override bool Equals(object obj);

        //public abstract override int GetHashCode();

        #endregion Operator overloading
    }
}
