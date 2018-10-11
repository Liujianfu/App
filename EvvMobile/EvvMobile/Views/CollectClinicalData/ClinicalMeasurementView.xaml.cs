using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvvMobile.Statics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using EvvMobile.ViewModels.CollectClinicalData;
using EvvMobile.Views.Base;
namespace EvvMobile.Views.CollectClinicalData
{
    public partial class ClinicalMeasurementView : ClinicalMeasurementViewXaml
    {
        public ClinicalMeasurementView()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty SourceModelProperty =
                    BindableProperty.Create("Value",typeof(VisitMeasurementViewModel),typeof( ClinicalMeasurementView), propertyChanged: OnSourceModelChanged);
        private static void OnSourceModelChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var measurementView = bindable as ClinicalMeasurementView;
            if (measurementView != null)
            {
                measurementView.BindingContext = newvalue as VisitMeasurementViewModel;
                measurementView.UpdateAttributes();
            }
        }
        public VisitMeasurementViewModel SourceModel
        {
            get { return (VisitMeasurementViewModel)GetValue(SourceModelProperty); }
            set {
                SetValue(SourceModelProperty, value);
                this.BindingContext = value;
            }
        }
        private void UpdateAttributes()
        {
            AttributesList.Children.Clear();
            var columDef = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };

            var rowDef = new RowDefinition { Height = new GridLength(1, GridUnitType.Star) };
            var attributeGrid = new Grid
                {
                   VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    RowSpacing = 2,                    
                    ColumnSpacing = 0,
                    Padding = new Thickness(10, 0, 10, 0)
                };
                attributeGrid.ColumnDefinitions = new ColumnDefinitionCollection { columDef, columDef };
                attributeGrid.RowDefinitions = new RowDefinitionCollection { rowDef };
            for(int i=1;i<ViewModel.Attributes.Count;i++)
            {
                attributeGrid.RowDefinitions.Add(rowDef);
            }
            int row = 0;
            foreach(var attr in ViewModel.Attributes)
            {

                var boxView = new BoxView
                {
                    Color = Palette.LevelFourBackgroundColor,
                    Margin= new Thickness(-10, 0, -10,0 )
                };
                attributeGrid.Children.Add(boxView, 0,2, row, row+1);
                var nameLabel = new Label
                {
                    Text = attr.AttributeName+": ",
                    VerticalOptions = LayoutOptions.Center,
                    
                };
                attributeGrid.Children.Add(nameLabel,0, row);

                var binding = new Binding()
                {
                    Path = "AttributeValue",
                    Source = attr
                };
                var valueEntry = new Entry
                {
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    VerticalOptions= LayoutOptions.Center,
                    WidthRequest=100
                };
                valueEntry.SetBinding(Entry.TextProperty, binding);
                attributeGrid.Children.Add(valueEntry,1, row);
                row++;
                
            }
            AttributesList.Children.Add(attributeGrid);
        }
    }

    public abstract class ClinicalMeasurementViewXaml : ModelBoundContentView<VisitMeasurementViewModel> { }
}