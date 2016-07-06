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

        public void AddEntry() {
            ConsoleLog.Add(Entry);

            Entry = string.Empty;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
