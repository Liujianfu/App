using EvvMobile.Authorization;
using EvvMobile.DataService.Services;
using EvvMobile.ViewModels.Base;
using EvvMobile.DataService.Interfaces;
namespace EvvMobile.ViewModels.Systems
{
    public class SystemViewModel : BaseViewModel
    {
        private SystemViewModel()
        {
            _schedulerService = new SchedulerService(Statics.AppConifgurations.ScheduleServiceBaseUrl);
            //TODO: remove this
            CurrentStaffId = "staffs/91ead1d3-59bd-4ad7-b57d-eb0976061e51";

            ///////////////
        }

        public static readonly SystemViewModel Instance = new SystemViewModel();

        public string UserName { get; set; }

        public string Password { get; set; }

        public string CurrentStaffId { get; set; }

        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }

            set
            {
                if (errorMessage != value)
                {
                    errorMessage = value;
                    OnPropertyChanged("ErrorMessage");
                }
            }
        }

        public string InfoMessage
        {
            get
            {
                return infoMessage;
            }

            set
            {
                if (infoMessage != value)
                {
                    infoMessage = value;
                    OnPropertyChanged("InfoMessage");
                }
            }
        }

        public bool HasNetworkConnection
        {
            get
            {
                return hasNetworkConnection;
            }

            set
            {
                if (hasNetworkConnection != value)
                {
                    hasNetworkConnection = value;
                    OnPropertyChanged("HasNetworkConnection");
                }
            }
        }

        public bool IsClientLogin
        {
            get { return _isClientLogin; }
            set
            {
                if (_isClientLogin != value)
                {
                    _isClientLogin = value;
                    OnPropertyChanged("IsClientLogin");
                    OnPropertyChanged("IsStaffLogin");
                }
            }
        }
        public bool IsStaffLogin
        {
            get { return !_isClientLogin; }

        }
        public double CurrentLatitude { get; set; }

        public double CurrentLongitude { get; set; }

        public bool InitialLocationCaptured { get; set; }

        public bool HasGps { get; set; }
        public IdentityServerResourceOwerAuthenticator IdentityServerAuthenticator { get; set; }

        public ISchedulerService SchedulerDataService
        {
            get { return _schedulerService; }
        }
        public void CleanMessages()
        {
            InfoMessage = null;
            ErrorMessage = null;
        }


        private string errorMessage;

        private string infoMessage;

        private bool hasNetworkConnection;
        private bool _isClientLogin;
        private static SystemViewModel systemViewModel;
        private ISchedulerService _schedulerService;
        private static object locker = new object();
    }
}
