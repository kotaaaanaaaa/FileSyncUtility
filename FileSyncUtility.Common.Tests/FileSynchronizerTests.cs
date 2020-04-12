using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace FileSyncUtility.Common.Tests
{
    public class FileSynchronizerTests : IDisposable
    {
        private readonly string SrcPath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
        private readonly string DstPath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

        private readonly ITestOutputHelper Output;

        public FileSynchronizerTests(ITestOutputHelper output)
        {
            Output = output;
        }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(SrcPath))
                    Directory.Delete(SrcPath);
            }
            catch (Exception e)
            {
                Output.WriteLine(e.Message);
            }
            try
            {
                if (Directory.Exists(DstPath))
                    Directory.Delete(DstPath);
            }
            catch (Exception e)
            {
                Output.WriteLine(e.Message);
            }
        }

        private void SetupData(string root, char dataType, string relative)
        {
            var path = Path.Combine(root, relative);
            if (char.ToUpper(dataType).Equals('D'))
            {
                Directory.CreateDirectory(path);
                return;
            }
            if (char.ToUpper(dataType).Equals('F'))
            {
                var parent = Path.GetDirectoryName(path);
                if (!Directory.Exists(parent))
                    Directory.CreateDirectory(parent);
                using (var fs = File.Create(path)) { }
                return;
            }

            return;
        }

        [Theory]
        [InlineData('D', @"dir")]
        [InlineData('F', @"dir\file")]
        [InlineData('D', @"dir\dir")]
        [InlineData('F', @"dir\dir\file")]
        public void FastSynchronizeTest1(char type, string path)
        {
            SetupData(SrcPath, type, path);

            FileSynchronizer.FastSynchronize(SrcPath, DstPath).Wait();

            var actualPath = Path.Combine(DstPath, path);
            if (char.ToUpper(type).Equals('D'))
            {
                Directory.Exists(actualPath).Is(true);
                File.Exists(actualPath).Is(false);
            }
            else if (char.ToUpper(type).Equals('F'))
            {
                Directory.Exists(actualPath).Is(false);
                File.Exists(actualPath).Is(true);
            }
        }

        [Fact()]
        public void SimpleSynchronizeTest()
        {
            Assert.True(false, "This test needs an implementation");
        }
    }
}
