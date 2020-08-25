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
            int? speakerId = null;
            bool isApproved = false;
            var technologies = new List<string> { "Cobol", "Punch Cards", "Commodore", "VBScript" };
            var domains = new List<string> { "aol.com", "prodigy.com", "compuserve.com" };

			if (!string.IsNullOrWhiteSpace(FirstName))
			{
				if (!string.IsNullOrWhiteSpace(LastName))
				{
					if (!string.IsNullOrWhiteSpace(Email))
					{
                        var employers = new List<string>() { "Pluralsight", "Microsoft", "Google" };
                        var isValid = Experience > 10 || HasBlog || Certifications.Count() > 3 || employers.Contains(Employer);

						if (!isValid)
                        {
                            
                            isValid = Email_IsValid(domains, isValid);
                        }

						if (isValid)
						{
                          
                            if (Sessions.Count != 0)
                            {
                                isApproved = Session_IsApproved(technologies, isApproved);
                            }
							else
							{
								return new RegisterResponse(RegisterError.NoSessionsProvided);
							}

							if (isApproved)
							{
				
                                RegistrationFee = Experience <= 1 ? 500
                                    : Experience >= 2 && Experience <= 3 ? 250
                                    : Experience >= 4 && Experience <= 5 ? 100
                                    : Experience >= 6 && Experience <= 9 ? 50
                                    : 0;
                                try
								{
									speakerId = repository.SaveSpeaker(this);
								}
								catch (Exception e)
								{
									//in case the db call fails 
								}
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
            
			return new RegisterResponse((int)speakerId);
		}

        private bool Session_IsApproved(List<string> technologies, bool isApproved)
        {
            foreach (var session in Sessions)
            {
                foreach (var tech in technologies)
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

            return isApproved;
        }

        private bool Email_IsValid(List<string> domains, bool isValid)
        {
            string emailDomain = Email.Split('@').Last();

            if (!domains.Contains(emailDomain) &&
                (!(Browser.Name == WebBrowser.BrowserName.InternetExplorer && Browser.MajorVersion < 9)))
            {
                isValid = true;
            }

            return isValid;
        }
    }
}