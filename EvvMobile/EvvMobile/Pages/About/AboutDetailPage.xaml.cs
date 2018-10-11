using EvvMobile.Pages.Base;
using EvvMobile.ViewModels.About;

namespace EvvMobile.Pages.About
{
    public partial class AboutDetailPage : AboutDetailPageXaml
    {
        public AboutDetailPage()
        {
            InitializeComponent();
        }
    }

    public class AboutDetailPageXaml : ModelBoundWithHomeButtonContentPage<AboutItemViewModel> { }
}
