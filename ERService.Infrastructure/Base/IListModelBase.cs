using System;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Regions;

namespace ERService.Infrastructure.Base
{
    public interface IListModelBase<T>
    {
        DelegateCommand AddCommand { get; set; }
        DelegateCommand DeleteCommand { get; set; }
        ObservableCollection<T> Models { get; set; }
        T SelectedModel { get; set; }

        void LoadAsync();
        void ShowDetail(NavigationParameters parameters);
        void OnMouseDoubleClickExecute();
        void OnAddExecute();
        void OnDeleteExecute();
        bool OnDeleteCanExecute();
    }
}