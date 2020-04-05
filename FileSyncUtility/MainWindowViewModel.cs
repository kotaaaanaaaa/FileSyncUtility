using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;

namespace FileSyncUtility
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            SettingCommand = new DelegateCommand(Setting);
            AddSynchronizeItemCommand = new DelegateCommand(AddSynchronizeItem);
            RemoveSynchronizeItemCommand = new DelegateCommand(RemoveSynchronizeItem);
        }

        public ObservableCollection<SynchronizeItem> SynchronizeItems
        {
            get => _synchronizeItems;
            set => SetProperty(ref _synchronizeItems, value);
        }

        private ObservableCollection<SynchronizeItem> _synchronizeItems;

        private void Setting()
        {
        }

        public DelegateCommand SettingCommand { get; set; }

        private void AddSynchronizeItem()
        {
        }

        public DelegateCommand AddSynchronizeItemCommand { get; set; }

        private void RemoveSynchronizeItem()
        {
        }

        public DelegateCommand RemoveSynchronizeItemCommand { get; set; }
    }

    public class MainWindowViewModelDesigner : MainWindowViewModel
    {
        public MainWindowViewModelDesigner()
        {
            SynchronizeItems = new ObservableCollection<SynchronizeItem>
            {
                new SynchronizeItem
                {
                    SourcePath = @"C:\Users\Administrator\Desktop",
                    DestinationPath = @"D:\Backup\Administrator\Desktop",
                },
                new SynchronizeItem
                {
                    SourcePath = @"C:\Users\Guest\Desktop",
                    DestinationPath = @"D:\Backup\Guest\Desktop",
                },
            };
        }
    }
}
