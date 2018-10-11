using EvvMobile.ViewModels.Base;
using Xamarin.Forms;

namespace EvvMobile.Pages.Base
{
    /// <summary>
    /// A generically typed ContentPage that enforces the type of its BindingContext according to TViewModel.
    /// </summary>
    public abstract class ModelBoundWithHomeButtonContentPage<TViewModel> : ContentPage where TViewModel : class, IBaseViewModel
    {
        public ModelBoundWithHomeButtonContentPage()
        {
            var homeIcon = "Home.png";
            if (Device.RuntimePlatform != Device.iOS && Device.RuntimePlatform != Device.Android)
            {
                homeIcon = "Assets/Home.png";
            }
            ToolbarItems.Add(new ToolbarItem("Home", homeIcon,  () =>
            {
                App.GoToRoot();
            }));
        }
        /// <summary>
        /// Gets the generically typed ViewModel from the underlying BindingContext.
        /// </summary>
        /// <value>The generically typed ViewModel.</value>
        protected TViewModel ViewModel
        {
            get { return base.BindingContext as TViewModel; }
        }

        /// <summary>
        /// Sets the underlying BindingContext as the defined generic type.
        /// </summary>
        /// <value>The generically typed ViewModel.</value>
        /// <remarks>Enforces a generically typed BindingContext, instead of the underlying loosely object-typed BindingContext.</remarks>
        public new TViewModel BindingContext
        {
            set 
            { 
                base.BindingContext = value;
                if (base.BindingContext != null)
                {
                    var viewModel = base.BindingContext as TViewModel;
                    if (viewModel != null)
                        viewModel.Navigation = this.Navigation;
                }
                base.OnPropertyChanged("BindingContext");
            }
        }
    }
}

