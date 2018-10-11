using System;
using EvvMobile.ViewModels.Base;

namespace EvvMobile.ViewModels.Clients
{
    public class ClientProfileViewModel : BaseViewModel
    {
        #region Properties

        #region personal info
        public string ClientId { get; set; }
        public string ClientPictureBase64Img { get; set; }
        public string ClientMaNumber { get; set; }
        public string ClientFullName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public int Age
        {
            get
            {
                return  (DateOfBirth!=null)? 0 : (DateTime.Now.Year - DateOfBirth.Value.Year);              
            }
            set
            {
                Age = (DateOfBirth != null)? 0 : (DateTime.Now.Year - DateOfBirth.Value.Year);
            }
        }

        public string MaritalStatus { get; set; }
        public string Gender { get; set; }
        public string Race { get; set; }
        public bool? IsHispanicLatino { get; set; }

        public bool? IsEnglishSpeaking { get; set; }

        public string OtherLanguageSpecify { get; set; }
        public string OtherRaceSpecify { get; set; }
        public string PrimaryLanguage { get; set; }
        #endregion

        #region living status
        public string FacilityAddress { get; set; }
        public string FacilityName { get; set; }
        public bool IsLivingWithFamily { get; set; }
        public string LivingStatus { get; set; }


        #endregion


        #region contact info
        public string CurrentAddress { get; set; }
        public string CountySpecify { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }


        #endregion

        #region others
        public string CurrentGuardianOfPerson { get; set; }

        public string CurrentSurrogate { get; set; }

        public string CurrentRepresentativePayee { get; set; }

        public string CurrentPowerOfAttorneyContact { get; set; }

        public string CurrentDurablePowerOfAttorneyContact { get; set; }

        public string CurrentCasemanagerOrServiceCoordinatorContact { get; set; }

        public string CurrentPhysician { get; set; }
        public string Comment { get; set; }


        #endregion


        #region emergency contacts

        public string CurrentEmergencyContact { get; set; }
        public string CurrentEmergencyContactPhoneNumber { get; set; }
        public string CurrentEmergencyContactRelation { get; set; }
        public string SecondEmergencyContact { get; set; }
        public string SecondEmergencyContactPhoneNumber { get; set; }
        public string SecondEmergencyContactRelation { get; set; }
        #endregion

        #endregion

        #region Commands


        #endregion

        #region Private methods
        
        #endregion

        #region Fields



        #endregion
    }
}
