using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EvvMobile.ViewModels.Base
{
    public interface IBaseViewModel
    {
        INavigation Navigation { get; set; }
    }
}
