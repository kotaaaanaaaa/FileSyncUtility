using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace FileSyncUtility.Common
{
    public static class FileSynchronizer
    {
        /// <summary>
        /// 単純同期する
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        public static async Task SimpleSynchronize(string src, string dst)
        {
            Log.Information("SimpleSynchronize");
            bool Predict(FileInfo srcInfo, FileInfo dstInfo)
            {
                return true;
            }

            await SynchronizeInternal(src, dst, Predict);
        }

        /// <summary>
        /// 高速同期する
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        public static async Task FastSynchronize(string src, string dst)
        {
            Log.Information("FastSynchronize");
            bool Predict(FileInfo srcInfo, FileInfo dstInfo)
            {
                if (srcInfo.Length != dstInfo.Length)
                    return true;
                if (srcInfo.LastWriteTime > dstInfo.LastWriteTime)
                    return true;
                return false;
            }

            await SynchronizeInternal(src, dst, Predict);
        }

        /// <summary>
        /// 同期する
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        /// <param name="predict"></param>
        private static async Task SynchronizeInternal(string src, string dst, Func<FileInfo, FileInfo, bool> predict)
        {
            async Task SyncTask(string relative, IEnumerable<FileSystemInfoEntity> infos)
            {
                var srcFiles = infos
                    .Where(x => (x.Value.Attributes & FileAttributes.Archive) == FileAttributes.Archive)
                    .ToList();
                var srcDirs = infos
                    .Where(x => (x.Value.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                    .ToList();

                if (srcFiles.Any())
                {
                    var dstFiles = (await FileAccessor.EnumerateInfo(dst, relative, null, true))
                        .Where(x => x.Value.Attributes == FileAttributes.Archive);
                    var target = Filter(srcFiles, dstFiles, predict);
                    await Task.Run(() =>
                    {
                        target.ToList()
                            .ForEach(x =>
                            {
                                FileAccessor.Copy(Path.Combine(src, x.Path), Path.Combine(dst, x.Path));
                            });
                    });
                }

                if (srcDirs.Any())
                {
                    srcDirs.ForEach(x => Directory.CreateDirectory(Path.Combine(dst, x.Path)));
                }
            }

            await FileAccessor.EnumerateInfo(src, SyncTask);
        }

        /// <summary>
        /// FileSystemInfoを抽出する
        /// </summary>
        /// <param name="srcInfos"></param>
        /// <param name="dstInfos"></param>
        /// <param name="predict">抽出条件</param>
        /// <returns></returns>
        private static IEnumerable<FileSystemInfoEntity> Filter(IEnumerable<FileSystemInfoEntity> srcInfos, IEnumerable<FileSystemInfoEntity> dstInfos, Func<FileInfo, FileInfo, bool> predict)
        {
            return srcInfos.Where(srcInfo =>
            {
                if (!dstInfos.Any())
                    return true;

                var dstInfo = dstInfos.FirstOrDefault(x => x.Path == srcInfo.Path);
                if (dstInfo == null)
                {
                    return true;
                }

                var srcFileInfo = srcInfo.Value as FileInfo;
                var dstFileInfo = dstInfo.Value as FileInfo;

                return predict(srcFileInfo, dstFileInfo);
            });
        }
    }
}
