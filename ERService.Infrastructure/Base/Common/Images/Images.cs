using System;
using System.Collections;

namespace ERService.Infrastructure.Base.Common
{
    [Serializable]
    public sealed class Images : CollectionBase
    {
        public void Add(ERimage image)
        {
            List.Add(image);
        }        
    }
}
