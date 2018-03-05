using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Data.Entity.Design.Extensibility;

namespace NTier.Domain.EntityDataModelToolsExtensions
{
    /// <summary>
    /// This class has one public property, CompositionRootProperty. This property is visible in the Visual Studio Properties window when a conceptual model entity type is selected in the Entity Designer or in the Model Browser.
    /// This property and its value are saved as a structured annotation in an EntityType element in the conceptual content of an .edmx document.
    /// </summary>
    public abstract class Property
    {
        internal const string Category = "N-Tier Entity Framework Property";

        private readonly XElement Parent;
        private readonly PropertyExtensionContext Context;
        private readonly string Namespace;


        public Property(XElement parent, PropertyExtensionContext context, string xmlNamespace)
        {
            Context = context;
            Parent = parent;
            Namespace = xmlNamespace;
        }


        protected void SetProperty<T>(string propertyName, T propertyValue)
        {
            var propertyXName = XName.Get(propertyName, Namespace);

            // Make changes to the .edmx document in an EntityDesignerChangeScope to enable undo/redo of changes.
            using (var scope = Context.CreateChangeScope("Set CompositionRootProperty"))
            {
                //if (Parent.HasElements)
                //{
                //    var lastChild = Parent.Elements()
                //        .Where(element => element != null && element.Name == propertyXName)
                //        .LastOrDefault();

                //    if (lastChild != null)
                //    {
                //        // CompositionRootProperty element already exists under the EntityType element, so update its value.
                //        lastChild.SetValue(propertyValue);
                //    }
                //    else
                //    {
                //        // CompositionRootProperty element does not exist, so create a new one as the last child of the EntityType element.
                //        Parent.Elements().Last().AddAfterSelf(new XElement(propertyXName, propertyValue));
                //    }
                //}
                if (Parent.HasAttributes)
                {
                    var attribute = Parent.Attributes()
                        .Where(a => a != null && a.Name == propertyXName)
                        .SingleOrDefault();

                    if (attribute != null)
                    {
                        // CompositionRootProperty element already exists under the EntityType element, so update its value.
                        attribute.SetValue(propertyValue);
                    }
                    else
                    {
                        // CompositionRootProperty element does not exist, so create a new one as the last child of the EntityType element.
                        Parent.SetAttributeValue(propertyXName, propertyValue);
                    }
                }
                else
                {
                    // The EntityType element has no child elements so create a new CompositionRootProperty element as its first child.
                    Parent.Add(new XElement(propertyXName, propertyValue));
                }

                // Commit the changes.
                scope.Complete();
            }
        }


        private string GetProperty(string propertyName)
        {
            var propertyXName = XName.Get(propertyName, Namespace);

            if (Parent.HasElements)
            {
                //var element = Parent.Elements()
                //    .Where(e => e != null && e.Name == propertyXName)
                //    .LastOrDefault();

                //if (element != null)
                //{
                //    // CompositionRoot element exists, so get its value.
                //    return element.Value.Trim();
                //}

                var attribute = Parent.Attributes()
                    .Where(a => a != null && a.Name == propertyXName)
                    .SingleOrDefault();

                if (attribute != null)
                {
                    // CompositionRoot element exists, so get its value.
                    return attribute.Value.Trim();
                }
            }

            return string.Empty;
        }


        protected T GetProperty<T>(string propertyName, T defaultValue = default(T))
        {
            var stringValue = GetProperty(propertyName);

            if (typeof(T) == typeof(bool))
            {
                bool boolValue;
                if (bool.TryParse(stringValue, out boolValue))
                {
                    return (T)(object)boolValue;
                }

                return defaultValue;
            }
            // TODO: implement type parser as required 

            throw new NotImplementedException(string.Format("The custom property '{2}' may not be parsed. The property parser {0} does not support type {1}.", this.GetType().Name, typeof(T).Name, propertyName));
        }


        //protected bool GetBooleanProperty(string propertyName, bool defaultValue = false)
        //{
        //    var stringValue = GetProperty(propertyName);

        //    bool boolValue;
        //    if (bool.TryParse(stringValue, out boolValue))
        //    {
        //        return boolValue;
        //    }

        //    return defaultValue;
        //}
    }
}