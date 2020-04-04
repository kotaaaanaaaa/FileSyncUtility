using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileSyncUtility.Common
{
    public static class FileSynchronizer
    {
        /// <summary>
        /// 単純同期する
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        public static void SimpleSynchronize(string src, string dst)
        {
            bool predict(FileInfo srcInfo, FileInfo dstInfo)
            {
                return true;
            }

            SynchronizeInternal(src, dst, predict);
        }

        /// <summary>
        /// 高速同期する
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        public static void FastSynchronize(string src, string dst)
        {
            bool predict(FileInfo srcInfo, FileInfo dstInfo)
            {
                if (srcInfo.Length != dstInfo.Length)
                    return true;
                if (srcInfo.LastWriteTime > dstInfo.LastWriteTime)
                    return true;
                return false;
            }

            SynchronizeInternal(src, dst, predict);
        }

        /// <summary>
        /// 同期する
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        /// <param name="predict"></param>
        private static void SynchronizeInternal(string src, string dst, Func<FileInfo, FileInfo, bool> predict)
        {
            void action(string relative, IEnumerable<FileSystemInfoEntity> infos)
            {
                var srcFiles = infos
                    .Where(x => x.Value.Attributes == FileAttributes.Archive)
                    .ToList();
                var srcDirs = infos
                    .Where(x => x.Value.Attributes == FileAttributes.Directory)
                    .ToList();
                if (srcFiles.Any())
                {
                    var dstFiles = FileAccessor.EnumerateInfo(dst, relative)
                        .Where(x => x.Value.Attributes == FileAttributes.Archive);
                    var target = Filter(srcFiles, dstFiles, predict);
                    target.ToList()
                        .ForEach(x => FileAccessor.Copy(Path.Combine(src, x.Path), Path.Combine(dst, x.Path)));
                }

                if (srcDirs.Any())
                {
                    srcDirs.ForEach(x => Directory.CreateDirectory(Path.Combine(dst, x.Path)));
                }
            }

            FileAccessor.EnumerateInfo(src, action);
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

                var dstInfo = dstInfos.First(x => x.Path == srcInfo.Path);
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
