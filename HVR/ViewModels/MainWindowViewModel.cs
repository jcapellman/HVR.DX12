using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using HVR.Objects.Launcher;

namespace HVR.ViewModels {
    public class MainWindowViewModel : INotifyPropertyChanged {
        private ObservableCollection<ScreenResolutionListingItem> _screenResolutions;

        public ObservableCollection<ScreenResolutionListingItem> ScreenResolutions {
            get { return _screenResolutions; }

            set { _screenResolutions = value;  OnPropertyChanged(); }
        }

        private ScreenResolutionListingItem _selectedScreenResolution;

        public ScreenResolutionListingItem SelectedScreenResolution {
            get { return _selectedScreenResolution; }
            set { _selectedScreenResolution = value;  OnPropertyChanged(); }
        }

        private bool _isFullscreen;

        public bool IsFullscreen {  get { return _isFullscreen; } set { _isFullscreen = value;  OnPropertyChanged(); } }

        public MainWindowViewModel() {
            ScreenResolutions = new ObservableCollection<ScreenResolutionListingItem>();
        }

        public void LoadData() {
            ScreenResolutions.Add(new ScreenResolutionListingItem {
                Width = 1280,
                Height = 720,
                Display = "1280x720"
            });

            ScreenResolutions.Add(new ScreenResolutionListingItem {
                Width = 1920,
                Height = 1080,
                Display = "1920x1080"
            });

            SelectedScreenResolution = ScreenResolutions.FirstOrDefault();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}