namespace _2BPRG.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class BaseViewModel : INotifyPropertyChanged
    {
        private Dictionary<string, string> errors = new Dictionary<string, string>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void AddError(string propertyName, string error)
        {
            if (!errors.ContainsKey(propertyName))
            {
                errors.Add(propertyName, error);
            }
            else
            {
                errors[propertyName] = error;
            }
        }

        protected string GetAllErrors()
        {
            var errorString = string.Empty;

            foreach (var key in errors.Keys)
            {
                errorString += errors[key] + Environment.NewLine;
            }
            return errorString;
        }

        protected string GetError(string propertyName)
        {
            var error = string.Empty;
            if (error.Contains(propertyName))
            {
                error = errors[propertyName];
            }
            return error;
        }

        protected void OnPropertyChanged(params string[] propertyNames)
        {
            if (propertyNames == null)
            {
                throw new ArgumentNullException(nameof(propertyNames));
            }

            foreach (string propertyName in propertyNames)
            {
                this.OnPropertyChanged(propertyName);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            //Debug.Assert(
            //    string.IsNullOrEmpty(propertyName) ||
            //    (this.GetType().GetRuntimeProperty(propertyName) != null),
            //    "Check that the property name exists for this instance.");

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void RemoveError(string propertyName)
        {
            if (errors.ContainsKey(propertyName))
            {
                errors.Remove(propertyName);
            }
        }

        protected bool SetProperty(Func<bool> equalCheck, Action action, params string[] propertyNames)
        {

            if (equalCheck())
            {
                return false;
            }

            //this.OnPropertyChanging(propertyNames);
            action();
            OnPropertyChanged(propertyNames);

            return true;
        }

        protected bool SetProperty<T>(ref T propertyCurrentValue, T propertyNextValue, [CallerMemberName] string propertyName = null)
        {
            if (!object.Equals(propertyCurrentValue, propertyNextValue))
            {
                propertyCurrentValue = propertyNextValue;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        protected bool SetProperty<T>(ref T propertyCurrentValue, T propertyNextValue, params string[] propertyNames)
        {
            if (!Equals(propertyCurrentValue, propertyNextValue))
            {
                //this.OnPropertyChanging(propertyNames);
                propertyCurrentValue = propertyNextValue;
                OnPropertyChanged(propertyNames);
                return true;
            }
            return false;
        }

        protected void ValidateProperty(Func<bool> validator, string columName, string error)
        {
            if (validator())
            {
                AddError(columName, error);
            }
            else
            {
                RemoveError(columName);
            }
        }
    }
}
