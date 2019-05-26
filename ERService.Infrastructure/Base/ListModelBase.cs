using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using ERService.Infrastructure.Repositories;
using Prism.Commands;
using Prism.Regions;

namespace ERService.Infrastructure.Base
{
    public abstract class ListModelBase<TEntity, TContext> : GenericRepository<TEntity, TContext>, IListModelBase<TEntity>
        where TEntity : class
        where TContext : DbContext
    {
        public ListModelBase(TContext context, IRegionManager regionManager) : base(context)
        {
            _regionManager = regionManager;
            Models = new ObservableCollection<TEntity>();
        }

        public DelegateCommand AddCommand { get; set; }

        public DelegateCommand DeleteCommand { get; set; }

        private IRegionManager _regionManager;

        public ObservableCollection<TEntity> Models { get; set; }

        public TEntity SelectedModel { get; set; }

        public virtual async void LoadAsync()
        {
            var models = await GetAllAsync();
            foreach (var model in models)
            {
                Models.Add(model);
            }
        }

        public virtual void OnAddExecute()
        {
            throw new NotImplementedException();
        }

        public virtual bool OnDeleteCanExecute()
        {
            return SelectedModel != null;
        }

        public virtual async void OnDeleteExecute()
        {
            Remove(SelectedModel);
            Models.Remove(SelectedModel);
            await SaveAsync();
        }

        public abstract void OnMouseDoubleClickExecute();

        public virtual void ShowDetail(Guid id, string regionName, string viewFullName)
        {
            var parameters = new NavigationParameters();
            parameters.Add("ID", id);

            var uri = new Uri(viewFullName + parameters, UriKind.Relative);
            _regionManager.Regions[regionName].RemoveAll();
            _regionManager.RequestNavigate(regionName, uri);
        }
    }
}
