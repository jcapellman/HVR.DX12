using System.Linq;

using SharpDX.DirectInput;

namespace HVR.Helpers {
    public class InputHandler {
        private Keyboard _keyboard;

        public InputHandler() {
            var directInput = new DirectInput();

            var devices = directInput.GetDevices();

            var deviceKeyboard = devices.Where(a => a.Type == DeviceType.Keyboard).FirstOrDefault();

            _keyboard = new Keyboard(directInput);

            _keyboard.Properties.BufferSize = 128;
            _keyboard.Acquire();
        }

        public KeyboardUpdate[] CheckInput() {
            _keyboard.Poll();

            return _keyboard.GetBufferedData();
        }
    }
}