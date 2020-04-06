using System;

namespace FileSyncUtility.Common.Models
{
    public class SynchronizeItem
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

        public SynchronizeItem()
        {
            Entity = new SynchronizeItemEntity();
        }

        public SynchronizeItem(SynchronizeItemEntity entity)
        {
            Entity = entity;
        }
    }
}