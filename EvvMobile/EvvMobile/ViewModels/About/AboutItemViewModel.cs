using System;
using EvvMobile.ViewModels.Base;

namespace EvvMobile.ViewModels.About
{
    public class AboutItemViewModel : BaseViewModel
    {
        public string Description { get; set; }
        public string Uri { get; set; }

        public bool UriIsPresent
        {
            get
            {
                return !String.IsNullOrWhiteSpace(Uri);
            }
        }
    }
}
