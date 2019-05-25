using System;
using System.Collections.ObjectModel;
using Prism.Commands;

namespace ERService.Infrastructure.Base
{
    public interface IListViewModelBase<T>
    {
        DelegateCommand AddCommand { get; set; }
        DelegateCommand DeleteCommand { get; set; }
        ObservableCollection<T> Models { get; set; }
        T SelectedModel { get; set; }

        void LoadAsync();
        void ShowDetail(Guid id, string regionName, string viewFullName);
        void OnMouseDoubleClickExecute();
        void OnAddExecute();
        void OnDeleteExecute();
        bool OnDeleteCanExecute();
    }
}