using ERService.Infrastructure.Base.Common;
using ERService.Infrastructure.Helpers;
using System.IO;
using Xunit;

namespace InfrastructureLibTestXUnit.Helpers
{
    public class SerializerTests
    {
        [Fact]
        public void SerializeShouldWriteFile()
        {
            var images = new Images();

            Serializer.Serialize("images.dat", images);

            Assert.True(File.Exists("images.dat"));
        }

        [Theory]
        [InlineData("images.dat")]
        public void DeserializeShouldReturnNonNullObject(string fileName)
        {
            var obj = Serializer.Deserialize(fileName);

            Assert.NotNull(obj);
            Assert.True(obj.GetType() == typeof(Images));
        }
    }
}
