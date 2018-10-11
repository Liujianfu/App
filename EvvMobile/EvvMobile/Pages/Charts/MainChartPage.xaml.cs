using System;
using System.Threading.Tasks;
using EvvMobile.Pages.Base;
using EvvMobile.ViewModels.Charts;
using EvvMobile.ViewModels.Systems;
using Xamarin.Forms;

namespace EvvMobile.Pages.Charts
{
    public partial class MainChartPage : MainChartPageXaml
    {
        public MainChartPage()
        {
            InitializeComponent();
            var collapseMapTapGestureRecognizer = new TapGestureRecognizer();
            collapseMapTapGestureRecognizer.Tapped += (s, e) =>
            {
                CollapseImageClicked(s, e);
            };
            CollapseImage.GestureRecognizers.Add(collapseMapTapGestureRecognizer);
            CollapseImage2.GestureRecognizers.Add(collapseMapTapGestureRecognizer);
            CollapseImage3.GestureRecognizers.Add(collapseMapTapGestureRecognizer);
            CollapseImage4.GestureRecognizers.Add(collapseMapTapGestureRecognizer);
            CollapseImage5.GestureRecognizers.Add(collapseMapTapGestureRecognizer);
            CollapseImage6.GestureRecognizers.Add(collapseMapTapGestureRecognizer);

            var refreashPieChartImageTapGestureRecognizer = new TapGestureRecognizer();
            refreashPieChartImageTapGestureRecognizer.Tapped += (s, e) =>
            {
                OnUpdateClicked(s, e);
            };
            RefreshPieChartImage.GestureRecognizers.Add(refreashPieChartImageTapGestureRecognizer);
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            SystemViewModel.Instance.CleanMessages();
            if (!ViewModel.IsInitialized)
            {
  
                await  ViewModel.ExecuteStatusPieChartDataCommand(new ChartDataSearchCriteria());
                
                ViewModel.IsInitialized = true;
                
            }
            this.piechart.Chart = ViewModel.PieChart;
            var charts = ViewModel.Charts;
            this.MonthlChart.Chart = ViewModel.TwelveMonthLineChart;
            this.chart3.Chart = charts[2];
            this.chart4.Chart = charts[3];
            this.chart5.Chart = charts[4];
            this.chart6.Chart = charts[5];

        }

        private  void OnUpdateClicked(object sender, EventArgs e)
        {
            Task.Run( () =>
            {
                ViewModel.ExecuteStatusPieChartDataCommand(new ChartDataSearchCriteria()).Wait();

            }).Wait();
            this.piechart.Chart =  ViewModel.PieChart;
        }
        private  void CollapseImageClicked(object sender, EventArgs e)
        {
            bool isVisible = false;
            if(sender.Equals(CollapseImage))
                isVisible = PieChartLayout.IsVisible = !PieChartLayout.IsVisible;
            else if (sender.Equals(CollapseImage2))
                isVisible = ChartLayout2.IsVisible = !ChartLayout2.IsVisible;
            else if (sender.Equals(CollapseImage3))
                isVisible = ChartLayout3.IsVisible = !ChartLayout3.IsVisible;
            else if (sender.Equals(CollapseImage4))
                isVisible = ChartLayout4.IsVisible = !ChartLayout4.IsVisible;
            else if (sender.Equals(CollapseImage5))
                isVisible = ChartLayout5.IsVisible = !ChartLayout5.IsVisible;
            else if (sender.Equals(CollapseImage6))
                isVisible = ChartLayout6.IsVisible = !ChartLayout6.IsVisible;

            if (isVisible)
            {
                ((Image)sender).Source = ImageSource.FromResource("EvvMobile.Images.collapsearrow40.png");
            }
            else
            {
                ((Image)sender).Source = ImageSource.FromResource("EvvMobile.Images.expandarrow40.png");
            }                

        }
    }
    public class MainChartPageXaml : ModelBoundWithHomeButtonContentPage<MainChartViewModel> { }
}