// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NTier.Common.Domain.Model
{
    /// <summary>
    /// ClientInfo holds name-value pairs to be transmitted to the server within service requests
    /// </summary>
    [DataContract(IsReference = true)]
    public sealed class ClientInfo : INotifyPropertyChanged
    {
        public ClientInfo()
        {
            this.Content = new ExtendedPropertiesDictionary();
        }

#if SILVERLIGHT
        [DataMember]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public ExtendedPropertiesDictionary Content { get; set; }
#else
        [DataMember]
        private ExtendedPropertiesDictionary Content { get; set; }
#endif

        /// <summary>
        /// Gets or sets the value associated with the specified property name
        /// </summary>
        /// <remarks>If you use DataContractSerializer for object serialization between client and server, 
        /// all objects stored in the ClientInfo instance must be searizable using DataContractSerializer.</remarks>
        /// <param name="propertyName">The name of the property</param>
        /// <returns>The value of the property</returns>
        public object this[string propertyName]
        {
            get
            {
                return Content.ContainsKey(propertyName) ? Content[propertyName] : null;
            }
            set
            {
                if (value == null && Content.ContainsKey(propertyName))
                {
                    Content.Remove(propertyName);
                }
                else
                {
                    Content[propertyName] = value;
                }

                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
