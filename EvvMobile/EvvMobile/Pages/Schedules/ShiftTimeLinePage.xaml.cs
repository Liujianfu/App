using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evv.Message.Portable.Schedulers.Dtos;
using EvvMobile.Pages.Base;
using EvvMobile.ViewModels.Schedules;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EvvMobile.Pages.Schedules
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShiftTimeLinePage : ShiftTimeLinePageXaml
    {
        public ShiftTimeLinePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!ViewModel.Notes.Any() ||
                !ViewModel.Notes.Last().Category.StartsWith("Paid", StringComparison.OrdinalIgnoreCase))
            {
                var parmentDue = DateTime.Now.AddDays(14);
                if (ViewModel.Notes.Any())
                {
                    parmentDue = ViewModel.Notes.Last().NoteTime.DateTime.AddDays(14);
                }
                for (var i = 0; i <= 3; i++)
                {
                    ViewModel.Notes.Add(new ReasonsCommentsDto
                    {
                        Content = "",
                        Name = "Placeholder"
                    });
                }
                ViewModel.Notes.Add(new ReasonsCommentsDto
                {
                    Content = "Estimated payment due date",
                    Name = "MMIS",
                    NoteTime = parmentDue,
                    SubKey = "Paid"
                });
            }

        }
    }
    public abstract class ShiftTimeLinePageXaml : ModelBoundWithHomeButtonContentPage<NotesViewModel> { }
}