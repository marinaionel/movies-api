using MoviesApi.Core.Helpers;
using NUnit.Framework;

namespace MoviesApi.Core.Test.Helpers
{
    public class MovieHelperTest
    {
        [Test]
        public void ConvertIdToInt_TestSunny()
        {
            Assert.True(MovieHelper.ConvertIdToInt("tt0000135", out int i1));
            Assert.AreEqual(135, i1);
        }

        [Test]
        public void ConvertIdToInt_TestNull()
        {
            Assert.False(MovieHelper.ConvertIdToInt(null, out int i1));
            Assert.AreEqual(0, i1);
        }

        [Test]
        public void ConvertIdToInt_TestWhiteSpace()
        {
            Assert.False(MovieHelper.ConvertIdToInt("   ", out int i1));
            Assert.AreEqual(0, i1);
        }

        [Test]
        public void ConvertIdToInt_TestRandomString()
        {
            Assert.False(MovieHelper.ConvertIdToInt("safgifwtt", out int i1));
            Assert.AreEqual(0, i1);
        }
    }
}
