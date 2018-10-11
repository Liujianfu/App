using System;
using EvvMobile.Localization;
using EvvMobile.Statics;
using EvvMobile.Views.Base;
using Xamarin.Forms;

namespace EvvMobile.Views.ClockInOuts
{
    public partial class ClockInOutListView : NonPersistentSelectedItemListView
    {
        public ClockInOutListView()
        {
            InitializeComponent();
        }

        private void ViewCellAppearing(object sender, EventArgs e)
        {
            var viewCell = (ViewCell)sender;
            if (viewCell.View != null)
            {
                if (this._isRowEven)
                {
                    viewCell.View.BackgroundColor = Palette._020;
                }
                else
                {
                    viewCell.View.BackgroundColor = Color.White;
                }
            }

            this._isRowEven = !this._isRowEven;
        }


        private bool _isRowEven;
    }
}
