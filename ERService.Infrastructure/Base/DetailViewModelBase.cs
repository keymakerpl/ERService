using ERService.Infrastructure.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ERService.Infrastructure.Base
{
    //TODO: Split to DetailViewModelBase and DetailModel?
    public abstract class DetailViewModelBase : BindableBase, IDetailViewModelBase
    {
        protected readonly IEventAggregator EventAggregator;
        //protected readonly IMessageDialogService MessageDialogService;
        private string _title;
        private bool _hasChanges;

        public bool AllowLoadAsync { get; set; } = true;

        public DetailViewModelBase(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            //MessageDialogService = messageDialogService;
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            CloseCommand = new DelegateCommand(OnCloseDetailViewExecute);
            CancelCommand = new DelegateCommand(OnCancelEditExecute, OnCancelEditCanExecute);
        }        

        public ICommand SaveCommand { get; private set; }

        public ICommand CloseCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        public Guid ID { get; protected set; }

        public string Title
        {
            get { return _title; }
            protected set
            {
                _title = value;
                RaisePropertyChanged();
            }
        }

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

        public abstract Task LoadAsync(Guid id);

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
                if (databaseValues == null) //sprawdzamy czy jest nadal w bazie
                {
                    //await MessageDialogService.ShowOkCancelDialog("The entity has been deleted by another user.", Title);
                    //RaiseDetailDeletedEvent(ID);
                    return;
                }

                //var dialogResult =
                //    await MessageDialogService.ShowOkCancelDialog(
                //        "Someone else has made changes in database. Override data with yours?", Title);

                //if (dialogResult == MessageDialogResult.OK)
                //{
                //    var entry = e.Entries.Single(); //pobierz krotkę której nie można zapisać
                //    entry.OriginalValues.SetValues(entry.GetDatabaseValues()); //pobierz aktualne dane z db (aby zaktualizować rowversion w current row)
                //    await saveFunc(); //zapisz
                //}
                //else
                //{
                //    await e.Entries.Single().ReloadAsync(); //przeładuj cache krotki z bazy
                //    await LoadAsync(Id); //załaduj ponownie model
                //}
            }

            afterSaveAction();
        }        

        protected abstract void OnSaveExecute();

        protected abstract bool OnSaveCanExecute();

        protected abstract void OnCancelEditExecute();

        protected abstract bool OnCancelEditCanExecute();

        protected virtual void OnCloseDetailViewExecute()
        {
            //TODO: MessageService
            if (HasChanges)
            {
                //var result = await MessageDialogService.ShowOkCancelDialog("Continue and Cancel changes?", Title);
                var result = MessageBox.Show("Leave changes?", "Continue?", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            EventAggregator.GetEvent<AfterDetailClosedEvent>().Publish(new AfterDetailClosedEventArgs()
            {
                Id = this.ID,
                ViewModelName = this.GetType().Name
            });
        }

        protected virtual void RaiseDetailSavedEvent(Guid modelId, string displayMember)
        {
            EventAggregator.GetEvent<AfterDetailSavedEvent>()
                .Publish(new AfterDetailSavedEventArgs()
                {
                    Id = modelId,
                    DisplayMember = displayMember,
                    ViewModelName = this.GetType().Name
                });
        }
    }
}
