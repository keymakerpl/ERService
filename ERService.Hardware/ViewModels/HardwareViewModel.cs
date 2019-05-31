using ERService.Infrastructure.Base;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Threading.Tasks;

namespace ERService.HardwareModule.ViewModels
{
    public class HardwareViewModel : DetailViewModelBase, INavigationAware, IConfirmNavigationRequest, IRegionMemberLifetime
    {
        public bool KeepAlive => false;

        private IRegionNavigationJournal _journal;

        private bool _wizardMode;
        public bool WizardMode { get { return _wizardMode; } set { _wizardMode = value; RaisePropertyChanged(); } }

        public DelegateCommand GoForwardCommand { get; private set; }
        public DelegateCommand GoBackwardCommand { get; private set; }

        public HardwareViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            GoForwardCommand = new DelegateCommand(OnGoForwardExecute, OnGoForwardCanExecute);
            GoBackwardCommand = new DelegateCommand(OnGoBackwardExecute, OnGoBackwardCanExecute);
        }

        private bool OnGoBackwardCanExecute()
        {
            return true;
        }

        private void OnGoBackwardExecute()
        {
            _journal.GoBack();
        }

        private bool OnGoForwardCanExecute()
        {
            return true;
        }

        private void OnGoForwardExecute()
        {
            throw new NotImplementedException();
        }

        public override Task LoadAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        protected override void OnSaveExecute()
        {
            throw new NotImplementedException();
        }

        protected override bool OnSaveCanExecute()
        {
            return true;
        }

        protected override void OnCancelEditExecute()
        {
            throw new NotImplementedException();
        }

        protected override bool OnCancelEditCanExecute()
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var id = navigationContext.Parameters.GetValue<string>("ID");
            WizardMode = navigationContext.Parameters.GetValue<bool>("Wizard");

            _journal = navigationContext.NavigationService.Journal;

            if (!String.IsNullOrWhiteSpace(id))
            {
                //await LoadAsync(Guid.Parse(id));
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            
        }
    }
}
