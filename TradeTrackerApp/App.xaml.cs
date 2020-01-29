using SCTradeTracker.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using SCTradeTracker.API;
using System.Drawing;
using SCTradeTracker.ComputerVision;
using SCTradeTracker.Business;
using System.Windows.Interop;
using NHotkey.Wpf;
using System.Windows.Input;
using NHotkey;
using System;

namespace SCTradeTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private const string c_PrintKey = "PrintKey";
        public static CommodityAnalyzer Analyzer { get; } = new CommodityAnalyzer();

        public override void Initialize()
        {
            base.Initialize();
            HotkeyManager.Current.AddOrReplace(c_PrintKey, Key.I, ModifierKeys.Control | ModifierKeys.Alt, InvokePrintHotkey);
        }

        private async void InvokePrintHotkey(object sender, HotkeyEventArgs e)
        {
            await Analyzer.AnalyseAsync();
            e.Handled = true;
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
