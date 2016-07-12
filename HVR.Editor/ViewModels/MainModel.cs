using HVR.Common.Enums;
using HVR.Common.Objects.Editor;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace HVR.Editor.ViewModels {
    public class MainModel : INotifyPropertyChanged {
        private ObservableCollection<MapObjectsContainerItem> _mapObjects;

        public ObservableCollection<MapObjectsContainerItem> MapObjects {
            get {
                if (_mapObjects == null) {
                    return new ObservableCollection<MapObjectsContainerItem>();
                }

                return new ObservableCollection<MapObjectsContainerItem>(_mapObjects.Where(a => a.MapObjectType == SelectedMapObjectType));
            }

            set { _mapObjects = value;  OnPropertyChanged(); }
        }

        private MapObjectTypes _selectedMapObjectType;

        public MapObjectTypes SelectedMapObjectType {
            get { return _selectedMapObjectType; }
            set { _selectedMapObjectType = value;  OnPropertyChanged(); OnPropertyChanged("MapObjects"); }
        }

        public IEnumerable<MapObjectTypes> SelectableMapObjectTypes {
            get {
                return Enum.GetValues(typeof(MapObjectTypes)).Cast<MapObjectTypes>();
            }
        }

        public void LoadModel() {
            SelectedMapObjectType = MapObjectTypes.PLAYER_START;

            if (!File.Exists("mapobjects.json")) {
                return;
            }

            var fileStr = File.ReadAllText("mapobjects.json");

            MapObjects = new ObservableCollection<MapObjectsContainerItem>(JsonConvert.DeserializeObject<List<MapObjectsContainerItem>>(fileStr));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}