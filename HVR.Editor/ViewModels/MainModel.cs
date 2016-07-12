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
                var files = Directory.GetFiles("mbs1/Textures", "*.png", SearchOption.AllDirectories);

                var objects = new List<MapObjectsContainerItem>();

                var item = new MapObjectsContainerItem {
                    Description = "Player Start",
                    MapGeometryType = MapGeometryTypes.NONE,
                    MapObjectType = MapObjectTypes.PLAYER_START,
                    PreviewImage = string.Empty
                };

                objects.Add(item);

                foreach (var file in files) {
                    item = new MapObjectsContainerItem {
                        Description = file,
                        MapGeometryType = file.Contains("WALL") ? MapGeometryTypes.WALL : MapGeometryTypes.FLOOR,
                        MapObjectType = MapObjectTypes.TEXTURE,
                        PreviewImage = file
                    };

                    objects.Add(item);
                }

                MapObjects = new ObservableCollection<MapObjectsContainerItem>(objects);

                var jsonStr = JsonConvert.SerializeObject(objects);

                File.WriteAllText("mapobjects.json", jsonStr);

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