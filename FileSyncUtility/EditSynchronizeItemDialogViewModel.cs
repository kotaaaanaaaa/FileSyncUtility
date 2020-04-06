using FileSyncUtility.Common;
using FileSyncUtility.Common.Models;
using Prism.Commands;
using Prism.Mvvm;
using System.Linq;
using System.Threading.Tasks;

namespace FileSyncUtility
{
    public class EditSynchronizeItemDialogViewModel : BindableBase
    {
        private SynchronizeItem Item;

        public EditSynchronizeItemDialogViewModel(SynchronizeItem item)
        {
            Item = item;
            SaveCommand = new DelegateCommand(async () => await Save());
        }

        /// <summary>
        /// 同期元パス
        /// </summary>
        public string SourcePath
        {
            get => Item.SourcePath;
            set
            {
                Item.SourcePath = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 同期先パス
        /// </summary>
        public string DestinationPath
        {
            get => Item.DestinationPath;
            set
            {
                Item.DestinationPath = value;
                RaisePropertyChanged();
            }
        }

        private async Task Save()
        {
            await using (var db = new ApplicationDbContext())
            {
                if (db.SynchronizeItems.Any(x => x.Guid == Item.Guid))
                {
                    var entity = db
                        .SynchronizeItems
                        .Single(x => x.Guid == Item.Guid);
                    entity.SourcePath = Item.SourcePath;
                    entity.DestinationPath = Item.DestinationPath;
                    await db.SaveChangesAsync();
                }
                else
                {
                    db.SynchronizeItems.Add(Item.Entity);
                    await db.SaveChangesAsync();
                }
            }
        }
        public DelegateCommand SaveCommand { get; set; }
    }
}
