using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evv.Message.Portable.Schedulers.Dtos;
using Evv.Message.Portable.Schedulers.Identifiers;
using EvvMobile.ViewModels.Base;
using EvvMobile.ViewModels.Schedules;
using EvvMobile.ViewModels.Systems;
using Xamarin.Forms;

namespace EvvMobile.ViewModels.Messages
{
    public class MessageListViewModel : BaseViewModel
    {
        public MessageListViewModel()
        {
            MessageList = new ObservableCollection<MessageViewModel>();
        }
        public ObservableCollection<MessageViewModel> MessageList { get; set; }
        public ObservableCollection<MessageViewModel> NewMessageList { get; set; }

        #region Commands
        Command _loadMessagesCommand;
        /// <summary>
        /// download next one month's messages
        /// </summary>
        public Command LoadMessagesCommand
        {
            get { return _loadMessagesCommand ?? (_loadMessagesCommand = new Command(async () => await ExecuteLoadMessagesCommand(), () => !IsBusy)); }
        }

        async Task ExecuteLoadMessagesCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            try
            {
                LoadMessagesCommand.ChangeCanExecute();
                //TODO:get messages from DB
                //testing code
                MessageList.Add(new MessageViewModel
                {
                    MessageTitle = "Assignment Notification",
                    SenderName = "Provider Admin",
                    SentDateTime = DateTimeOffset.Now.AddDays(-1),
                    Message = "A new shift assignment is assigned to you.Using advanced encryption, GPS, cellular and Wi-Fi technology, Axxess' Electronic Visit Verification records the date, time and location of home care visits using phones or tablets on both the Apple iOS or Google Android platforms. Our mobile apps are fully integrated with our software and automatically sync in real time!"
                });
                MessageList.Add(new MessageViewModel
                {
                    MessageTitle = "Reminds",
                    SenderName = "System",
                    SentDateTime = DateTimeOffset.Now.AddDays(-3),
                    Message = "You have a planned visit tomorrow."
                });

            }
            finally
            {
                IsBusy = false;
                LoadMessagesCommand.ChangeCanExecute();
                OnPropertyChanged("MessageList");
            }


        }
        Command _loadNewMessagesCommand;
        /// <summary>
        /// download next one month's messages
        /// </summary>
        public Command LoadNewMessagesCommand
        {
            get { return _loadNewMessagesCommand ?? (_loadNewMessagesCommand = new Command(async () => await ExecuteLoadNewMessagesCommand(), () => !IsBusy)); }
        }

        async Task ExecuteLoadNewMessagesCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            try
            {
                LoadNewMessagesCommand.ChangeCanExecute();

                //TODO:get new messages from DB

                NewMessageList = new ObservableCollection<MessageViewModel>(NewMessageList.Where(x => !x.IsViewed));
                NewMessageList.Add(new MessageViewModel
                {
                    MessageTitle = "Assignment",
                    SenderName = "Provider Admin",
                    SentDateTime = DateTimeOffset.Now.AddDays(-1),
                    Message =
                        "A new shift assignment is assigned to you. testing"
                });
                NewMessageList.Add(new MessageViewModel
                {
                    MessageTitle = "Denied",
                    SenderName = "System",
                    SentDateTime = DateTimeOffset.Now.AddDays(-3),
                    Message = "Your service visit was denied."
                });

            }
            finally
            {
                IsBusy = false;
                LoadNewMessagesCommand.ChangeCanExecute();
                OnPropertyChanged("NewMessageList");
            }


        }
        #endregion  
    }
}
