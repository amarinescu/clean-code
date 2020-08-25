namespace CodeLuau
{
    public class RegisterResponse
	{
		public int? SpeakerId { get; set; }
		public RegisterError? Error { get; set; }


		public RegisterResponse(int speakerId)
		{
			this.SpeakerId = speakerId;
		}

		public RegisterResponse(RegisterError? error)
		{
			this.Error = error;
		}

	}
}
