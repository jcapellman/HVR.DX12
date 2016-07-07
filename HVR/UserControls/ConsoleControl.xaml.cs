using System;
using System.Windows.Controls;
using System.Windows.Input;

using HVR.ViewModels;

using static HVR.ViewModels.ConsoleViewModel;

namespace HVR.UserControls {
    public partial class ConsoleControl : UserControl {
        public event EventHandler OnHide;

        public event EventHandler OnAction;

        private ConsoleViewModel viewModel => (ConsoleViewModel)DataContext;

        public CommandAction CAction;

        public ConsoleControl() {
            InitializeComponent();

            DataContext = new ConsoleViewModel();

            tbMain.Focus();
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                CAction = viewModel.AddEntry();

                OnAction(this, EventArgs.Empty);
                return;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.OemTilde) {
                OnHide(this, null);
                return;
            }
        }
    }
}