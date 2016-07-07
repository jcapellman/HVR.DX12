using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using HVR.Common.Objects.Launcher;

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

        internal void ApplyAction(ConsoleViewModel.CommandAction cAction) {
            
        }

        private bool _isFullscreen;

        public bool IsFullscreen {  get { return _isFullscreen; } set { _isFullscreen = value;  OnPropertyChanged(); } }

        public MainWindowViewModel() {
            ScreenResolutions = new ObservableCollection<ScreenResolutionListingItem>();
            Adapters = new ObservableCollection<AdapterListingItem>();
            MutliSamplingValues = new ObservableCollection<int>();
        }

        public MainLoopTransportItem GetMainLoopTransportItem() => new MainLoopTransportItem {
            CfgHelper = App.CfgHelper,
            DXAdapter = SelectedAdapter.DXAdapter,
            Height = SelectedScreenResolution.Height,
            Width = SelectedScreenResolution.Width,
            IsFullScreen = IsFullscreen,
            Level = Common.Helpers.LevelLoaderHelper.GetLevel("e1m1.lvl")
        };

        private ObservableCollection<AdapterListingItem> _adapters;

        public ObservableCollection<AdapterListingItem> Adapters {
            get { return _adapters; }
            set { _adapters = value; OnPropertyChanged(); }
        }

        private bool _enableFPSCounter;

        public bool EnableFPSCounter {
            get { return _enableFPSCounter; }
            set { _enableFPSCounter = value;  OnPropertyChanged(); }
        }

        private AdapterListingItem _selectedAdapter;

        private ObservableCollection<int> _multiSamplingValues;

        public ObservableCollection<int> MutliSamplingValues {
            get { return _multiSamplingValues; }
            set { _multiSamplingValues = value;  OnPropertyChanged(); }
        }

        private int _selectedMultiSample;

        public int SelectedMultiSample {
            get { return _selectedMultiSample; }
            set { _selectedMultiSample = value;  OnPropertyChanged(); }
        }

        public AdapterListingItem SelectedAdapter {
            get { return _selectedAdapter; }
            set { _selectedAdapter = value; OnPropertyChanged(); updateSupportedResolutions(); }
        }

        public void SaveConfig() {
            App.CfgHelper.SetConfigOption(Common.Enums.ConfigOptions.SELECTED_ADAPTER, SelectedAdapter.Display);
            App.CfgHelper.SetConfigOption(Common.Enums.ConfigOptions.SELECTED_FULLSCREEN, IsFullscreen);
            App.CfgHelper.SetConfigOption(Common.Enums.ConfigOptions.SELECTED_RESOLUTION, SelectedScreenResolution.Display);
            App.CfgHelper.SetConfigOption(Common.Enums.ConfigOptions.SELECTED_MULTISAMPLE_VALUE, SelectedMultiSample);
            App.CfgHelper.SetConfigOption(Common.Enums.ConfigOptions.ENABLE_FPS_COUNTER, EnableFPSCounter);

            App.CfgHelper.WriteConfig();
        }

        private void updateSupportedResolutions() {
            var adapterOutput = SelectedAdapter.DXAdapter.Outputs.FirstOrDefault();
            
            var displayModes = adapterOutput.GetDisplayModeList(SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb, SharpDX.DXGI.DisplayModeEnumerationFlags.Scaling);

            ScreenResolutions.Clear();
            SelectedScreenResolution = null;

            foreach (var displayMode in displayModes.Where(a => a.Scaling == SharpDX.DXGI.DisplayModeScaling.Unspecified && a.Width >= 1280)) {
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

            var cfgSelectedScreenResolution = App.CfgHelper.GetConfigOption(Common.Enums.ConfigOptions.SELECTED_RESOLUTION);

            if (!ScreenResolutions.Any(a => a.Display == cfgSelectedScreenResolution)) {
                SelectedScreenResolution = ScreenResolutions.FirstOrDefault();
            }

            SelectedScreenResolution = ScreenResolutions.FirstOrDefault(a => a.Display == cfgSelectedScreenResolution);
        }

        public void LoadData() {
            IsFullscreen = App.CfgHelper.GetConfigOption(Common.Enums.ConfigOptions.SELECTED_FULLSCREEN);
            EnableFPSCounter = App.CfgHelper.GetConfigOption(Common.Enums.ConfigOptions.ENABLE_FPS_COUNTER);
            SelectedMultiSample = App.CfgHelper.GetConfigOption(Common.Enums.ConfigOptions.SELECTED_MULTISAMPLE_VALUE);

            MutliSamplingValues.Add(1);
            MutliSamplingValues.Add(2);
            MutliSamplingValues.Add(4);
            MutliSamplingValues.Add(8);
            MutliSamplingValues.Add(16);

            var factory = new SharpDX.DXGI.Factory1();

            foreach (var adapter in factory.Adapters.Where(a => a.Outputs.Count() > 0)) {
                Adapters.Add(new AdapterListingItem {
                    DXAdapter = adapter
                });
            }

            var cfgSelectedAdapter = App.CfgHelper.GetConfigOption(Common.Enums.ConfigOptions.SELECTED_ADAPTER);

            if (Adapters.Any(a => a.Display != cfgSelectedAdapter) || Adapters.FirstOrDefault().Display == cfgSelectedAdapter) {
                SelectedAdapter = Adapters.FirstOrDefault();
                return;
            }

            SelectedAdapter = Adapters.FirstOrDefault(a => a.Display == cfgSelectedAdapter);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}