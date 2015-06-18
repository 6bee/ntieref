using System;

// copied from .NET Framework since missing in Silverlight
namespace System.ComponentModel
{
    /// <summary>
    /// Notifies clients that a property value has changed.
    /// </summary>
    public interface INotifyPropertyChanging
    {
        /// <summary>
        /// Occurs when a property value is changing.
        /// </summary>
        event PropertyChangingEventHandler PropertyChanging;
    }

    /// <summary>
    /// Represents the method that will handle the System.ComponentModel.INotifyPropertyChanging.PropertyChanging event 
    /// of an System.ComponentModel.INotifyPropertyChanging interface.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A System.ComponentModel.PropertyChangingEventArgs that contains the event data.</param>
    public delegate void PropertyChangingEventHandler(object sender, PropertyChangingEventArgs e);

    /// <summary>
    /// Provides data for the System.ComponentModel.INotifyPropertyChanging.PropertyChanging event.
    /// </summary>
    public class PropertyChangingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the System.ComponentModel.PropertyChangingEventArgs class.
        /// </summary>
        /// <param name="propertyName">The name of the property whose value is changing.</param>
        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public PropertyChangingEventArgs(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        /// <summary>
        /// Gets the name of the property whose value is changing.
        /// Returns: The name of the property whose value is changing.
        /// </summary>
        public virtual string PropertyName { get; private set; }
    }
}
