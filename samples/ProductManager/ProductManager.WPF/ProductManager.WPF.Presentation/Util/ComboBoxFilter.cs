using System.ComponentModel;  
using System.Windows;  
using System.Windows.Controls;  
using System.Windows.Input;  
using System.Windows.Media;  
using System.Windows.Documents;
using System.Windows.Data;

namespace ProductManager.WPF.Presentation.Util
{
    public class ComboBoxFilter
    {
        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...  
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(ComboBoxFilter),
                new UIPropertyMetadata(
                    null,
                    (o, e) =>
                    {
                        var comboBox = o as ComboBox;
                        if (comboBox != null)
                        {
                            if (e.OldValue != null && e.NewValue == null)
                            {
                                comboBox.SelectionChanged -= new SelectionChangedEventHandler(comboBox_SelectionChanged);
                            }
                            if (e.OldValue == null && e.NewValue != null)
                            {
                                comboBox.SelectionChanged += new SelectionChangedEventHandler(comboBox_SelectionChanged);
                            }
                        }
                    }
                )
            );

        private static void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            ICommand command = GetCommand(comboBox);
            if (command != null)
            {
                if (command.CanExecute(comboBox.SelectedValue))
                {
                    command.Execute(comboBox.SelectedValue);
                }
            }
        }
    }
}