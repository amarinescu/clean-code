namespace CodeLuau
{
    public class RegisterResponse
    {
        public int? SpeakerId { get; set; }
        public RegisterError? Error { get; set; }


        public RegisterResponse(int speakerId)
        {
            SpeakerId = speakerId;
        }

        public RegisterResponse(RegisterError? error)
        {
            Error = error;
        }

    }
}
