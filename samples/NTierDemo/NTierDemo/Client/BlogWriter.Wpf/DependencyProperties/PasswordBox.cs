using BlogWriter.Wpf.Extensions;
using System.Security;
using System.Windows;

namespace BlogWriter.Wpf.DependencyProperties
{
    public static class PasswordBox
    {
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached(
            "Password",
            typeof(SecureString),
            typeof(PasswordBox),
            new FrameworkPropertyMetadata(new SecureString(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPasswordPropertyChanged));

        private static readonly DependencyProperty IsUpdatingProperty = DependencyProperty.RegisterAttached(
            "IsUpdating", 
            typeof(bool),
            typeof(PasswordBox));

        private static void OnPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = d as System.Windows.Controls.PasswordBox;
            if (passwordBox == null) return;

            if ((bool)passwordBox.GetValue(IsUpdatingProperty)) return;

            passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;

            var password = e.NewValue as SecureString;
            if (password == null)
                passwordBox.Clear();
            else
                passwordBox.Password = password.ToUnsecureString(); // unfortunately PasswordBox won't let us set the SecureString

            passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as System.Windows.Controls.PasswordBox;
            if (passwordBox == null) return;

            passwordBox.SetValue(IsUpdatingProperty, true);
            passwordBox.SetValue(PasswordProperty, passwordBox.SecurePassword);
            passwordBox.SetValue(IsUpdatingProperty, false);
        }

        public static void SetPassword(this System.Windows.Controls.PasswordBox control, SecureString value)
        {
            control.SetValue(PasswordProperty, value);
        }

        public static SecureString GetPassword(this System.Windows.Controls.PasswordBox control)
        {
            return (SecureString)control.GetValue(PasswordProperty);
        }
    }
}
