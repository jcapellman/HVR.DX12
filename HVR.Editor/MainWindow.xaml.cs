using HVR.Editor.ViewModels;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace HVR.Editor {
    public partial class MainWindow : Window {
        private MainModel viewModel => (MainModel)DataContext;

        public MainWindow() {
            InitializeComponent();

            DataContext = new MainModel();

            viewModel.LoadModel();

            foreach (var tile in viewModel.Level.Geometry) {
                var item = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + tile.TextureName, UriKind.RelativeOrAbsolute));

                var img = new Image();
                img.Source = item;
                img.Margin = new Thickness(0, 0, 0, 2);
                img.Stretch = System.Windows.Media.Stretch.UniformToFill;

                ugMain.Children.Add(img);
            }
        }

        private void MiExit_OnClick(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }
    }
}