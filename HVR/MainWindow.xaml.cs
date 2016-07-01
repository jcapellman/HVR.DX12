using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpDX.Windows;

namespace HVR {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            var form = new RenderForm("HVR") {
                Width = 1280,
                Height = 720,
                ShowIcon = false
            };

            form.Show();

            using (var app = new MainRenderWindow()) {
                app.Initialize(form);

                using (var loop = new RenderLoop(form)) {
                    while (loop.NextFrame()) {
                        app.Update();
                        app.Render();
                    }
                }
            }

        }
    }
}
