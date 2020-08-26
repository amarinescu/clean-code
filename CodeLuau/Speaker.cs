using System;
using System.Collections.Generic;
using System.Linq;


namespace CodeLuau
{
    public class Speaker
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? Experience { get; set; }
        public bool HasBlog { get; set; }
        public WebBrowser Browser { get; set; }
        public List<string> Certifications { get; set; }
        public string Employer { get; set; }
        public int RegistrationFee { get; set; }
        public List<Session> Sessions { get; set; }

        public RegisterSpeaker Register(IRepository repository)
        {

            var response = ValidateSpeaker();
            if (response != null)
                return response;

            bool isValid = MeetsStandartsSpeaker() || FilterByDomain();
            if (!isValid)
                return new RegisterSpeaker(RegisterError.SpeakerDoesNotMeetStandards);

            var isApproved = Register_IsApproved();
            if (!isApproved)
                return new RegisterSpeaker(RegisterError.NoSessionsApproved);


            RegistrationFee = CalculateRegistrationFee();
            int? speakerId = repository.SaveSpeaker(this);
            return new RegisterSpeaker((int)speakerId);
        }

        private bool MeetsStandartsSpeaker()
        {
            const int RequiredCertifications = 3; 
            const int RequiredExperience = 10;
            var employers = new List<string>() { "Pluralsight", "Microsoft", "Google" };

            return Experience > RequiredExperience || HasBlog || Certifications.Count() > RequiredCertifications || employers.Contains(Employer);
        }

        private RegisterSpeaker ValidateSpeaker()
        {
            if (string.IsNullOrWhiteSpace(FirstName))
                return new RegisterSpeaker(RegisterError.FirstNameRequired);

            if (string.IsNullOrWhiteSpace(LastName))
                return new RegisterSpeaker(RegisterError.LastNameRequired);

            if (string.IsNullOrWhiteSpace(Email))
                return new RegisterSpeaker(RegisterError.EmailRequired);

            if (!Sessions.Any())
                return new RegisterSpeaker(RegisterError.NoSessionsProvided);
            return null;
        }

        private bool FilterByDomain()
        { 
            const int MaxVersionBrowser = 9;
            bool meetsCriteria = false;
            var domains = new List<string>() { "aol.com", "prodigy.com", "compuserve.com" }; 
            
            string emailDomain = GetEmailDomain();

            if (!domains.Contains(emailDomain) &&
                (!(Browser.Name == WebBrowser.BrowserName.InternetExplorer && Browser.MajorVersion < MaxVersionBrowser)))
            {
                meetsCriteria = true;
            }

            return meetsCriteria;
        }

        private bool Register_IsApproved()
        {
            bool isApproved = false;
            var technologies = new List<string>() { "Cobol", "Punch Cards", "Commodore", "VBScript" }; 

            foreach (Session session in Sessions)
            {
                foreach (string technology in technologies)
                {
                    if (SesionContainsTechnology(session, technology))
                    {
                        session.Approved = false;
                        break;
                    }

                    session.Approved = true;
                    isApproved = true;
                }
            }

            return isApproved;
        }

        private string GetEmailDomain()
        {
            return Email.Split('@').Last();
        }

        private bool SesionContainsTechnology(Session session, string technology)
        {
            return session.Title.Contains(technology) || session.Description.Contains(technology);
        }

        private int CalculateRegistrationFee()
        { 
            const int MinExpLev1 = 1;
            const int MinExpLev2 = 2; 
            const int MinExpLev3 = 3;
            const int MinExpLev4 = 4; 
            const int MinExpLev5 = 5;
            const int MinExpLev6 = 6;
            const int MinExpLev7 = 9;
            const int RegFeeLevel1 = 500;
            const int RegFeeLevel2 = 250;
            const int RegFeeLevel3 = 100;
            const int RegFeeLevel4 = 50;
            const int RegFeeLevel5 = 0;

            if (Experience <= MinExpLev1)
                return RegFeeLevel1;

            if (Experience >= MinExpLev2 && Experience <= MinExpLev3)
                return RegFeeLevel2;

            if (Experience >= MinExpLev4 && Experience <= MinExpLev5)
                return RegFeeLevel3;

            if (Experience >= MinExpLev6 && Experience <= MinExpLev7)
                return RegFeeLevel4;

            return RegFeeLevel5;
        }
    }
}