using ERService.Infrastructure.Base;
using ERService.Infrastructure.Dialogs;
using Prism.Events;

namespace ERService.Settings.ViewModels
{
    public class GeneralSettingsViewModel : DetailViewModelBase
    {
        public GeneralSettingsViewModel(IEventAggregator eventAggregator, IMessageDialogService messageDialogService) : base(eventAggregator, messageDialogService)
        {
            Title = "Ogólne";
        }        

        public string Content { get { return "Ustawienia ogólne"; } }

        #region Navigation
        public override bool KeepAlive => true;
        #endregion
    }
}
