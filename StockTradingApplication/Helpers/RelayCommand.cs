using System.Windows.Input;

namespace StockTradingApplication.Helpers
{
    /// <summary>
    /// Defines the RelayCommand class, which implements the ICommand interface. 
    /// This class allows you to pass custom logic to UI elements like buttons, 
    /// connecting actions (like button clicks) with the business logic defined 
    /// in the ViewModel.
    /// </summary>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;
        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => _canExecute?.Invoke((T)parameter) ?? true;
        public void Execute(object parameter) => _execute((T)parameter);
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
