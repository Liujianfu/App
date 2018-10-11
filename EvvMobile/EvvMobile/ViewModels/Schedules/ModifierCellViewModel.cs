using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvvMobile.ViewModels.Base;

namespace EvvMobile.ViewModels.Schedules
{
    public class ModifierCellViewModel : BaseViewModel
    {
        public event EventHandler<EventArgs> ModifierDeleted;
        public IList<string> ModifierStrings { get; set; }
        public string SelectedModifier { get; set; }

        public void Delete()
        {
            if (ModifierDeleted != null)
            {
                ModifierDeleted(this,new EventArgs());
            }
        }
    }
}
