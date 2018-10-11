using System;
using System.Collections.Generic;
using System.Linq;
using EvvMobile.Statics;
using EvvMobile.Views.Base;
using Xamarin.Forms;

namespace EvvMobile.Views.Syncs
{
    public partial class SyncLocationTrackingListView : NonPersistentSelectedItemListView
    {
        public SyncLocationTrackingListView()
        {
            InitializeComponent();
        }
        protected override void SetupContent(Cell content, int index)
        {
            base.SetupContent(content, index);

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
                            viewCell.View.BackgroundColor = Color.White;
                        }
                        else if (row > 0)
                        {
                            viewCell.View.BackgroundColor = Color.AliceBlue;
                        }
                    }

                }
            }

        }
    }
}
