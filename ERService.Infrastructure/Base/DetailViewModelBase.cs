using ERService.Infrastructure.Events;
using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERService.Infrastructure.Base
{
    public abstract class DetailViewModelBase : BindableBase, IDetailViewModelBase
    {
        private bool _hasChanges;
        protected readonly IEventAggregator EventAggregator;
        protected readonly IMessageDialogService MessageDialogService;
        private string _title;
        private int _id;

        public DetailViewModelBase(IEventAggregator eventAggregator, IMessageDialogService messageDialogService)
        {
            EventAggregator = eventAggregator;
            MessageDialogService = messageDialogService;
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);
            CloseDetailViewCommand = new DelegateCommand(OnCloseDetailViewExecute);
        }

        protected async virtual void OnCloseDetailViewExecute()
        {
            if (HasChanges)
            {
                var result = await MessageDialogService.ShowOkCancelDialog("Continue and Cancel changes?", Title);

                if (result == MessageDialogResult.Cancel)
                {
                    return;
                }
            }

            EventAggregator.GetEvent<AfterDetailClosedEvent>().Publish(new AfterDetailClosedEventArgs()
            {
                Id = this.Id,
                ViewModelName = this.GetType().Name
            });
        }

        //TODO: Replace int to Guid ID
        public abstract Task LoadAsync(int id);

        public ICommand SaveCommand { get; private set; }

        public ICommand DeleteCommand { get; private set; }

        public ICommand CloseDetailViewCommand { get; set; }

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
                    await MessageDialogService.ShowOkCancelDialog("The entity has been deleted by another user.", Title);
                    RaiseDetailDeletedEvent(Id);
                    return;
                }

                var dialogResult =
                    await MessageDialogService.ShowOkCancelDialog(
                        "Someone else has made changes in database. Override data with yours?", Title);

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

        protected abstract void OnDeleteExecute();

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
                    OnPropertyChanged();
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public int Id
        {
            get { return _id; }
            protected set { _id = value; }
        }

        public string Title
        {
            get { return _title; }
            protected set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        protected virtual void RaiseDetailSavedEvent(int modelId, string displayMember)
        {
            EventAggregator.GetEvent<AfterDetailSavedEvent>()
                .Publish(new AfterDetailSavedEventArgs()
                {
                    Id = modelId,
                    DisplayMember = displayMember,
                    ViewModelName = this.GetType().Name
                });
        }

        protected virtual void RaiseDetailDeletedEvent(int modelId)
        {
            EventAggregator.GetEvent<AfterDetailDeletedEvent>()
                .Publish(new AfterDetailDeletedEventArgs()
                {
                    Id = modelId,
                    ViewModelName = this.GetType().Name
                });
        }

        protected virtual void RaiseCollectionSavedEvent()
        {
            EventAggregator.GetEvent<AfterCollectionSavedEvent>()
                .Publish(new AfterCollectionSavedEventArgs()
                {
                    ViewModelName = this.GetType().Name
                });
        }
    }
}
