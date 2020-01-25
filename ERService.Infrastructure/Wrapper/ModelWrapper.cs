using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ERService.Infrastructure.Wrapper
{
    /// <summary>
    /// Klasa opakowująca model. Wywołuje walidację. SetProperty ustawia i informuje subskrybentów o zmianie.
    /// </summary>
    /// <typeparam name="T">Model który chcemy opakować</typeparam>
    public class ModelWrapper<T> : NotifyDataErrorInfoBase, IModelWrapper<T>
    {
        public T Model { get; set; }

        public ModelWrapper(T model)
        {
            Model = model;
        }
        
        protected virtual TValue GetValue<TValue>([CallerMemberName]string propertyName = null)
        {
            return (TValue)typeof(T).GetProperty(propertyName).GetValue(Model);
        }

        protected override bool SetProperty<TValue>(ref TValue storage, TValue value, [CallerMemberName] string propertyName = null)
        {
            typeof(T).GetProperty(propertyName).SetValue(Model, value);
            ValidatePropertyInternal(propertyName, value);

            return base.SetProperty(ref storage, value, propertyName);
        }

        /// <summary>
        /// Walidacja 2 etapowa: 1.Adnotacje w modelu krotki 2.Custom errors
        /// </summary>
        /// <param name="propertyName"></param>
        private void ValidatePropertyInternal(string propertyName, object currentValue)
        {
            ClearErrors(propertyName);
            ValidateDataAnnotations(propertyName, currentValue);
            ValidateCustomErrors(propertyName);
        }

        private void ValidateDataAnnotations(string propertyName, object currentValue)
        {
            var context = new ValidationContext(Model) { MemberName = propertyName };
            var results = new List<ValidationResult>();

            Validator.TryValidateProperty(currentValue, context, results);
            results.ForEach(r => AddError(propertyName, r.ErrorMessage));
        }

        private void ValidateCustomErrors(string propertyName)
        {
            var errors = ValidateProperty(propertyName);
            if (errors == null) return;
            foreach (var error in errors)
            {
                AddError(propertyName, error);
            }
        }

        /// <summary>
        /// Przeładuj tę metodę jeśli chcesz dodać walidację swojego propertisa
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected virtual IEnumerable<string> ValidateProperty(string propertyName)
        {
            return null;
        }
    }
}