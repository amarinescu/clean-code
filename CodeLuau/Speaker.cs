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
        public string BlogUrl { get; set; }
        public WebBrowser Browser { get; set; }
        public List<string> Certifications { get; set; }
        public string Employer { get; set; }
        public int RegistrationFee { get; set; }

        public List<Session> Sessions { get; set; }


        public RegisterResponse Register(IRepository repository)
        {
            var response = ValidateSpeaker();

            if (response != null)
                return response;


            bool isValid = MeetsStandartsSpeaker() || FilterByDomain();
            if (!isValid)
                return new RegisterResponse(RegisterError.SpeakerDoesNotMeetStandards);

            var isApproved = Session_IsApproved();
            if (!isApproved)
                return new RegisterResponse(RegisterError.NoSessionsApproved);

            RegistrationFee = CalculateRegistrationFee();


            int? speakerId = repository.SaveSpeaker(this);
            return new RegisterResponse((int) speakerId);
        }


        private bool Session_IsApproved()
        {
            var technologies = new List<string>()
            {
                "Cobol",
                "Punch Cards",
                "Commodore",
                "VBScript"
            };
            bool isApproved = false;
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

        private bool FilterByDomain()
        {
            var domains = new List<string>()
            {
                "aol.com",
                "prodigy.com",
                "compuserve.com"
            };
            bool meetsCriteria = false;
            string emailDomain = GetEmailDomain();

            if (!domains.Contains(emailDomain) &&
                (!(Browser.Name == WebBrowser.BrowserName.InternetExplorer && Browser.MajorVersion < 9)))
            {
                meetsCriteria = true;
            }

            return meetsCriteria;
        }

        private RegisterResponse ValidateSpeaker()
        {
            if (string.IsNullOrWhiteSpace(FirstName))
                return new RegisterResponse(RegisterError.FirstNameRequired);

            if (string.IsNullOrWhiteSpace(LastName))
                return new RegisterResponse(RegisterError.LastNameRequired);

            if (string.IsNullOrWhiteSpace(Email))
                return new RegisterResponse(RegisterError.EmailRequired);

            if (!Sessions.Any())
                return new RegisterResponse(RegisterError.NoSessionsProvided);
            return null;
        }

        private bool MeetsStandartsSpeaker()
        {
            var employers = new List<string>()
            {
                "Pluralsight",
                "Microsoft",
                "Google"
            };
            return Experience > 10 || HasBlog || Certifications.Count() > 3 ||
                   employers.Contains(Employer);
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
            var RegistrationFee = Experience <= 1 ? 500
                : Experience >= 2 && Experience <= 3 ? 250
                : Experience >= 4 && Experience <= 5 ? 100
                : Experience >= 6 && Experience <= 9 ? 50
                : 0;

            return RegistrationFee;
        }
    }
}