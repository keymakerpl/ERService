using ERService.Infrastructure.Base.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ERService.Infrastructure.Helpers
{
    public class ImagesCollection : CollectionBase, IImagesCollection
    {
        private const string filePath = "images.dat";

        public ImagesCollection()
        {
            Initialize();
        }

        private void Initialize()
        {
            var images = Serializer.Deserialize(filePath) as Images;
            if (images != null)
            {
                foreach (var image in images)
                {
                    List.Add(image);
                }
            }
        }

        public ERimage this[string tag]
        {
            get
            {
                return List.Cast<ERimage>().SingleOrDefault(i => i.Tag == tag);
            }
            set
            {
                var image = List.Cast<ERimage>().SingleOrDefault(i => i.Tag == tag);
                
                if (image != null)
                    List.Remove(image);
                
                List.Add(value);
            }
        }

        public void Add(ERimage image)
        {
            List.Add(image);
        }

        public void Save()
        {
            var images = new Images();
            foreach (var image in List.Cast<ERimage>())
            {
                images.Add(image);
            }

            Serializer.Serialize("images.dat", images);
        }
    }
}
