using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace CommonLibs.WpfLibrary.Commands
{
    public class DelegateCommand : ICommand
    {
        private readonly Func<bool> m_CanExecute;
        private readonly Action m_Execute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action execute) : this(execute, null)
        {
        }

        public DelegateCommand(Action execute, Func<bool> canExecute)
        {
            m_Execute = execute;
            m_CanExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (m_CanExecute == null)
                return true;

            return m_CanExecute();
        }

        public void Execute(object parameter)
        {
            m_Execute();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class DelegateCommand<T> : ICommand
    {
        private readonly Predicate<T> m_CanExecute;
        private readonly Action<T> m_Execute;

        public event EventHandler CanExecuteChanged;



        public DelegateCommand(Action<T> execute) : this(execute, null)
        {
        }


        public DelegateCommand(Action<T> execute, Predicate<T> canExecute)
        {
            m_Execute = execute;
            m_CanExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (m_CanExecute == null)
                return true;

            var castedParameter = (T)parameter;

            return m_CanExecute(castedParameter);
        }

        public void Execute(object parameter)
        {
            var castedParameter = (T)parameter;

            m_Execute(castedParameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
