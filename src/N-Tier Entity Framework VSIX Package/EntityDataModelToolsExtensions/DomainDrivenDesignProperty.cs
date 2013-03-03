using System;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Data.Entity.Design.Extensibility;

namespace NTier.Domain.EntityDataModelToolsExtensions
{
    /// <summary>
    /// This class has one public property, AggregateRootProperty. This property is visible in the Visual Studio Properties window when a conceptual model entity type is selected in the Entity Designer or in the Model Browser.
    /// This property and its value are saved as a structured annotation in an EntityType element in the conceptual content of an .edmx document.
    /// </summary>
    public class DomainDrivenDesignProperty : Property
    {
        private const string Namespace = "http://schemas.trivadis.com/dotnet/2011/11/edm/DomainDrivenDesign";

        public DomainDrivenDesignProperty(XElement parent, PropertyExtensionContext context)
            : base(parent, context, Namespace)
        {
        }

        // This property is saved in the conceptual content of an .edmx document in the following format:
        // <EntityType>
        //     <!-- other entity properties -->
        //     <AggregateRoot xmlns="http://schemas.trivadis.com/dotnet/2011/11/edm/DomainDrivenDesign">True</AggregateRoot>
        // </EntityType>
        [DisplayName("Aggregate Root")]
        [Description("Determines whether the entity is an aggregate root.")]
        [Category(DomainDrivenDesignProperty.Category)]
        [DefaultValue(true)]
        public bool AggregateRoot
        {
            // Read and return the property value from the structured annotation in the EntityType element.
            get
            {
                return GetProperty("AggregateRoot", defaultValue: true);
            }
            // Write the new property value to the structured annotation in the EntityType element.
            set
            {
                SetProperty("AggregateRoot", value);
            }
        }
    }
}
