using ERService.Infrastructure.Base.Common;
using ERService.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace InfrastructureLibTestXUnit.Helpers
{
    public class ImagesCollectionTests
    {
        public ImagesCollectionTests()
        {
            Images = new ImagesCollection();
        }

        public ImagesCollection Images { get; }

        [Fact]
        public void AddImage()
        {
            var image = new ERimage();
            image.FileName = "logo";

        }

        [Fact]
        public void Save()
        {

        }
    }
}
