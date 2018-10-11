using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvvMobile.ViewModels.Base;
using Xamarin.Forms;

namespace EvvMobile.ViewModels.Messages
{
    public class MessageViewModel : BaseViewModel
    {
        public string Id { get; set; }
        public string MessageTitle { get; set; }
        public string Message { get; set; }
        public IList<string> RecipientIds { get; set; }//staff 
        public string SenderName { get; set; }
        public string Category { get; set; }//red,blue,green... or assign/unassign....
        private const string IsViewedPropertyName = "IsViewed";
        private bool _isViewed;

        public bool IsViewed
        {
            get { return _isViewed; }
            set { SetProperty(ref _isViewed, value, IsViewedPropertyName); }
        }
        public DateTimeOffset SentDateTime { get; set; }


        #region Commands
        Command _saveMessageCommand;
        /// <summary>
        /// download next one month's messages
        /// </summary>
        public Command SaveMessageCommand
        {
            get { return _saveMessageCommand ?? (_saveMessageCommand = new Command(async () => await ExecuteSaveMessageCommand(), () => !IsBusy&& !IsViewed)); }
        }

        async Task ExecuteSaveMessageCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            try
            {
                SaveMessageCommand.ChangeCanExecute();
                IsViewed = true;//update the flag
                //TODO:save tp DB
            }
            finally
            {
                IsBusy = false;
                SaveMessageCommand.ChangeCanExecute();
            }


        }
      
        #endregion  
    }
}
