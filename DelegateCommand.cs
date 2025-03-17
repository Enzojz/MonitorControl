using System.Windows.Input;

namespace MonitorControl
{
    public class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> execute)
        {
            this.m_execute = execute;
        }

        public bool CanExecute(object parameter) { return true; }
        public void Execute(object parameter) { this.m_execute(parameter); }

        private Action<object> m_execute;
    }
}
