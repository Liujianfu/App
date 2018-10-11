using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvvMobile.ViewModels.Schedules;
using EvvMobile.Views.Base;
using Xamarin.Forms;

namespace EvvMobile.Views.Schedules
{
    public partial class ModifierGridView: ModifierGridViewXaml
    {
        public ModifierGridView()
        {
            InitializeComponent();
        }

        private void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.Delete();
            }
        }
    }
    public abstract class ModifierGridViewXaml : ModelBoundContentView<ModifierCellViewModel> { }
    
}
