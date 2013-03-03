// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.XPath;

namespace NTier.Common.Domain.Model
{
    /// <summary>
    /// Provides methods for getting and setting name-value pairs from an xml string.
    /// </summary>
    public static class DynamicPropertyHelper
    {
        /// <summary>
        /// Stores dynamic content (name-value pairs) as xml
        /// </summary>
        /// <param name="dynamicContent">The xml string to hold the dynamic content as xml</param>
        /// <param name="propertyName">The name of the dynamic property</param>
        /// <param name="propertyValue">The value of the dynamic property</param>
        public static void SetDynamicStringProperty(ref string dynamicContent, string propertyName, string propertyValue)
        {
            // new value is null
            if (propertyValue == null)
            {
                // remove value if existing
                if (!string.IsNullOrEmpty(dynamicContent))
                {
                    var root = XElement.Parse(dynamicContent);
                    var element = root.XPathSelectElement(string.Format("/{0}", propertyName));
                    if (element != null)
                    {
                        element.Remove();
                        dynamicContent = root.ToString(SaveOptions.DisableFormatting);
                    }
                }
            }
            else
            {
                // retrieve xml node
                XElement root = null;
                XElement element = null;
                if (string.IsNullOrEmpty(dynamicContent))
                {
                    root = new XElement(XName.Get("fields"));
                }
                else
                {
                    root = XElement.Parse(dynamicContent);
                    element = root.XPathSelectElement(string.Format("/{0}", propertyName));
                }

                // create xml node if not existing
                if (element == null)
                {
                    element = new XElement(XName.Get(propertyName));
                    root.Add(element);
                }

                // set value
                element.Value = propertyValue;

                // store xml
                dynamicContent = root.ToString(SaveOptions.DisableFormatting);
            }
        }

        /// <summary>
        /// Rertieves a property value from an xml string containing dynamic content as name-value pairs
        /// </summary>
        /// <param name="dynamicContent">The xml string holding the dynamic content as xml</param>
        /// <param name="propertyName">The name of the dynamic property</param>
        /// <returns>The value of the dynamic property</returns>
        public static string GetDynamicStringProperty(string dynamicContent, string propertyName)
        {
            if (string.IsNullOrEmpty(dynamicContent))
            {
                return null;
            }
            else
            {
                var root = XElement.Parse(dynamicContent);
                var element = root.XPathSelectElement(string.Format("/{0}", propertyName));
                if (element == null)
                {
                    return null;
                }
                else
                {
                    return element.Value;
                }
            }
        }

#if !SILVERLIGHT
        // note: as BinaryFormatter is not available for Silverlight, this code is for .NET platform only

        /// <summary>
        /// Stores dynamic content (name-value pairs) as xml
        /// </summary>
        /// <remarks>Values are stored in xml as binary formatted objects, base64 encoded.</remarks>
        /// <param name="dynamicContent">The xml string to hold the dynamic content as xml</param>
        /// <param name="propertyName">The name of the dynamic property</param>
        /// <param name="propertyValue">The value of the dynamic property</param>
        public static void SetDynamicProperty(ref string dynamicContent, string propertyName, object propertyValue)
        {
            // new value is null
            if (propertyValue == null)
            {
                // remove value if existing
                if (!string.IsNullOrEmpty(dynamicContent))
                {
                    var root = XElement.Parse(dynamicContent);
                    var element = root.XPathSelectElement(string.Format("/{0}", propertyName));
                    if (element != null)
                    {
                        element.Remove();
                        dynamicContent = root.ToString(SaveOptions.DisableFormatting);
                    }
                }
            }
            else
            {
                // encode value
                var byteArray = SerializeObject(propertyValue);
                var base64 = EncodeToBase64(byteArray);

                // retrieve xml node
                XElement root = null;
                XElement element = null;
                if (string.IsNullOrEmpty(dynamicContent))
                {
                    root = new XElement(XName.Get("fields"));
                }
                else
                {
                    root = XElement.Parse(dynamicContent);
                    element = root.XPathSelectElement(string.Format("/{0}", propertyName));
                }

                // create xml node if not existing
                if (element == null)
                {
                    element = new XElement(XName.Get(propertyName));
                    root.Add(element);
                }

                // set value
                element.Value = base64;

                // store xml
                dynamicContent = root.ToString(SaveOptions.DisableFormatting);
            }
        }

        /// <summary>
        /// Rertieves a property value from an xml string containing dynamic content as name-value pairs
        /// </summary>
        /// <param name="dynamicContent">The xml string holding the dynamic content as xml</param>
        /// <param name="propertyName">The name of the dynamic property</param>
        /// <returns>The value of the dynamic property</returns>
        public static object GetDynamicProperty(string dynamicContent, string propertyName)
        {
            if (string.IsNullOrEmpty(dynamicContent))
            {
                return null;
            }
            else
            {
                var root = XElement.Parse(dynamicContent);
                var element = root.XPathSelectElement(string.Format("/{0}", propertyName));
                if (element == null)
                {
                    return null;
                }
                else
                {
                    // decode
                    var base64 = element.Value;
                    var byteArray = DecodeFromBase64(base64);
                    object obj = DeserializeObject(byteArray);
                    return obj;
                }
            }
        }

        private static byte[] SerializeObject(object obj)
        {
            using (var stream = new System.IO.MemoryStream())
            {
                IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Seek(0, System.IO.SeekOrigin.Begin);

                byte[] byteArray = new byte[stream.Length];
                stream.Read(byteArray, 0, (int)stream.Length);
                return byteArray;
            }
        }

        private static object DeserializeObject(byte[] byteArray)
        {
            using (var stream = new System.IO.MemoryStream(byteArray))
            {
                IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                var obj = formatter.Deserialize(stream);
                return obj;
            }
        }

        private static string EncodeToBase64(byte[] toEncode)
        {
            string encodedData = System.Convert.ToBase64String(toEncode);
            return encodedData;
        }

        private static byte[] DecodeFromBase64(string toDecode)
        {
            byte[] decodedData = System.Convert.FromBase64String(toDecode);
            return decodedData;
        }
#endif
    }
}
