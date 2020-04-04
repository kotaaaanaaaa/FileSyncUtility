using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FileSyncUtility.Common
{
    public static class FileAccessor
    {
        /// <summary>
        /// FileSystemInfoを取得します
        /// </summary>
        /// <param name="path">パス</param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable<FileSystemInfoEntity> EnumerateInfo(string path, Action<string, IEnumerable<FileSystemInfoEntity>> action = null)
        {
            return EnumerateInfo(path, "", action);
        }

        /// <summary>
        /// FileSystemInfoを取得します
        /// </summary>
        /// <param name="root">基点となるパス</param>
        /// <param name="relative">相対パス</param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable<FileSystemInfoEntity> EnumerateInfo(string root, string relative, Action<string, IEnumerable<FileSystemInfoEntity>> action = null)
        {
            var infos = FetchInfo(root, relative)
                .ToList();
            var files = infos
                .Where(x => x.Value.Attributes == FileAttributes.Archive);
            action?.Invoke(relative, files);

            var dirs = infos
                .Where(x => x.Value.Attributes == FileAttributes.Directory);
            action?.Invoke(relative, dirs);

            dirs.ToList()
                .ForEach(x =>
                {
                    Directory.CreateDirectory(Path.Combine(root, x.Path));
                    var dirFiles = EnumerateInfo(root, x.Path, action);
                    files = files.Union(dirFiles);
                });

            return files;
        }

        /// <summary>
        /// FileSystemInfoを取得します
        /// </summary>
        /// <param name="root">基点となるパス</param>
        /// <param name="relative">相対パス</param>
        /// <returns></returns>
        public static IEnumerable<FileSystemInfoEntity> FetchInfo(string root, string relative)
        {
            var searchPattern = "*";
            var searchOption = SearchOption.TopDirectoryOnly;

            var di = new DirectoryInfo(Path.Combine(root, relative));

            if (!di.Exists)
                return new List<FileSystemInfoEntity>();

            var files = di
                .EnumerateFiles(searchPattern, searchOption)
                .Select(x => new FileSystemInfoEntity
                {
                    Value = x,
                    Path = RelativePath(x.FullName, root),
                }
                );
            var dirs = di
                .EnumerateDirectories(searchPattern, searchOption)
                .Select(x => new FileSystemInfoEntity
                {
                    Value = x,
                    Path = RelativePath(x.FullName, root),
                }
                );
            var infos = files.Union(dirs);
            return infos;
        }

        /// <summary>
        /// 相対パスを取得します
        /// </summary>
        /// <param name="path">パス</param>
        /// <param name="root">基点となるパス</param>
        /// <returns></returns>
        public static string RelativePath(string path, string root)
        {
            var _path = Path.GetFullPath(path);
            var _root = Path.GetFullPath(root);
            if (_path.StartsWith(_root) && _path.Length > _root.Length)
                return _path.Remove(0, _root.Length + 1);
            if (_path.StartsWith(_root))
                return "";
            return _path;
        }

        /// <summary>
        /// ファイルをコピーします
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        public static void Copy(string src, string dst)
        {
            var di = new DirectoryInfo(dst);
            var dir = di.Parent.FullName;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.Copy(src, dst, true);
        }

        /// <summary>
        /// ネットワークフォルダにマウントします
        /// </summary>
        /// <param name="path">接続先のUNCパス</param>
        /// <param name="username">接続時のユーザ名</param>
        /// <param name="password">接続時のパスワード</param>
        /// <returns></returns>
        private static bool NetMount(string path, string username, string password)
        {
            var result = NetMount(path, true, username, password);
            return result.StartsWith(@"コマンドは正常に終了しました。");
        }

        /// <summary>
        /// ネットワークフォルダのマウントを解除します
        /// </summary>
        /// <param name="path"></param>
        private static void NetUnmount(string path)
        {
            NetMount(path, false, "", "");
        }

        /// <summary>
        /// net(net use)コマンドを実行します
        /// </summary>
        /// <param name="path">接続先のUNCパス</param>
        /// <param name="use">接続時はtrue 切断時はfalse</param>
        /// <param name="username">接続時のユーザ名</param>
        /// <param name="password">接続時のパスワード</param>
        /// <returns></returns>
        private static string NetMount(string path, bool use, string username, string password)
        {
            var psInfo = new ProcessStartInfo
            {
                FileName = "net",
                Arguments = NetMountQuery(path, use, username, password),
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };

            var p = Process.Start(psInfo);
            var output = p.StandardOutput.ReadToEnd().Replace("\r\r\n", "\n");
            return output;
        }

        /// <summary>
        /// net(net use)コマンドのクエリを返します
        /// </summary>
        /// <param name="path">接続先のUNCパス</param>
        /// <param name="use">接続時はtrue 切断時はfalse</param>
        /// <param name="username">接続時のユーザ名</param>
        /// <param name="password">接続時のパスワード</param>
        /// <returns></returns>
        private static string NetMountQuery(string path, bool use, string username, string password)
        {
            if (use)
            {
                var result = $"use {path}";
                if (!string.IsNullOrWhiteSpace(password))
                {
                    result += $" {password}";
                }

                if (!string.IsNullOrWhiteSpace(username))
                {
                    result += $" /user:{username}";
                }

                return result + " /persistent:no";
            }
            return $"use {path} /delete";
        }
    }
}
