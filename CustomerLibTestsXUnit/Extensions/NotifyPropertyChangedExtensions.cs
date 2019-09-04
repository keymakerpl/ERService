using System;
using System.ComponentModel;

namespace CustomerLibTestsXUnit.Extensions
{
    public static class NotifyPropertyChangedExtensions
    {
        public static bool IsPropertyChangedFired(this INotifyPropertyChanged notifyProperty, Action action, string propertyName)
        {
            var fired = false;
            notifyProperty.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == propertyName)
                {
                    fired = true;
                }
            };

            action();

            return fired;
        }
    }
}
