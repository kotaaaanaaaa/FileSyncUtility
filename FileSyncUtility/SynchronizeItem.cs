using System;

namespace FileSyncUtility
{
    public class SynchronizeItem
    {
        /// <summary>
        /// 同期元パス
        /// </summary>
        public string SourcePath { get; set; }

        /// <summary>
        /// 同期先パス
        /// </summary>
        public string DestinationPath { get; set; }

        /// <summary>
        /// 最終実行時刻
        /// </summary>
        public DateTime LastExecuteTime { get; set; }

        /// <summary>
        /// 最終同期時刻
        /// </summary>
        public DateTime LastSynchronizeTime { get; set; }
    }
}