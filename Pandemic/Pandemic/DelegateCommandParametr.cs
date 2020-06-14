using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pandemic
{
    class DelegateCommandParametr : ICommand
    {
        private Func<object, bool> canExecute;
        private Func<object, object> execute;

        
        public DelegateCommandParametr(Func<object, object> execute, Func<object, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
        
        public DelegateCommandParametr(Func<object, object> execute)
        {
            this.execute = execute;
            canExecute = new Func<object, bool>((parameter) => { return true; });
        }


        public bool CanExecute(object parameter)
        {
            return canExecute(parameter);
        }
        public void Execute(object parameter) => execute(parameter);


        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
