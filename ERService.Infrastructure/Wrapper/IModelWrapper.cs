using System.ComponentModel;

namespace ERService.Infrastructure.Wrapper
{
    public interface IModelWrapper<T> : INotifyDataErrorInfo, INotifyPropertyChanged
    {
        T Model { get; set; }
    }
}