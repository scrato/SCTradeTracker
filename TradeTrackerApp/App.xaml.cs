using SCTradeTracker.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using SCTradeTracker.API;
using System.Drawing;

namespace SCTradeTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private Hotkey hotkey;

        static HotkeyArgs PrintScreenHotkey { get; } = new HotkeyArgs(ModifierKeys.Alt, System.Windows.Forms.Keys.Print);
        public static Screenshot Screenshot {get;} = new Screenshot();

        public override void Initialize()
        {
            base.Initialize();
            hotkey = new Hotkey();
            hotkey.RegisterHotKey(PrintScreenHotkey);
            hotkey.KeyPressed += Hotkey_KeyPressed;
        }

        private void Hotkey_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if(e.HotkeyArgs == PrintScreenHotkey)
            {
                Bitmap p = Screenshot.ActiveWindow();
            }
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }

    }
}
