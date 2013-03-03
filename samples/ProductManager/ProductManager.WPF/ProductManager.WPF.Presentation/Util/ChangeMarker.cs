using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;
using System.Globalization;
using ProductManager.Common.Domain.Model;
using NTier.Common.Domain.Model;

namespace ProductManager.WPF.Presentation.Util
{
    public class ChangeMarker
    {
        #region Entity
        // Using a DependencyProperty as the backing store for Info.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EntityProperty = 
            DependencyProperty.RegisterAttached(
                "Entity",
                typeof(Entity),
                typeof(ChangeMarker),
                new FrameworkPropertyMetadata(null, 
                    //FrameworkPropertyMetadataOptions.AffectsRender, 
                    OnEntityRegistered));


        private static void OnEntityRegistered(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            //if (GetTriggerVisibility(sender))
            {
                Entity entity = args.OldValue as Entity;
                if (entity != null)
                {
                    var watcher = sender.GetValue(WatcherProperty) as Watcher;
                    if (watcher != null)
                    {
                        watcher.Dispose();
                    }
                    sender.ClearValue(WatcherProperty);
                }

                entity = args.NewValue as Entity;
                if (entity != null)
                {
                    var propertyName = GetPropertyName(sender);
                    //if (propertyName != null)
                    {
                        sender.SetValue(WatcherProperty, new Watcher(sender, entity, propertyName));
                    }

                    //if (propertyName != null && entity.ChangeTracker.ModifiedProperties.Contains(propertyName))
                    //{
                    //    sender.SetValue(UIElement.VisibilityProperty, Visibility.Visible);
                    //}
                    //else
                    //{
                    //    sender.SetValue(UIElement.VisibilityProperty, Visibility.Collapsed);
                    //}

                    //Binding binding = new Binding("Visibility") { Source = entity, Mode = BindingMode.OneWay };
                    //BindingOperations.SetBinding(sender, UIElement.VisibilityProperty, binding);
                }
            }
        }

        public static Entity GetEntity(DependencyObject obj)
        {
            return (Entity)obj.GetValue(EntityProperty);
        }

        public static void SetEntity(DependencyObject obj, Entity value)
        {
            if (EntityProperty != null) // needed because of Blend
            {
                obj.SetValue(EntityProperty, value);
            }
        }
        #endregion

        #region TriggerVisibility
        //// Using a DependencyProperty as the backing store for Info.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty TriggerVisibilityProperty =
        //    DependencyProperty.RegisterAttached(
        //        "TriggerVisibility",
        //        typeof(bool),
        //        typeof(ChangeMarker),
        //        new FrameworkPropertyMetadata(false));

        //public static bool GetTriggerVisibility(DependencyObject obj)
        //{
        //    return (bool)obj.GetValue(PropertyNameProperty);
        //}

        //public static void SetTriggerVisibility(DependencyObject obj, bool value)
        //{
        //    if (PropertyNameProperty != null) // needed because of Blend
        //    {
        //        obj.SetValue(EntityProperty, value);
        //    }
        //}
        #endregion

        #region PropertyName
        // Using a DependencyProperty as the backing store for Info.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.RegisterAttached(
                "PropertyName",
                typeof(string),
                typeof(ChangeMarker),
                new FrameworkPropertyMetadata(null,
            //FrameworkPropertyMetadataOptions.AffectsRender, 
                    (s, e) =>
                    {

                    }));

        public static string GetPropertyName(DependencyObject obj)
        {
            return (string)obj.GetValue(PropertyNameProperty);
        }

        public static void SetPropertyName(DependencyObject obj, string value)
        {
            if (PropertyNameProperty != null) // needed because of Blend
            {
                obj.SetValue(PropertyNameProperty, value);
            }
        }
        #endregion

        #region Watcher
        private class Watcher : INotifyPropertyChanged, IDisposable
        {
            private readonly DependencyObject _dependencyObject;
            private readonly Entity _entity;
            private readonly string _propertyName;

            public Watcher(DependencyObject dependencyObject, Entity entity, string propertyName)
            {
                _dependencyObject = dependencyObject;
                _entity = entity;
                _propertyName = propertyName;

                _entity.PropertyChanged += entity_PropertyChanged;

                Binding binding = new Binding("Visibility") { Source = this, Mode = BindingMode.OneWay };
                BindingOperations.SetBinding(_dependencyObject, UIElement.VisibilityProperty, binding);
            }

            private void entity_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (PropertyChanged != null && (e.PropertyName == _propertyName || string.IsNullOrEmpty(_propertyName)))
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Visibility"));
                }
            }

            public Visibility Visibility
            {
                get
                {
                    if (string.IsNullOrEmpty(_propertyName))
                    {
                        return _entity.HasChanges ? Visibility.Visible : Visibility.Collapsed;
                    }
                    else
                    {
                        return //_entity.ChangeTracker.ModifiedProperties.Contains(_propertyName) ||
                               _entity.ChangeTracker.OriginalValues.ContainsKey(_propertyName) ?
                               Visibility.Visible : Visibility.Collapsed;
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public void Dispose()
            {
                if (_entity == null) return;
                _entity.PropertyChanged -= entity_PropertyChanged;
            }
        }

        // Using a DependencyProperty as the backing store for Info.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty WatcherProperty =
            DependencyProperty.RegisterAttached(
                "Watcher",
                typeof(Watcher),
                typeof(ChangeMarker),
                new FrameworkPropertyMetadata(null));
        #endregion
    }
}
