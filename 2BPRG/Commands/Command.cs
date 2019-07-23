namespace _2BPRG.Commands
{
    using System;
    using System.Windows.Input;

    public class Command : ICommand
    {
        private Action<object> action;

        private Func<object, bool> canExecuteFunctor;

        public Command(Action<object> action, Func<object, bool> canExecuteFunctor = null)
        {
            this.action = action ?? throw new ArgumentNullException();
            this.canExecuteFunctor = canExecuteFunctor;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            var canCommandExecute = true;
            if (canExecuteFunctor != null)
            {
                canCommandExecute = canExecuteFunctor(parameter);
            }
            return canCommandExecute;
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                action(parameter);
            }
        }
    }
}
