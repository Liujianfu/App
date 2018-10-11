using EvvMobile.Statics;
using Xamarin.Forms;

namespace EvvMobile.ViewModels.Systems
{
    public class HomeMenuItem
    {
        public HomeMenuItem()
        {
            MenuType = MenuType.About;
        }

        public ImageSource Icon { get; set; }

        public MenuType MenuType { get; set; }

        public string Title { get; set; }

        public string Details { get; set; }

        public int Id { get; set; }
    }
}
