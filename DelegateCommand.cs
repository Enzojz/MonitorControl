using System.Windows.Input;

namespace MonitorControl
{
    public class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> execute)
        {
            this.m_execute = execute;
            this.m_canExecute = (object p) => true;
        }
        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this.m_execute = execute;
            this.m_canExecute = canExecute;
        }

        public bool CanExecute(object parameter) { return this.m_canExecute(parameter); }
        public void Execute(object parameter) { this.m_execute(parameter); }

        private Action<object> m_execute;

        private Func<object, bool> m_canExecute;
    }
}
