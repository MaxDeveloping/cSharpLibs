using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Threading;

namespace CommonLibs.WpfLibrary
{
    public abstract class ObservableObject : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string pPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pPropertyName));
            Validate();
        }

        public void OnPropertyChanged<T>(Expression<Func<T>> pPropertyFunc)
        {
            string name = ((MemberExpression)pPropertyFunc.Body).Member.Name;
            OnPropertyChanged(name);
        }

        public void OnAllPropertiesChanged()
        {
            var properties = GetType().GetProperties();
            foreach (var property in properties)
                OnPropertyChanged(property.Name);
        }

        #endregion


        #region INotifyDataErrorInfo

        private Dictionary<string, List<string>> m_ErrorDict = new Dictionary<string, List<string>>();

        public bool HasErrors
        {
            get { return m_ErrorDict.Any(kv => kv.Value != null && kv.Value.Count > 0); }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            List<string> errorsForName;
            m_ErrorDict.TryGetValue(propertyName, out errorsForName);
            return errorsForName;
        }

        public bool HasPropertyErrors<T>(Expression<Func<T>> pPropertyFunc)
        {
            string name = ((MemberExpression)pPropertyFunc.Body).Member.Name;
            var errorList = (List<string>)GetErrors(name);
            return errorList != null && errorList.Count > 0;
        }

        public void Validate()
        {
            var validationContext = new ValidationContext(this, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(this, validationContext, validationResults, true);

            foreach (var kv in m_ErrorDict.ToList())
            {
                if (validationResults.All(r => r.MemberNames.All(m => m != kv.Key)))
                {
                    m_ErrorDict.Remove(kv.Key);
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(kv.Key));
                }
            }

            var q = from r in validationResults
                    from m in r.MemberNames
                    group r by m into g
                    select g;

            foreach (var prop in q)
            {
                var messages = prop.Select(r => r.ErrorMessage).ToList();

                if (m_ErrorDict.ContainsKey(prop.Key))
                    m_ErrorDict.Remove(prop.Key);

                m_ErrorDict.TryAdd(prop.Key, messages);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(prop.Key));
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasErrors"));
        }

        #endregion
    }
}
