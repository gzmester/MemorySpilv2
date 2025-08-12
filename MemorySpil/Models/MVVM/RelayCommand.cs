using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MemorySpil.Models.MVVM
{
    class RelayCommand : ICommand
    {
        private Action<object> exceute;
        private Func<object, bool> canExecute;

        public RelayCommand(Action<object> exceute, Func<object, bool> canExecute>)
        {
            this.exceute = exceute;
            this.canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove {  CommandManager.RequerySuggested -= value;}
        }

        public bool CanExecute(object? parameter)
        {
            return canExecute != null || canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            Execute(parameter);
        }
    }
}
