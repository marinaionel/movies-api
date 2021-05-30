using MoviesApi.Core.Models;
using NUnit.Framework;

namespace MoviesApi.Core.Test.Models
{
    public class MovieTest
    {
        [Test]
        public void MovieIdString_Test()
        {
            Assert.AreEqual("tt0000135", new Movie { Id = 135 }.IdString);
            Assert.AreEqual("tt1287347", new Movie { Id = 1287347 }.IdString);
            Assert.AreEqual("tt0028782", new Movie { Id = 28782 }.IdString);
        }
    }
}
