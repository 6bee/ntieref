using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BlogWriter.Wpf.Commands
{
    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<object,Task> _execute;
        private readonly Func<object, bool> _canExecute;
        private readonly bool _setBusyCursor;

        public AsyncRelayCommand(Func<Task> execute, bool setBusyCursor = true)
            : this(execute, null, setBusyCursor)
        {
        }

        public AsyncRelayCommand(Func<object, Task> execute, bool setBusyCursor = true)
            : this(execute, null, setBusyCursor)
        {
        }

        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute, bool setBusyCursor = true)
            : this(execute == null ? default(Func<object, Task>) : (object x) => execute(), canExecute == null ? default(Func<object, bool>) : (object x) => canExecute(), setBusyCursor)
        {
        }

        public AsyncRelayCommand(Func<object, Task> execute, Func<object, bool> canExecute, bool setBusyCursor = true)
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

        public async void Execute(object parameter)
        {
            if (_setBusyCursor) BusyCursor.Start();
            try
            {
                await _execute(parameter);
                CommandManager.InvalidateRequerySuggested();
            }
            finally
            {
                if (_setBusyCursor) BusyCursor.Stop();
            }
        }
    }
}
