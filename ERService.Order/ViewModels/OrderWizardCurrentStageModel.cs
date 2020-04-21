using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Helpers;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Globalization;
using System.Windows.Controls;

namespace ERService.OrderModule.ViewModels
{
    public class OrderWizardCurrentStageModel : DetailViewModelBase
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IRegionManager _regionManager;

        public CultureInfo Culture { get; }

        public OrderWizardCurrentStageModel(IRegionManager regionManager, IEventAggregator eventAggregator, IMessageDialogService messageDialogService) : base(eventAggregator, messageDialogService)
        {
            _regionManager = regionManager;

            Culture = new CultureInfo("pl-PL");            

            Context = _regionManager.Regions[RegionNames.OrderWizardStageRegion].Context as IOrderContext;

            DropDownCloseCommand = new DelegateCommand<object>(OnDropDownClosed);
            AddAttachmentCommand = new DelegateCommand(OnAddAttachmentExecute);
            RemoveAttachmentCommand = new DelegateCommand(OnRemoveAttachmentExecute, OnRemoveAttachmentCanExecute);

            WizardMode = true;
        }

        public DelegateCommand AddAttachmentCommand { get; }

        public DelegateCommand<object> DropDownCloseCommand { get; }

        public DelegateCommand GoBackCommand { get; }

        public DelegateCommand<object> PrintCommand { get; }

        public DelegateCommand RemoveAttachmentCommand { get; }

        public override bool KeepAlive => true;

        public bool WizardMode { get; }

        public IOrderContext Context { get; private set; }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        private void OnAddAttachmentExecute()
        {
            //TODO: Make open file dialog service
            var openFileDialog = new OpenFileDialog();
            var attachment = new Blob();
            if (openFileDialog.ShowDialog() == true)
            {
                var fileBinary = FileUtils.GetFileBinary(openFileDialog.FileName);
                attachment.Data = fileBinary;
                attachment.FileName = openFileDialog.SafeFileName;
                attachment.Size = fileBinary.Length;
                attachment.Description = $"File attachment for order: {Context.Order.Number}";
                attachment.Checksum = Cryptography.CalculateMD5(openFileDialog.FileName);

                Context.Attachments.Add(attachment);
                Context.Order.Model.Attachments.Add(attachment);
            }
        }

        private void OnDropDownClosed(object arg)
        {
            var box = arg as AutoCompleteBox;
            if (box?.SelectedItem != null && box.IsMouseOver)
            {
                Context.InitializeCustomer(Context.SelectedCustomer);
                IsReadOnly = true;
            }
        }

        private bool OnRemoveAttachmentCanExecute()
        {
            return Context.SelectedAttachment != null;
        }

        private void OnRemoveAttachmentExecute()
        {
            Context.Order.Model.Attachments.Remove(Context.SelectedAttachment);
            Context.Attachments.Remove(Context.SelectedAttachment);
        }
    }
}