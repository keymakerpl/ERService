using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq.Expressions;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Repositories;
using Prism.Commands;
using Prism.Regions;

namespace ERService.Infrastructure.Base
{
    //TODO: Czy trzeba to opakować pod ListViewModel? View View!
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

        private TEntity _selectedModel;
        public virtual TEntity SelectedModel
        {
            get {return _selectedModel; }
            set {_selectedModel = value; DeleteCommand.RaiseCanExecuteChanged(); }
        }

        public virtual async void LoadAsync()
        {
            var models = await GetAllAsync();
            foreach (var model in models)
            {
                Models.Add(model);
            }
        }

        public virtual void Load(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includeProps)
        {
            var models = FindByInclude(predicate, includeProps);
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

        //TODO: Refactor .requestNavigate
        public virtual void ShowDetail(NavigationParameters parameters)
        {
            var region = parameters.GetValue<string>("REGION");
            if (region != null)
            {
                region = parameters["REGION"].ToString();
            }
            else region = RegionNames.ContentRegion;

            var viewName = parameters["ViewFullName"].ToString();

            _regionManager.Regions[region].RemoveAll();
            _regionManager.RequestNavigate(region, new Uri(viewName + parameters, UriKind.Relative));
        }
    }
}
