using Xunit;

namespace FileSyncUtility.Common.Tests
{
    public class FileAccessorTests
    {
        [Fact]
        public void NetMountTest1()
        {
            FileAccessor.NetMount(@"\\127.0.0.1", "", "").IsTrue();
            bool exceptionRaised = false;
            try
            {
                FileAccessor.NetUnmount(@"\\127.0.0.1");
            }
            catch
            {
                exceptionRaised = true;
            }
            exceptionRaised.IsFalse();
        }

        [Fact()]
        public void EnumerateInfoTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void EnumerateInfoTest1()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void FetchInfoTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void RelativePathTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void CopyTest()
        {
            Assert.True(false, "This test needs an implementation");
        }
    }
}