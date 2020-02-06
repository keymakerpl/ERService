using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace ERService.Infrastructure.Base
{
    public abstract class SearchViewModelBase : BindableBase
    {
        public SearchViewModelBase(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;

            SearchCommand = new DelegateCommand(OnSearchExecute);
        }

        public DelegateCommand SearchCommand { get; }

        protected IEventAggregator EventAggregator { get; }

        protected abstract void OnSearchExecute();
    }
}