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
		public int? Exp { get; set; }
		public bool HasBlog { get; set; }
		public string BlogURL { get; set; }
		public WebBrowser Browser { get; set; }
		public List<string> Certifications { get; set; }
		public string Employer { get; set; }
		public int RegistrationFee { get; set; }
		public List<Session> Sessions { get; set; }

		/// <summary>
		/// Register a speaker
		/// </summary>
		/// <returns>speakerID</returns>
		public RegisterResponse Register(IRepository repository)
		{
			// lets init some vars
			int? speakerId = null;
            bool appr = false;
			//var nt = new List<string> {"Node.js", "Docker"};
			var ot = new List<string>() { "Cobol", "Punch Cards", "Commodore", "VBScript" };

			//DEFECT #5274 DA 12/10/2012
			//We weren't filtering out the prodigy domain so I added it.
			var domains = new List<string>() { "aol.com", "prodigy.com", "compuserve.com" };

            if (string.IsNullOrWhiteSpace(FirstName))            
                return new RegisterResponse(RegisterError.FirstNameRequired);
            
            if (string.IsNullOrWhiteSpace(LastName))
                return new RegisterResponse(RegisterError.LastNameRequired);

            if (string.IsNullOrWhiteSpace(Email))
                return new RegisterResponse(RegisterError.EmailRequired);


            //put list of employers in array
            List<string> emps = EmployersToArray();

                        bool good = Exp > 10 || HasBlog || Certifications.Count() > 3 || emps.Contains(Employer);

                        if (!good)
                        {

                            good = GetDomain(good, domains);

                        }

                        if (good)
                        {

                            if (!IsSession())
                                return new RegisterResponse(RegisterError.NoSessionsProvided);

                            appr = CheckAbleSession(appr, ot);

                            if (!appr)
                                return new RegisterResponse(RegisterError.NoSessionsApproved);

                            speakerId = SaveSpeaker(repository);


                        }
                        else
                        {
                            return new RegisterResponse(RegisterError.SpeakerDoesNotMeetStandards);
                        }


			


			//if we got this far, the speaker is registered.
			return new RegisterResponse((int)speakerId);
		}

        private global::System.Int32? SaveSpeaker(IRepository repository)
        {
            int? speakerId;
            FeeCalculation();

            speakerId = repository.SaveSpeaker(this);
            return speakerId;
        }

        private bool CheckAbleSession(bool appr, List<string> ot)
        {
            foreach (var session in Sessions)
            {
                foreach (var tech in ot)
                {
                    bool isTechInTitle = session.Title.Contains(tech);
                    bool isTechInDescription = session.Description.Contains(tech);
                    
                    if (isTechInTitle || isTechInDescription)
                    {
                        session.Approved = false;
                        break;
                    }
                    else
                    {
                        session.Approved = true;
                        appr = true;

                    }
                }
            }

            return appr;
        }




        private static List<string> EmployersToArray()
        {
            return new List<string>() { "Pluralsight", "Microsoft", "Google" };
        }

        private bool IsSession()
        {
            return Sessions.Count() != 0;
        }

   

        private void FeeCalculation()
        {
            if (CheckExpRange(0, 1))
            {
                RegistrationFee = 500;
            }
            else if (CheckExpRange(2, 3))
            {
                RegistrationFee = 250;
            }
            else if (CheckExpRange(4, 5))
            {
                RegistrationFee = 100;
            }
            else if (CheckExpRange(6, 9))
            {
                RegistrationFee = 50;
            }
            else
            {
                RegistrationFee = 0;
            }
        }

        private bool CheckExpRange(int inital, int final)
        {
            
            return Exp >= inital && Exp <= final;
        }

        private bool GetDomain(bool good, List<string> domains)
        {
            string emailDomain = Email.Split('@').Last();

            bool hasDomain = domains.Contains(emailDomain);
            bool isInternetExplorer = Browser.Name == WebBrowser.BrowserName.InternetExplorer;
            int maximumVersion = Browser.MajorVersion;

            good = (!hasDomain && (!(isInternetExplorer && maximumVersion < 9)));

            return good;
        }
    }
}