using System.Collections.Generic;
using System.Linq;

namespace CodeLuau
{
    public class Speaker
    {
        #region Properties

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Experience { get; set; }
        public bool HasBlog { get; set; }
        public string BlogUrl { get; set; }
        public WebBrowser Browser { get; set; }
        public List<string> Certifications { get; set; }
        public string Employer { get; set; }
        public int RegistrationFee { get; set; }
        public List<Session> Sessions { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Register a speaker
        /// </summary>
        /// <returns>speakerID or specific error</returns>
        public RegisterResponse Register(IRepository repository)
        {
            int returnSpeakerId;

            var techniquesList = new List<string>() { "Cobol", "Punch Cards", "Commodore", "VBScript" };

            //DEFECT #5274 DA 12/10/2012
            //We weren't filtering out the prodigy domain so I added it.
            var domainsList = new List<string>() { "aol.com", "prodigy.com", "compuserve.com" };

            if (!string.IsNullOrWhiteSpace(FirstName))
            {
                if (!string.IsNullOrWhiteSpace(LastName))
                {
                    if (!string.IsNullOrWhiteSpace(Email))
                    {
                        var employerList = new List<string>() { "Pluralsight", "Microsoft", "Google" };

                        var isSpeakerMeetStandards = Experience > 10 || HasBlog || Certifications.Count() > 3 || employerList.Contains(Employer);

                        //if Speaker does not meet standards, then check by Domain and browser version
                        if (!isSpeakerMeetStandards)
                        {
                            CheckIfSpeakerMeetStandardsByDomainAndBrowser(domainsList, ref isSpeakerMeetStandards);
                        }

                        if (isSpeakerMeetStandards)
                        {
                            bool isApproved = false;

                            if (Sessions.Any())
                            {
                                CheckIfIsApproved(techniquesList, ref isApproved);
                            }
                            else
                            {
                                return new RegisterResponse(RegisterError.NoSessionsProvided);
                            }

                            if (isApproved)
                            {
                                CalculateRegistrationFee();

                                returnSpeakerId = repository.SaveSpeaker(this);
                            }
                            else
                            {
                                return new RegisterResponse(RegisterError.NoSessionsApproved);
                            }
                        }
                        else
                        {
                            return new RegisterResponse(RegisterError.SpeakerDoesNotMeetStandards);
                        }
                    }
                    else
                    {
                        return new RegisterResponse(RegisterError.EmailRequired);
                    }
                }
                else
                {
                    return new RegisterResponse(RegisterError.LastNameRequired);
                }
            }
            else
            {
                return new RegisterResponse(RegisterError.FirstNameRequired);
            }

            return new RegisterResponse(returnSpeakerId);
        }

        #endregion

        #region Private Methods

        private void CheckIfIsApproved(List<string> techniquesList, ref bool isApproved)
        {
            foreach (var session in Sessions)
            {
                foreach (var tech in techniquesList)
                {
                    if (session.Title.Contains(tech) || session.Description.Contains(tech))
                    {
                        session.Approved = false;
                        break;
                    }
                    else
                    {
                        session.Approved = true;
                        isApproved = true;
                    }
                }
            }
        }

        private void CalculateRegistrationFee()
        {
            if (Experience <= 1)
            {
                RegistrationFee = 500;
            }
            else if (Experience >= 2 && Experience <= 3)
            {
                RegistrationFee = 250;
            }
            else if (Experience >= 4 && Experience <= 5)
            {
                RegistrationFee = 100;
            }
            else if (Experience >= 6 && Experience <= 9)
            {
                RegistrationFee = 50;
            }
            else
            {
                RegistrationFee = 0;
            }
        }

        private void CheckIfSpeakerMeetStandardsByDomainAndBrowser(List<string> domainsList, ref bool isSpeakerMeetStandards)
        {
            //need to get just the domain from the email
            string emailDomain = Email.Split('@').Last();

            if (!domainsList.Contains(emailDomain) &&
                (!(Browser.Name == WebBrowser.BrowserName.InternetExplorer && Browser.MajorVersion < 9)))
            {
                isSpeakerMeetStandards = true;
            }
        }

        #endregion
    }
}