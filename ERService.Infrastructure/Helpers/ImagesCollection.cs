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
    public class ImagesCollection : CollectionBase
    {
        private const string filePath = "images.dat";

        public ImagesCollection()
        {
            Initialize();
        }

        private void Initialize()
        {
            CreateFileIfNotExist();

            var images = Serializer.Deserialize(filePath) as Images;
            if (images != null)
            {
                foreach (var image in images)
                {
                    List.Add(image);
                }
            }
        }

        private void CreateFileIfNotExist()
        {
            if (!File.Exists(filePath))
            {
                var images = new Images();
                Serializer.Serialize(filePath, images);
            }
        }

        public ERimage this[string imageName]
        {
            get
            {
                return List.Cast<ERimage>().Single(i => i.FileName == imageName);
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
