using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCTradeTracker.API
{
    /// <summary>
    /// Extracted from <see cref="https://stackoverflow.com/questions/2450373/set-global-hotkeys-using-c-sharp"/>
    /// </summary>
    public sealed class Hotkey : IDisposable
    {
        // Registers a hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        // Unregisters the hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// Represents the window that is used internally to get the messages.
        /// </summary>
        private class Window : NativeWindow, IDisposable
        {
            private const int WM_HOTKEY = 0x0312;

            public Window()
            {
                // create the handle for the window.
                this.CreateHandle(new CreateParams());
            }

            /// <summary>
            /// Overridden to get the notifications.
            /// </summary>
            /// <param name="m"></param>
            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                // check if we got a hot key pressed.
                if (m.Msg == WM_HOTKEY)
                {
                    // get the keys.
                    Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                    ModifierKeys modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);

                    // invoke the event to notify the parent.
                    KeyPressed?.Invoke(this, new KeyPressedEventArgs(modifier, key));
                }
            }

            public event EventHandler<KeyPressedEventArgs> KeyPressed;

            #region IDisposable Members

            public void Dispose()
            {
                this.DestroyHandle();
            }

            #endregion
        }

        private readonly Window _window = new Window();
        private int _currentId;

        public Hotkey()
        {
            // register the event of the inner native window.
            _window.KeyPressed += delegate (object sender, KeyPressedEventArgs args)
            {
                KeyPressed?.Invoke(this, args);
            };
        }

        /// <summary>
        /// Registers a hot key in the system.
        /// </summary>
        /// <param name="modifier">The modifiers that are associated with the hot key.</param>
        /// <param name="key">The key itself that is associated with the hot key.</param>
        public void RegisterHotKey(HotkeyArgs args)
        {
            // increment the counter.
            _currentId += 1;

            // register the hot key.
            if (!RegisterHotKey(_window.Handle, _currentId, (uint)args.Modifier, (uint)args.Key))
                throw new InvalidOperationException("Couldn’t register the hot key.");
        }

        /// <summary>
        /// A hot key has been pressed.
        /// </summary>
        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        #region IDisposable Members

        public void Dispose()
        {
            // unregister all the registered hot keys.
            for (int i = _currentId; i > 0; i--)
            {
                UnregisterHotKey(_window.Handle, i);
            }

            // dispose the inner native window.
            _window.Dispose();
        }

        #endregion
    }

    /// <summary>
    /// Event Args for the event that is fired after the hot key has been pressed.
    /// </summary>
    public class KeyPressedEventArgs : EventArgs
    {
        internal KeyPressedEventArgs(ModifierKeys modifier, Keys key)
        {
            Modifier = modifier;
            Key = key;
        }

        public ModifierKeys Modifier { get; }

        public Keys Key { get; }

        public HotkeyArgs HotkeyArgs
        {
            get { return new HotkeyArgs(Modifier, Key); }
        }
            
    }

    public class HotkeyArgs
    {
        public ModifierKeys Modifier { get; }
        public Keys Key { get; }

        public HotkeyArgs(ModifierKeys modifier, Keys key)
        {
            Modifier = modifier;
            Key = key;
        }

        public override bool Equals(object obj)
        {
            var item = obj as HotkeyArgs;
            if (item == null) return false;
            return item.Modifier.Equals(Modifier) && item.Key.Equals(Key);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + Modifier.GetHashCode();
            hash = hash * 31 + Key.GetHashCode();
            return hash;
        }

        public static bool operator ==(HotkeyArgs obj1, HotkeyArgs obj2) => obj1.Equals(obj2);
        public static bool operator !=(HotkeyArgs obj1, HotkeyArgs obj2) => !obj1.Equals(obj2);
    }

    /// <summary>
    /// The enumeration of possible modifiers.
    /// </summary>
    [Flags]
    public enum ModifierKeys : uint
    {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8
    }
}
