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
        /// Calculate fee based on speaker's experience
        /// </summary>
        /// <param name="experience">How much experience in years the speaker has</param>
        /// <returns>Fee for the speaker</returns>
        
        public int CalculateFee(int? experience)
        {
            return experience switch
            {
                int n when (n <= 1) => 500,
                int n when (n >= 2 && n <= 3) => 250,
                int n when (n >= 4 && n <= 5) => 100,
                int n when (n >= 6 && n <= 9) => 50,
                _ => 0,
            };
        }

        /// <summary>
        /// Check if a speaker has any sessions about any of the technologies in the list
        /// </summary>
        /// <param name="sessions">Sessions where to perform search</param>
        /// <param name="technologies">List of technologies to look for</param>
        /// <returns>True if there are no sessions for the technologies in the list</returns>
        public bool HasListedTechs(List<Session> sessions, List<string> technologies)
        {
            foreach (var session in sessions)
            {
                foreach (var tech in technologies)
                {
                    if (session.Title.Contains(tech) || session.Description.Contains(tech))
                    {
                        session.Approved = false;
                        return false;
                    }
                    else
                    {
                        session.Approved = true;
                    }
                }
            }
            return true;
        }

        /// <summary>
		/// Register a speaker
		/// </summary>
		/// <returns>speakerID</returns>

		public RegisterResponse Register(IRepository repository)
		{
			int? speakerId = null;
			bool speakerStandart; //speaker meets standarts
			bool speakerApproved = false;
			var oldTech = new List<string>() { "Cobol", "Punch Cards", "Commodore", "VBScript" };
			var domains = new List<string>() { "aol.com", "prodigy.com", "compuserve.com" };
            var employers = new List<string>() { "Pluralsight", "Microsoft", "Google" };
            string emailDomain = Email.Split('@').Last();

            if (string.IsNullOrWhiteSpace(FirstName))
				return new RegisterResponse(RegisterError.FirstNameRequired);
			if (string.IsNullOrWhiteSpace(LastName))
				return new RegisterResponse(RegisterError.LastNameRequired);
			if (string.IsNullOrWhiteSpace(Email))
				return new RegisterResponse(RegisterError.EmailRequired);

			speakerStandart = Exp > 10 || HasBlog || Certifications.Count() > 3 || employers.Contains(Employer);

			if (!domains.Contains(emailDomain) && !(Browser.Name == WebBrowser.BrowserName.InternetExplorer && Browser.MajorVersion < 9))
			{
				speakerStandart = true;
			}

            if (!speakerStandart)
            {
                return new RegisterResponse(RegisterError.SpeakerDoesNotMeetStandards);
            }
            else
            {
                if (Sessions.Count() == 0)
                {
                    return new RegisterResponse(RegisterError.NoSessionsProvided);
                }
                else
                {
                    speakerApproved = HasListedTechs(Sessions, oldTech);
                }

                if (speakerApproved)
                {
                    RegistrationFee = CalculateFee(Exp);
                    //save the speaker and sessions to the db.
                    try
                    {
                        speakerId = repository.SaveSpeaker(this);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{e.Message} has occured, cannot save data");
                    }
                }
                else
                {
                    return new RegisterResponse(RegisterError.NoSessionsApproved);
                }
            }
            return new RegisterResponse((int)speakerId);
		}
	}
}