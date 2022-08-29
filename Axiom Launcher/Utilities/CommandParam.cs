using System;
using System.Windows.Input;

namespace Axiom
{
    public class CommandParam<Parameter> : ICommand 
    {
        readonly Action<Parameter> _execute;

        public CommandParam(Action<Parameter> execute) {
            if (execute == null)
                return;

            this._execute = execute;
        }

        public void Execute(object parameter) {
            if (parameter is Parameter p)
                this._execute?.Invoke(p);
        }

        public bool CanExecute(object parameter) {
            return true;
        }

        public event EventHandler CanExecuteChanged {
            add { }
            remove { }
        }
    }
}