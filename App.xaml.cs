using SCTradeTracker.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using SCTradeTracker.API;

namespace SCTradeTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private Hotkey hotkey;

        const ModifierKeys printModifier = ModifierKeys.Control | ModifierKeys.Alt;

        public override void Initialize()
        {
            base.Initialize();
            hotkey = new Hotkey();
            hotkey.RegisterHotKey(, System.Windows.Forms.Keys.Print);
            hotkey.KeyPressed += Hotkey_KeyPressed;
        }

        private void Hotkey_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if(e.)
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
