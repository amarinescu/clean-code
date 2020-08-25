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

        // lets init some vars
        private int? _speakerId;
        private bool _good;
        private bool _isApproved;


        /// <summary>
        /// Register a speaker
        /// </summary>
        /// <returns>speakerID</returns>
        public RegisterResponse Register(IRepository repository)
        {
            //var nt = new List<string> {"Node.js", "Docker"};
            var ot = new List<string>() { "Cobol", "Punch Cards", "Commodore", "VBScript" };

            //DEFECT #5274 DA 12/10/2012
            //We weren't filtering out the prodigy domain so I added it.
            var domains = new List<string>() { "aol.com", "prodigy.com", "compuserve.com" };

            if (!string.IsNullOrWhiteSpace(FirstName))
            {
                if (!string.IsNullOrWhiteSpace(LastName))
                {
                    if (!string.IsNullOrWhiteSpace(Email))
                    {
                        //put list of employers in array
                        var emps = new List<string>() { "Pluralsight", "Microsoft", "Google" };

                        _good = Experience > 10 || HasBlog || Certifications.Count() > 3 || emps.Contains(Employer);

                        GetEmailDomain(domains);

                        if (_good)
                        {
                            if (Sessions.Count() <= 0)
                            {
                                return new RegisterResponse(RegisterError.NoSessionsProvided);
                            }
                            else
                            {
                                SessionApproving(ot);
                            }

                            if (_isApproved)
                            {
                                //if we got this far, the speaker is approved
                                //let's go ahead and register him/her now.
                                //First, let's calculate the registration fee. 
                                //More experienced speakers pay a lower fee.
                                CountingRegistrationFee();


                                //Now, save the speaker and sessions to the db.
                                SessionsSaving(repository);
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

            //if we got this far, the speaker is registered.
            return new RegisterResponse((int)_speakerId);
        }

        private void CountingRegistrationFee()
        {
            if (Experience <= 1)
                RegistrationFee = 500;

            else
                if (Experience >= 2 && Experience <= 3)
                RegistrationFee = 250;

            else if (Experience >= 4 && Experience <= 5)
                RegistrationFee = 100;

            else if (Experience >= 6 && Experience <= 9)
                RegistrationFee = 50;

            else
                RegistrationFee = 0;
        }

        private void SessionsSaving(IRepository repository)
        {
            try
            {
                _speakerId = repository.SaveSpeaker(this);
            }
            catch (Exception e)
            {
                //in case the db call fails 
            }
        }

        private void SessionApproving(List<string> ot)
        {
            foreach (var session in Sessions)
            {
                //foreach (var tech in nt)
                //{
                //    if (session.Title.Contains(tech))
                //    {
                //        session.Approved = true;
                //        break;
                //    }
                //}

                foreach (var tech in ot)
                {
                    if (session.Title.Contains(tech) || session.Description.Contains(tech))
                    {
                        session.Approved = false;
                        break;
                    }
                    else
                    {
                        session.Approved = true;
                        _isApproved = true;
                    }
                }
            }
        }

        private void GetEmailDomain(List<string> domains)
        {
            if (!_good)
            {
                //need to get just the domain from the email
                string emailDomain = Email.Split('@').Last();

                if (!domains.Contains(emailDomain) && (!(Browser.Name == WebBrowser.BrowserName.InternetExplorer && Browser.MajorVersion < 9)))
                {
                    _good = true;
                }
            }
        }

    }
}