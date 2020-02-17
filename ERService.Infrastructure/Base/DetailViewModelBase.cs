using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Events;
using ERService.Infrastructure.Notifications.ToastNotifications;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERService.Infrastructure.Base
{
    public abstract class DetailViewModelBase : BindableBase, IDetailViewModelBase, IConfirmNavigationRequest, IRegionMemberLifetime
    {
        protected readonly IEventAggregator _eventAggregator;

        protected readonly IMessageDialogService _messageDialogService;
        private bool _hasChanges;

        private bool _isReadOnly;
        private string _title;

        public DetailViewModelBase(IEventAggregator eventAggregator, IMessageDialogService messageDialogService)
        {
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;

            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            CloseCommand = new DelegateCommand(OnCloseDetailViewExecute); //TODO: Czy możemy zrobić refactor cancel i close do jednego przycisku z enumem?
            CancelCommand = new DelegateCommand(OnCancelEditExecute);

            _eventAggregator.GetEvent<AfterLicenseValidationRequestEvent>().Subscribe((e) => IsReadOnly = !e.IsValid, true);
            _eventAggregator.GetEvent<LicenseValidationRequestEvent>().Publish();            
        }

        public bool AllowLoadAsync { get; set; } = true; //TODO: czy da się z tego zrezygnować?
        public ICommand CancelCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        /// <summary>
        /// Właściwośc pomocnicza do przechowania zmiany z repo, odpala even jeśli w repo zaszły  zmiany
        /// </summary>
        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                if (_hasChanges != value)
                {
                    _hasChanges = value;
                    RaisePropertyChanged();
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public Guid ID { get; protected set; }
        public bool IsReadOnly { get => _isReadOnly; set { SetProperty(ref _isReadOnly, value); } }
        public ICommand SaveCommand { get; private set; }

        public string Title
        {
            get { return _title; }
            protected set
            {
                _title = value;
                _eventAggregator.GetEvent<AfterDetailOpenedEvent>().Publish(new AfterDetailOpenedEventArgs
                {
                    DisplayableName = value, Id = ID,
                    ViewModelName = this.ToString()
                });

                RaisePropertyChanged();
            }
        }

        public virtual void Load()
        {
            throw new NotImplementedException();
        }

        public virtual void Load(Guid id)
        {
            throw new NotImplementedException();
        }

        public virtual Task LoadAsync()
        {
            throw new NotImplementedException("Ogarnij się!");
        }

        public virtual Task LoadAsync(Guid id)
        {
            throw new NotImplementedException("Ogarnij się!");
        }

        #region Events and Events Handlers
        protected virtual void OnCancelEditExecute()
        {
            Console.WriteLine("Not implemented");
        }

        protected virtual void OnCloseDetailViewExecute()
        {            
            _eventAggregator.GetEvent<AfterDetailClosedEvent>().Publish(new AfterDetailClosedEventArgs()
            {
                Id = this.ID,
                ViewModelName = this.GetType().Name
            });
        }

        protected virtual bool OnSaveCanExecute()
        {
            return HasChanges && !IsReadOnly;
        }

        protected virtual void OnSaveExecute()
        {
            throw new NotImplementedException();
        }

        protected virtual void RaiseDetailSavedEvent(Guid modelId, string displayMember)
        {
            _eventAggregator.GetEvent<AfterDetailSavedEvent>()
                .Publish(new AfterDetailSavedEventArgs()
                {
                    Id = modelId,
                    DisplayMember = displayMember,
                    ViewModelName = this.GetType().Name
                });
        }

        protected virtual void RaiseDetailDeletedEvent(Guid modelId, string displayMember)
        {
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>()
                .Publish(new AfterDetailDeletedEventArgs()
                {
                    Id = modelId,
                    ViewModelName = this.GetType().Name
                });
        }
        #endregion

        /// <summary>
        /// Zapisuje optymistycznie, ze sprawdzaniem czy nadpisać
        /// </summary>
        /// <param name="saveFunc">Funkcja z repo SaveAsync() zwracająca Task</param>
        /// <param name="afterSaveAction">Metoda wykonuje instrukcje po zapisie, możesz użyć lambdy</param>
        /// <returns></returns>
        protected async Task SaveWithOptimisticConcurrencyAsync(Func<Task> saveFunc, Action afterSaveAction)
        {
            try
            {
                await saveFunc();
            }
            catch (DbUpdateConcurrencyException e) // rowversion się zmienił - ktoś inny zmienił dane
            {
                var databaseValues = e.Entries.Single().GetDatabaseValues();
                if (databaseValues == null) // sprawdzamy czy jest nadal w bazie
                {
                    await _messageDialogService
                        .ShowInformationMessageAsync(this, "Usunięty element...", "Element został w międzyczasie usunięty przez innego użytkownika.");
                    RaiseDetailSavedEvent(ID, Title);
                    return;
                }

                var dialogResult =
                    await _messageDialogService
                    .ShowConfirmationMessageAsync(this, "Dane zmienione przez innego użytkownika...", "Dane zostały zmienione w międzyczasie przez innego użytkownika, czy nadpisać aktualne dane?");

                if (dialogResult == DialogResult.OK)
                {
                    var entry = e.Entries.Single(); //pobierz krotkę której nie można zapisać
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues()); //pobierz aktualne dane z db (aby zaktualizować rowversion w current row)
                    await saveFunc(); //zapisz
                }
                else
                {
                    await e.Entries.Single().ReloadAsync(); //przeładuj cache krotki z bazy
                    await LoadAsync(ID); //załaduj ponownie model
                }
            }

            afterSaveAction();

            _messageDialogService.ShowInsideContainer("Zapisano...", "Zapisano nowy element.", NotificationTypes.Success);
        }

        #region Navigation
        public virtual async void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            var dialogResult = true;
            if (!IsReadOnly && HasChanges)
            {
                dialogResult = await _messageDialogService.ShowConfirmationMessageAsync(this, "Nie zapisane dane...", "Nie zapisano zmienionych danych, kontynuować?")
                    == DialogResult.OK;
            }

            continuationCallback(dialogResult);
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
            
        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public virtual bool KeepAlive => false;
        #endregion

    }
}