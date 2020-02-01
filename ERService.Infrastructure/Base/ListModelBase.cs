using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Events;
using ERService.Infrastructure.Repositories;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq.Expressions;

namespace ERService.Infrastructure.Base
{
    public abstract class ListModelBase<TEntity, TContext> : GenericRepository<TEntity, TContext>, IListModelBase<TEntity>
        where TEntity : class
        where TContext : DbContext
    {
        private IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private TEntity _selectedModel;
        private bool _isReadOnly;

        public ListModelBase(TContext context, IRegionManager regionManager, IEventAggregator eventAggregator) : base(context)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            AddCommand = new DelegateCommand(OnAddExecute, OnAddCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute, OnDeleteCanExecute);

            _eventAggregator.GetEvent<AfterLicenseValidationRequestEvent>().Subscribe((e) => IsReadOnly = !e.IsValid);
            _eventAggregator.GetEvent<LicenseValidationRequestEvent>().Publish();

            Models = new ObservableCollection<TEntity>();
        }

        public DelegateCommand AddCommand { get; set; }

        public DelegateCommand DeleteCommand { get; set; }

        public ObservableCollection<TEntity> Models { get; set; }

        public virtual TEntity SelectedModel
        {
            get { return _selectedModel; }
            set { _selectedModel = value; DeleteCommand.RaiseCanExecuteChanged(); }
        }

        public bool IsReadOnly
        {
            get => _isReadOnly;
            set { _isReadOnly = value; AddCommand.RaiseCanExecuteChanged(); }
        }

        public virtual void Load(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includeProps)
        {
            Models.Clear();
            var models = FindByInclude(predicate, includeProps);
            foreach (var model in models)
            {
                Models.Add(model);
            }
        }

        public virtual async void LoadAsync(QueryBuilder<TEntity> queryBuilder)
        {
            Models.Clear();
            var models = await FindByAsync(queryBuilder);
            foreach (var model in models)
            {
                Models.Add(model);
            }
        }

        public virtual async void LoadAsync()
        {
            Models.Clear();
            var models = await GetAllAsync();
            foreach (var model in models)
            {
                Models.Add(model);
            }
        }

        public abstract void OnAddExecute();        

        private bool OnAddCanExecute()
        {
            return !IsReadOnly;
        }

        public virtual bool OnDeleteCanExecute()
        {
            return SelectedModel != null && !IsReadOnly;
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