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
            Adapters = new ObservableCollection<AdapterListingItem>();
        }

        private ObservableCollection<AdapterListingItem> _adapters;

        public ObservableCollection<AdapterListingItem> Adapters {
            get { return _adapters; }
            set { _adapters = value; OnPropertyChanged(); }
        }

        private AdapterListingItem _selectedAdapter;

        public AdapterListingItem SelectedAdapter {
            get { return _selectedAdapter; }
            set { _selectedAdapter = value; OnPropertyChanged(); updateSupportedResolutions(); }
        }

        private void updateSupportedResolutions() {
            var adapterOutput = SelectedAdapter.DXAdapter.Outputs.FirstOrDefault();

            var displayModes = adapterOutput.GetDisplayModeList(SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb, SharpDX.DXGI.DisplayModeEnumerationFlags.Scaling);

            ScreenResolutions.Clear();
            SelectedScreenResolution = null;

            foreach (var displayMode in displayModes.Where(a => a.Scaling == SharpDX.DXGI.DisplayModeScaling.Unspecified)) {
                var displayStr = $"{displayMode.Width}x{displayMode.Height}";

                if (ScreenResolutions.Any(a => a.Display == displayStr)) {
                    continue;
                }

                ScreenResolutions.Add(new ScreenResolutionListingItem {
                    Display = displayStr,
                    Width = displayMode.Width,
                    Height = displayMode.Height
                });
            }

            SelectedScreenResolution = ScreenResolutions.FirstOrDefault();
        }

        public void LoadData() {
            var factory = new SharpDX.DXGI.Factory1();

            foreach (var adapter in factory.Adapters.Where(a => a.Outputs.Count() > 0)) {
                Adapters.Add(new AdapterListingItem {
                    DXAdapter = adapter
                });
            }

            SelectedAdapter = Adapters.FirstOrDefault();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}