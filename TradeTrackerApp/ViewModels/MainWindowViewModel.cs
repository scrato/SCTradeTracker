using Prism.Mvvm;

namespace SCTradeTracker.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Star Citizen - Trade Console Watcher";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {

        }
    }
}
