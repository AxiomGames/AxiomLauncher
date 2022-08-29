using System;
using System.Windows.Input;

namespace Axiom 
{
    public class Command : ICommand 
    {
        private readonly Action _action;

        public Command(Action action) { this._action = action; }

        public void Execute(object parameter) { this._action?.Invoke(); }

        public bool CanExecute(object parameter) { return true; }

        public event EventHandler CanExecuteChanged { add { } remove { } }
    }
}