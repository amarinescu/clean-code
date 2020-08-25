namespace CodeLuau
{
	public class RegisterSpeaker
	{
		public RegisterSpeaker(int speakerId)
		{
			this.SpeakerId = speakerId;
		}

		public RegisterSpeaker(RegisterError? error)
		{
			this.Error = error;
		}

		public int? SpeakerId { get; set; }
		public RegisterError? Error { get; set; }
	}

	public enum RegisterError
	{
		FirstNameRequired,
		LastNameRequired,
		EmailRequired,
		NoSessionsProvided,
		NoSessionsApproved,
		SpeakerDoesNotMeetStandards
	};
}
