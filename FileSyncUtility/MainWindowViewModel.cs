using FileSyncUtility.Common;
using FileSyncUtility.Common.Models;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FileSyncUtility
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            SettingCommand = new DelegateCommand(async () => await Setting());
            AddSynchronizeItemCommand = new DelegateCommand(async () => await AddSynchronizeItem());
            RemoveSynchronizeItemCommand = new DelegateCommand<SynchronizeItem>(async item => await RemoveSynchronizeItem(item));
            EditSynchronizeItemCommand = new DelegateCommand<SynchronizeItem>(async item => await EditSynchronizeItem(item));
        }

        public ObservableCollection<SynchronizeItem> SynchronizeItems
        {
            get => _synchronizeItems;
            set => SetProperty(ref _synchronizeItems, value);
        }

        private ObservableCollection<SynchronizeItem> _synchronizeItems = new ObservableCollection<SynchronizeItem>();

        private async Task Setting()
        {
        }
        public DelegateCommand SettingCommand { get; set; }

        private async Task AddSynchronizeItem()
        {
            var item = new SynchronizeItem();
            var vm = new EditSynchronizeItemDialogViewModel(item);
            var dialog = new EditSynchronizeItemDialog();
            dialog.DataContext = vm;
            var result = await DialogHost.Show(dialog, "DialogHost");
            await ReloadSynchronizeItems();
        }
        public DelegateCommand AddSynchronizeItemCommand { get; set; }

        private async Task RemoveSynchronizeItem(SynchronizeItem item)
        {
            var dialog = new RemoveSynchronizeItemDialog();
            var result = (bool)await DialogHost.Show(dialog, "DialogHost");
            if (result)
            {
                await using (var db = new ApplicationDbContext())
                {
                    db.Attach(item.Entity);
                    db.Remove(item.Entity);
                    await db.SaveChangesAsync();
                }
            }
            await ReloadSynchronizeItems();
        }
        public DelegateCommand<SynchronizeItem> RemoveSynchronizeItemCommand { get; set; }

        private async Task EditSynchronizeItem(SynchronizeItem item)
        {
            var vm = new EditSynchronizeItemDialogViewModel(item);
            var dialog = new EditSynchronizeItemDialog();
            dialog.DataContext = vm;
            var result = await DialogHost.Show(dialog, "DialogHost");
            await ReloadSynchronizeItems();
        }
        public DelegateCommand<SynchronizeItem> EditSynchronizeItemCommand { get; set; }

        public async Task ReloadSynchronizeItems()
        {
            await using (var db = new ApplicationDbContext())
            {
                SynchronizeItems.Clear();

                db.EnsureCreated();
                db.Migrate();
                var items = db.SynchronizeItems.Select(x => new SynchronizeItem(x)).ToList();
                items.ForEach(x =>
                {
                    SynchronizeItems.Add(x);
                });
            }
        }
    }

    [Obsolete(null, true)]
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
