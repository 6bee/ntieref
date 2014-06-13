using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Threading;

namespace BlogWriter.Wpf.ViewModels
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsActive
        {
            get { return _isActive; }
            internal protected set { _isActive = value; OnPropertyChanged(() => IsActive); }
        }
        private bool _isActive;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                Invoke(() => propertyChanged(this, new PropertyChangedEventArgs(propertyName)));
            }
        }

        protected void OnPropertyChanged<T>(Expression<Func<T>> property)
        {
            var propertyName = ((MemberExpression)property.Body).Member.Name;
            OnPropertyChanged(propertyName);
        }

        protected void Invoke(Action action)
        {
            var dispatcher = Dispatcher;
            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                dispatcher.BeginInvoke(action);
            }
            else
            {
                action();
            }
        }

        public static Dispatcher Dispatcher
        {
            get { return Dispatcher.CurrentDispatcher; }
        }
    }
}
