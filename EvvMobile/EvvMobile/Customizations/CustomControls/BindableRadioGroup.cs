using System;
using System.Collections;
using System.Collections.Generic;
using EvvMobile.Helper;
using Xamarin.Forms;

namespace EvvMobile.Customizations.CustomControls
{
    public class BindableRadioGroup: StackLayout
    {
       
        public List<CustomRadioButton> rads;

        public BindableRadioGroup()
        {

            rads = new List<CustomRadioButton>();
        }



        public static BindableProperty ItemsSourceProperty =
            BindableProperty.Create("ItemsSource", typeof(IEnumerable), typeof(BindableRadioGroup),  default(IEnumerable),BindingMode.OneWay, propertyChanged: OnItemsSourceChanged);


        public static BindableProperty SelectedIndexProperty =
            BindableProperty.Create("SelectedIndex", typeof(int), typeof(BindableRadioGroup), -1, BindingMode.TwoWay, propertyChanged:OnSelectedIndexChanged );

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

       
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }
      
        public event EventHandler<int> CheckedChanged;



        private static void OnItemsSourceChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var radButtons = bindable as BindableRadioGroup;
            if (radButtons == null) return;
            radButtons.rads.Clear();
            radButtons.Children.Clear();
            var newValues = newvalue as IEnumerable;
            if (newValues != null)
            {
              
                int radIndex = 0;
                foreach (var item in newValues)
                {
                    var rad = new CustomRadioButton();
                    rad.Text = item.ToString();
                    rad.Id = radIndex;  
                   
                    rad.CheckedChanged += radButtons.OnCheckedChanged;
                  
                    radButtons.rads.Add(rad);
                                    
                    radButtons.Children.Add(rad);
                    radIndex++;
                }
            }
        }

        private void OnCheckedChanged(object sender, EventArgs<bool> e)
        {
           
           if (e.Value == false) return;

            var selectedRad = sender as CustomRadioButton;

            foreach (var rad in rads)
            {
                if(selectedRad== null ||!selectedRad.Id.Equals(rad.Id))
                {
                    rad.Checked = false;
                }
                else
                {
					if(CheckedChanged != null)
                    	CheckedChanged.Invoke(sender, rad.Id);
                    SelectedIndex = rad.Id;
                }
                
            }

        }

        private static void OnSelectedIndexChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var newIntValue = newvalue as int?;
            if (newIntValue == null ||newIntValue.Value == -1) return;

            var bindableRadioGroup = bindable as BindableRadioGroup;

            if(bindableRadioGroup == null) return;
            foreach (var rad in bindableRadioGroup.rads)
            {
                if (rad.Id == bindableRadioGroup.SelectedIndex)
                {
                    rad.Checked = true;
                }
               
            }


        }
    
    }
}
