using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BlogWriter.Wpf.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;
        private readonly bool _setBusyCursor;

        public RelayCommand(Action execute, bool setBusyCursor = true)
            : this(execute, null, setBusyCursor)
        {
        }

        public RelayCommand(Action<object> execute, bool setBusyCursor = true)
            : this(execute, null, setBusyCursor)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute, bool setBusyCursor = true)
            : this((object x) => execute(), canExecute == null ? default(Func<object, bool>) : (object x) => canExecute(), setBusyCursor)
        {
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute, bool setBusyCursor = true)
        {
            if (execute == null) throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
            _setBusyCursor = setBusyCursor;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            if (_setBusyCursor) BusyCursor.Start();
            try
            {
                _execute(parameter);
            }
            finally
            {
                if (_setBusyCursor) BusyCursor.Stop();
            }
        }
    }
}
