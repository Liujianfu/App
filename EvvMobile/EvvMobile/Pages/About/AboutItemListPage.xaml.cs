using EvvMobile.Pages.Base;
using EvvMobile.ViewModels.About;
using Xamarin.Forms;

namespace EvvMobile.Pages.About
{
    public partial class AboutItemListPage : AboutItemListPageXaml
    {
        public AboutItemListPage()
        {
            InitializeComponent();

            // This prevents the ugly default highlighting of the selected cell upon navigating back to a list view.
            // The side effect is that the list view will no longer be maintaining the most recently selected item (if you're into that kind of thing).
            // Probably not the best way to remove that default SelectedItem styling, but simple and straighforward.
            AboutItemList.ItemSelected += (sender, e) => AboutItemList.SelectedItem = null;
            AboutBackGroundImage.Source = ImageSource.FromResource("EvvMobile.Images.about_background.png");
        }

        async void AboutItemTapped(object sender, ItemTappedEventArgs e)
        {
            AboutItemViewModel vm = ((AboutItemViewModel)e.Item);
            vm.Navigation = this.Navigation;
            await Navigation.PushAsync(new AboutDetailPage() { BindingContext = vm });
        }
    }
    public class AboutItemListPageXaml : ModelBoundWithHomeButtonContentPage<AboutItemListViewModel> { }
}
