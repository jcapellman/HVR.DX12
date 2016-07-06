using System;
using System.Windows.Controls;
using System.Windows.Input;

using HVR.ViewModels;

namespace HVR.UserControls {
    /// <summary>
    /// Interaction logic for ConsoleControl.xaml
    /// </summary>
    public partial class ConsoleControl : UserControl {
        public event EventHandler OnHide;

        private ConsoleViewModel viewModel => (ConsoleViewModel)DataContext;

        public ConsoleControl() {
            InitializeComponent();

            DataContext = new ConsoleViewModel();
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.OemTilde) {
                OnHide(this, null);
                return;
            }

            if (e.Key == Key.Return) {
                viewModel.AddEntry();
                return;
            }
        }
    }
}
