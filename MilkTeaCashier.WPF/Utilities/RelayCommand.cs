using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MilkTeaCashier.WPF.Utilities
{
    /// <summary>
    /// A flexible and reusable ICommand implementation that supports both 
    /// synchronous and asynchronous execution, dynamic CanExecute validation, 
    /// and centralized exception handling for WPF applications.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Func<object, Task> _executeAsync;
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;
        private readonly Action<Exception> _onException;

        public RelayCommand
        (
            Action<object> execute,
            Func<object, bool> canExecute = null,
            Action<Exception> onException = null
        )
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
            _onException = onException;
        }

        public RelayCommand(
            Func<object, Task> executeAsync,
            Func<object, bool> canExecute = null,
            Action<Exception> onException = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute;
            _onException = onException;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public async void Execute(object parameter)
        {
            try
            {
                if (_execute != null)
                {
                    _execute(parameter);
                }
                else if (_executeAsync != null)
                {
                    await _executeAsync(parameter);
                }
            }
            catch (Exception ex)
            {
                _onException?.Invoke(ex);
            }
        }
    }
}
