using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EvvMobile.Validation
{
    public class ValidationDatePicker : DatePicker, IControlValidation
    {
        #region Properties
        public BaseControlValidation<ValidationDatePicker> _Validate;
        #endregion

        #region Constructor
        public ValidationDatePicker()
        {
            _Validate = new BaseControlValidation<ValidationDatePicker>(
                this,
                DatePicker.DateProperty.PropertyName,
                SetPrivateFilelds);
        }

        private void SetPrivateFilelds(bool _hasError, string _errorMessage)
        {
            // Set private fields
            this.HasError = _hasError;
            this.ErrorMessage = _errorMessage;
        }
        #endregion

        #region HasError

        public static readonly BindableProperty HasErrorProperty =
            BindableProperty.Create("HasError", typeof(bool), typeof(ValidationDatePicker), false);

        public bool HasError
        {
            get { return (bool)GetValue(HasErrorProperty); }
            private set { SetValue(HasErrorProperty, value); }
        }
        #endregion

        #region ErrorMessage

        public static readonly BindableProperty ErrorMessageProperty =
            BindableProperty.Create("ErrorMessage", typeof(string), typeof(ValidationDatePicker), string.Empty);

        public string ErrorMessage
        {
            get { return (string)GetValue(ErrorMessageProperty); }
            private set { SetValue(ErrorMessageProperty, value); }
        }
        #endregion

        #region ShowErrorMessage

        public static readonly BindableProperty ShowErrorMessageProperty =
            BindableProperty.Create("ShowErrorMessage", typeof(bool), typeof(ValidationDatePicker), false, propertyChanged: OnPropertyChanged);

        private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ValidationDatePicker control = bindable as ValidationDatePicker;
            if (control != null && control.BindingContext != null)
            {
                control._Validate.CheckValidation();
            }
        }

        public bool ShowErrorMessage
        {
            get { return (bool)GetValue(ShowErrorMessageProperty); }
            set { SetValue(ShowErrorMessageProperty, value); }
        }
        #endregion

        #region On Binding context chagned
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            this._Validate.CheckValidation(); // Check for validation
        }
        #endregion
    }
}
