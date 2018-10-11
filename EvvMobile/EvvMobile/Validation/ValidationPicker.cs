using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EvvMobile.Validation
{
    public class ValidationPicker: Picker, IControlValidation
    {
        #region Constructor
        public ValidationPicker()
        {
            _Validate = new BaseControlValidation<ValidationPicker>(
                this,
                ValidationPicker.SelectedValueProperty.PropertyName,
                this.SetPrivateFields);
        }


        private void SetPrivateFields(bool _hasError, string _errorMessage)
        {
            this.HasError = _hasError;
            this.ErrorMessage = _errorMessage;
        }
        public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;
        #endregion
        #region Properties
        Boolean _disableNestedCalls;



        public static readonly BindableProperty SelectedValueProperty =
            BindableProperty.Create("SelectedValue", typeof(Object), typeof(ValidationPicker),
                null, BindingMode.TwoWay);


        public Object SelectedValue
        {
            get { return GetValue(SelectedValueProperty); }
            set
            {
                SetValue(SelectedValueProperty, value);
            }
        }

        public String SelectedValuePath { get; set; }
        #endregion
        #region Validations Properties        

        #region Has Error
        public static readonly BindableProperty HasErrorProperty =
            BindableProperty.Create("HasError", typeof(bool), typeof(ValidationPicker), false, defaultBindingMode: BindingMode.TwoWay);

        public bool HasError
        {
            get { return (bool)GetValue(HasErrorProperty); }
            private set { SetValue(HasErrorProperty, value); }
        }
        #endregion

        #region ErrorMessage

        public static readonly BindableProperty ErrorMessageProperty =
            BindableProperty.Create("ErrorMessage", typeof(string), typeof(ValidationPicker), string.Empty);

        public string ErrorMessage
        {
            get { return (string)GetValue(ErrorMessageProperty); }
            set { SetValue(ErrorMessageProperty, value); }
        }
        #endregion

        #region ShowErrorMessage

        public static readonly BindableProperty ShowErrorMessageProperty =
            BindableProperty.Create("ShowErrorMessage", typeof(bool), typeof(ValidationPicker), false, propertyChanged: OnShowErrorMessageChanged, defaultBindingMode: BindingMode.TwoWay);

        private static void OnShowErrorMessageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            // execute on bindable context changed method
            ValidationPicker control = bindable as ValidationPicker;
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

        #endregion
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            this._Validate.CheckValidation();
        }


        public BaseControlValidation<ValidationPicker> _Validate;
    }
}
