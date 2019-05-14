﻿using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;


namespace ERService.Infrastructure.Wrapper
{

    /// <summary>
    /// Klasa Agreguje błędy z propertisów. Powiadamia o nowych błędach eventem.
    /// </summary>
    public class NotifyDataErrorInfoBase : BindableBase, INotifyDataErrorInfo
    {
        private Dictionary<string, List<string>> _errorsByPropertyName
            = new Dictionary<string, List<string>>();

        public IEnumerable GetErrors(string propertyName)
        {
            return _errorsByPropertyName.ContainsKey(propertyName) ? _errorsByPropertyName[propertyName] : null;
        }

        public bool HasErrors => _errorsByPropertyName.Any();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected virtual void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            base.RaisePropertyChanged(nameof(HasErrors));
        }

        protected void AddError(string propertyName, string error)
        {
            if (!_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName[propertyName] = new List<string>();
            }
            if (!_errorsByPropertyName[propertyName].Contains(error))
            {
                _errorsByPropertyName[propertyName].Add(error);
                OnErrorsChanged(propertyName);
            }
        }

        protected void ClearErrors(string propertyName)
        {
            if (_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }
    }
}