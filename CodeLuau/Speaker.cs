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
		public string BlogUrl { get; set; }
		public WebBrowser Browser { get; set; }
		public List<string> Certifications { get; set; }
		public string Employer { get; set; }
		public int RegistrationFee { get; set; }
		public List<Session> Sessions { get; set; }

		/// <summary>
		/// Register a speaker
		/// </summary>
		/// <returns>speakerID</returns>
		public RegisterSpeaker Register(IRepository repository)
		{


            var response= Register_RegisterResponse();
            if(response !=null)
                 return response;


            var employers = new List<string>() { "Pluralsight", "Microsoft", "Google" };

            bool meetsCriteria = Experience > 10 || HasBlog || Certifications.Count() > 3 || employers.Contains(Employer);

            if (!meetsCriteria)
            {
                meetsCriteria = FilterByDomain();
            }

            if (!meetsCriteria)
                return new RegisterSpeaker(RegisterError.SpeakerDoesNotMeetStandards);
            


            var technologies = new List<string>() { "Cobol", "Punch Cards", "Commodore", "VBScript" };

            var isApproved = Register_IsApproved(technologies);

            if (!isApproved)
                return new RegisterSpeaker(RegisterError.NoSessionsApproved);

                    
            CalculateRegistrationFee();
            int? speakerId = repository.SaveSpeaker(this);
            return new RegisterSpeaker((int)speakerId);
		}

        private RegisterSpeaker Register_RegisterResponse()
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
            bool meetsCriteria=false;
            string emailDomain = GetEmailDomain();

            var domains = new List<string>() {"aol.com", "prodigy.com", "compuserve.com"};

            if (!domains.Contains(emailDomain) &&
                (!(Browser.Name == WebBrowser.BrowserName.InternetExplorer && Browser.MajorVersion < 9)))
            {
                meetsCriteria = true;
            }

            return meetsCriteria;
        }

        private bool Register_IsApproved(List<string> technologies)
        {
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

        private string GetEmailDomain()
        {
            return Email.Split('@').Last();
        }

        private bool SesionContainsTechnology(Session session, string technology)
        {
            return session.Title.Contains(technology) || session.Description.Contains(technology);
        }

        private void CalculateRegistrationFee()
        { //More experienced speakers pay a lower fee.
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
    }
}