using ERService.Infrastructure.Base.Common;

namespace ERService.Infrastructure.Helpers
{
    public interface IImagesCollection
    {
        ERimage this[string imageName] { get; set; }

        void Add(ERimage image);
        void Save();
    }
}