using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeLuau
{
    /// <summary>
    /// Represents a single speaker
    /// </summary>
    public class Speaker
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? Experience { get; set; }
        public bool HasBlog { get; set; }
        public string BlogURL { get; set; }
        public WebBrowser Browser { get; set; }
        public List<string> Certifications { get; set; }
        public string Employer { get; set; }
        public int RegistrationFee { get; set; }
        public List<Session> Sessions { get; set; }

        public Speaker(string firstName, string lastName, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }


        List<string> notAllowedSessions = new List<string>() { "Cobol", "Punch Cards", "Commodore", "VBScript" };
        List<string> notAllowedDomains = new List<string>() { "aol.com", "prodigy.com", "compuserve.com" };
        List<string> employers = new List<string>() { "Pluralsight", "Microsoft", "Google" };


        /// <summary>
        /// Register a speaker
        /// </summary>
        /// <returns>speakerID</returns>
        public RegisterResponse Register(IRepository repository)
        {
            if (string.IsNullOrWhiteSpace(FirstName))
                return new RegisterResponse(RegisterError.FirstNameRequired);
            if (string.IsNullOrWhiteSpace(LastName))
                return new RegisterResponse(RegisterError.LastNameRequired);
            if (string.IsNullOrWhiteSpace(Email))
                return new RegisterResponse(RegisterError.EmailRequired);

            bool isEnough = EnoughExpirienced(notAllowedDomains, employers);

            if (!isEnough)
                return new RegisterResponse(RegisterError.SpeakerDoesNotMeetStandards);

            if (Sessions.Count() <= 0)
                return new RegisterResponse(RegisterError.NoSessionsProvided);

            var isApproved = SessionApproving(notAllowedSessions);

            if (!isApproved)
                return new RegisterResponse(RegisterError.NoSessionsApproved);

            CountingRegistrationFee();
            
            return new RegisterResponse(SaveSpeaker(repository));
        }

        private bool EnoughExpirienced(List<string> notAllowedDomains, List<string> employers)
        {
            var good = Experience > 10 || HasBlog || Certifications.Count() > 3 || employers.Contains(Employer);

            if (!good)
            {
                string emailDomain = Email.Split('@').Last();

                if (!notAllowedDomains.Contains(emailDomain) && !(Browser.Name == WebBrowser.BrowserName.InternetExplorer && Browser.MajorVersion < 9))
                {
                    good = true;
                }
            }

            return good;
        }

        private void CountingRegistrationFee()
        {
            if (Experience <= 1)
                RegistrationFee = 500;

            else if (Experience >= 2 && Experience <= 3)
                RegistrationFee = 250;

            else if (Experience >= 4 && Experience <= 5)
                RegistrationFee = 100;

            else if (Experience >= 6 && Experience <= 9)
                RegistrationFee = 50;

            else
                RegistrationFee = 0;
        }

        private int SaveSpeaker(IRepository repository)
        {
            try
            {
                return repository.SaveSpeaker(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }

        private bool SessionApproving(List<string> restrictedSessions)
        {
            foreach (var session in Sessions)
            {
                foreach (var tech in restrictedSessions)
                {
                    if (session.Title.Contains(tech) || session.Description.Contains(tech))
                    {
                        session.Approved = false;
                        break;
                    }
                    else
                    {
                        session.Approved = true;
                    }
                }
            }

            return Sessions.Any(x => x.Approved);
        }
    }
}