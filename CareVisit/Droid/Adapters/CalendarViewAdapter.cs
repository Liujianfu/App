using System;
using System.Collections.Generic;
using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using CareVisit.Core.Controls;
using CareVisit.Core.ViewModels;

namespace CareVisit.Droid.Adapters
{
    public class CalendarViewAdapter:BaseAdapter<CalendarCellViewModel>
    {

        List<CalendarCellViewModel> items;
    
        protected Android.Support.V4.App.Fragment context;

        public  CalendarViewAdapter(Android.Support.V4.App.Fragment context, List<CalendarCellViewModel> items)

            : base()
        {
            this.context = context;
            this.items = items;
        }


        public override long GetItemId(int position)
        {
            return position;
        }
        public override CalendarCellViewModel this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Count; }
        }


        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];

            View view = convertView;

            if(position >=7)
            {
                //if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Resource.Layout.calendar_rowcell, null);
                var textView = view.FindViewById<TextView>(Resource.Id.textViewCell);
                textView.Text = item.ToString();
                textView.TextAlignment = TextAlignment.Center;

                if(item.EventCount>0)
                {
                    var indicatorView =view.FindViewById<ImageView>(Resource.Id.imageView1);

                }

            }
            else
            {
              // if (view == null) // no view to re-use, create new
                    view = context.LayoutInflater.Inflate(Resource.Layout.calendar_weekheader, null);
                switch (position)
                {
                    case 0:
                        view.FindViewById<TextView>(Resource.Id.textViewWeekday).Text = "Sun";
                        break;
                    case 1:
                        view.FindViewById<TextView>(Resource.Id.textViewWeekday).Text = "Mon";
                        break;
                    case 2:
                        view.FindViewById<TextView>(Resource.Id.textViewWeekday).Text = "Tue";
                        break;
                    case 3:
                        view.FindViewById<TextView>(Resource.Id.textViewWeekday).Text = "Wed";
                        break;
                    case 4:
                        view.FindViewById<TextView>(Resource.Id.textViewWeekday).Text = "Thu";
                        break;
                    case 5:
                        view.FindViewById<TextView>(Resource.Id.textViewWeekday).Text = "Fri";
                        break;
                    case 6:
                        view.FindViewById<TextView>(Resource.Id.textViewWeekday).Text = "Sat";
                        break;
                }
            }
//    view.FindViewById<ImageView>(Resource.Id.Image).SetImageResource(item.ImageResourceId);
           
            return view;
        }
    }
}
