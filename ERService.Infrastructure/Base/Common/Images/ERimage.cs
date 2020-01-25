using System;

namespace ERService.Infrastructure.Base.Common
{
    [Serializable]
    public class ERimage
    {
        public string FileName { get; set; }

        public string Tag { get; set; }

        public string Description { get; set; }

        public string Checksum { get; set; }

        public int Size { get; set; }

        public byte[] ImageData { get; set; }
    }
}
