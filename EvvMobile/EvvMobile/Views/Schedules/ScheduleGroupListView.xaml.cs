using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvvMobile.Localization;
using EvvMobile.Pages.Schedules;
using EvvMobile.Statics;
using EvvMobile.ViewModels.Schedules;
using EvvMobile.Views.Base;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace EvvMobile.Views.Schedules
{
    public partial class ScheduleGroupListView : NonPersistentSelectedItemListView
    {
        public ScheduleGroupListView()
        {
            InitializeComponent();
        }

        protected override void SetupContent(Cell content, int index)
        {
            base.SetupContent(content, index);

            //strip
            /*
            var viewCell = content as ViewCell;
            if (viewCell != null)
            {
                var value = viewCell.BindingContext;
                var groupItemList = ItemsSource.Cast<IEnumerable<object>>().ToList();
                if (groupItemList != null)
                {
                    foreach (var group in groupItemList)
                    {
                        var row = group.Cast<object>().ToList().IndexOf(value);
                        if (row % 2 == 0)
                        {
                            viewCell.View.BackgroundColor =  Palette._020;
                        }
                        else if(row >0)
                        {
                            viewCell.View.BackgroundColor = Color.AliceBlue; 
                        }
                    }

                }                
            }*/

        }
    }

    
}
