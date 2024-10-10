// Helpers/RelayCommand.cs
using System;
using System.Windows.Input;

namespace StockTradingApp.Helpers
{
    /// <summary>
    /// Defines the RelayCommand class, which implements the ICommand interface. 
    /// This class allows you to pass custom logic to UI elements like buttons, 
    /// connecting actions (like button clicks) with the business logic defined 
    /// in the ViewModel.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object parameter) => _execute(parameter);

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
