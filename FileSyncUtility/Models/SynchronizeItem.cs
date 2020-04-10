using FileSyncUtility.Common;
using FileSyncUtility.Common.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Threading.Tasks;

namespace FileSyncUtility.Models
{
    public class SynchronizeItem : BindableBase
    {
        public SynchronizeItemEntity Entity { get; }

        /// <summary>
        /// GUID
        /// </summary>
        public string Guid
        {
            get => Entity.Guid;
            set => Entity.Guid = value;
        }

        /// <summary>
        /// 同期元パス
        /// </summary>
        public string SourcePath
        {
            get => Entity.SourcePath;
            set => Entity.SourcePath = value;
        }

        /// <summary>
        /// 同期先パス
        /// </summary>
        public string DestinationPath
        {
            get => Entity.DestinationPath;
            set => Entity.DestinationPath = value;
        }

        /// <summary>
        /// 最終実行時刻
        /// </summary>
        public DateTime LastExecuteTime
        {
            get => Entity.LastExecuteTime;
            set => Entity.LastExecuteTime = value;
        }

        /// <summary>
        /// 最終同期時刻
        /// </summary>
        public DateTime LastSynchronizeTime
        {
            get => Entity.LastSynchronizeTime;
            set => Entity.LastSynchronizeTime = value;
        }

        /// <summary>
        /// 同期中
        /// </summary>
        public bool IsSyncing
        {
            get=>_isSyncing;
            set=>SetProperty(ref _isSyncing, value);
        }
        private bool _isSyncing = false;

        public SynchronizeItem()
        {
            Entity = new SynchronizeItemEntity();
            SynchronizeCommand = new DelegateCommand(async () => await Synchronize(), () => CanSynchronize());
        }

        public SynchronizeItem(SynchronizeItemEntity entity)
        {
            Entity = entity;
            SynchronizeCommand = new DelegateCommand(async () => await Synchronize(), () => CanSynchronize());
        }

        /// <summary>
        /// 同期する
        /// </summary>
        /// <returns></returns>
        private async Task Synchronize()
        {
            IsSyncing = true;
            SynchronizeCommand.RaiseCanExecuteChanged();

            await using (var db = new ApplicationDbContext())
            {
                LastExecuteTime = DateTime.Now;
                db.SynchronizeItems
                    .Attach(Entity);
                db.Entry(Entity)
                    .Property(x => x.LastExecuteTime).IsModified = true;
                await db.SaveChangesAsync();
            }

            FileSynchronizer.FastSynchronize(SourcePath, DestinationPath);

            await using (var db = new ApplicationDbContext())
            {
                LastSynchronizeTime = DateTime.Now;
                db.SynchronizeItems.Attach(Entity);
                await db.SaveChangesAsync();
            }

            IsSyncing = false;
            SynchronizeCommand.RaiseCanExecuteChanged();
        }
        public DelegateCommand SynchronizeCommand { get; set; }

        /// <summary>
        /// 同期可能
        /// </summary>
        /// <returns></returns>
        private bool CanSynchronize()
        {
            return !IsSyncing;
        }
    }
}