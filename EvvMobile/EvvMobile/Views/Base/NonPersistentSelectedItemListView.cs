﻿using EvvMobile.Statics;
using Xamarin.Forms;

namespace EvvMobile.Views.Base
{
    public  class NonPersistentSelectedItemListView : ListView
    {
        public NonPersistentSelectedItemListView()
        {

            // This prevents the ugly default highlighting of the selected cell upon navigating back to a list view.
            // The side effect is that the list view will no longer be maintaining the most recently selected item (if you're into that kind of thing).
            // Probably not the best way to remove that default SelectedItem styling, but simple and straighforward.
            ItemSelected += (sender, e) => SelectedItem = null;
        }

    }
}

