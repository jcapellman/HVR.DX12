using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HVR.ViewModels {
    public class ConsoleViewModel : INotifyPropertyChanged {
        private ObservableCollection<string> _consoleLog;

        public ObservableCollection<string> ConsoleLog {
            get { return _consoleLog; }
            set { _consoleLog = value; OnPropertyChanged(); }
        }

        public ConsoleViewModel() {
            ConsoleLog = new ObservableCollection<string>();
        }

        private string _entry;

        public string Entry {
            get { return _entry; }
            set { _entry = value; OnPropertyChanged(); }
        }

        public class CommandAction {
            public string Action { get; set; }

            public string Value { get; set; }
        }

        public CommandAction AddEntry() {
            ConsoleLog.Add(Entry);

            var commands = Entry.Split(' ');

            var action = new CommandAction();

            if (commands.Length == 1) {
                action.Action = Entry.ToUpper();

                Entry = string.Empty;
                return action;
            }

            action = new CommandAction {
                Action = commands[0].ToUpper(),
                Value = commands[1]
            };
            
            Entry = string.Empty;

            return action;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
