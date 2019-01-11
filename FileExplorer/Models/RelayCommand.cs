using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
namespace FileExplorer.Models
{
    public class RelayCommand : ICommand,INotifyPropertyChanged
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;
        private string name;
        private bool isExecutable;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public string Name { get => name; private set => name = value; }

        public bool IsExecutable
        {
            get => isExecutable;
            set
            {
                isExecutable = value; OnPropertyChanged(nameof(IsExecutable));
            }
        }

        public RelayCommand(string name, Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
            this.name = name;
        }

        public bool CanExecute(object parameter)
        {
            IsExecutable = this.canExecute(parameter);
            return canExecute == null || IsExecutable;
        }
        public void Execute(object parameter)
        {
            execute(parameter);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

    }
}
