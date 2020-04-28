using ERService.Infrastructure.Base;
using ERService.Infrastructure.Dialogs;
using ERService.OrderModule.Data.Repository;
using ERService.OrderModule.OrderNumeration;
using ERService.RBAC;
using ERService.Settings.Wrapper;
using Prism.Events;
using Prism.Regions;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERService.Settings.ViewModels
{
    public class NumerationSettingsViewModel : DetailViewModelBase
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IRBACManager _rBACManager;
        private NumerationWrapper _numeration;
        private string _numerationExample;
        private INumerationRepository _repository;

        public NumerationSettingsViewModel(IEventAggregator eventAggregator, INumerationRepository numerationRepository,
            IMessageDialogService messageDialogService, IRBACManager rBACManager) : base(eventAggregator, messageDialogService)
        {
            _repository = numerationRepository;
            _rBACManager = rBACManager;

            Title = "Schemat numeracji";
        }

        public NumerationWrapper Numeration
        {
            get { return _numeration; }
            set { SetProperty(ref _numeration, value); }
        }

        public string NumerationExample
        {
            get { return _numerationExample; }
            set { SetProperty(ref _numerationExample, $"1/{value}"); }
        }

        public override async Task LoadAsync()
        {
            await LoadNumeration();
        }

        private async Task LoadNumeration()
        {
            var numerations = await _repository.GetAllAsync();
            var defaultNumeration = numerations.FirstOrDefault(n => n.Name == "default");

            if (defaultNumeration != null)
                Numeration = new NumerationWrapper(defaultNumeration);
            else
            {
                var newNumeration = new Business.Numeration();
                _repository.Add(newNumeration);
                Numeration = new NumerationWrapper(newNumeration);
            }

            var initials = String.Empty;
            try
            {
                initials = _rBACManager.LoggedUser.Initials;
            }
            catch (ArgumentOutOfRangeException)
            {
                _logger.Debug("Cant get User Initials");
            }

            Numeration.PropertyChanged += (o, a) =>
            {
                NumerationExample = OrderNumberGenerator.GetNumberFromPattern(Numeration.Pattern, initials);                
            };

            NumerationExample = OrderNumberGenerator.GetNumberFromPattern(Numeration.Pattern);
        }

        #region Events and Event Handlers

        protected override bool OnSaveCanExecute()
        {
            return true;
        }

        protected override async void OnSaveExecute()
        {
            await _repository.SaveAsync();
        }

        #endregion Events and Event Handlers

        #region Navigation

        public override bool KeepAlive => true;

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            await LoadAsync();
        }

        #endregion Navigation
    }
}