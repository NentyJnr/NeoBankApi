using System.ComponentModel;

namespace NeoBank.Responses
{
    public class BasicResponse
    {
        [DefaultValue(true)]
        public bool IsSuccessful { get; set; }
        [DefaultValue(null)]
        public ErrorResponse Error { get; set; }

        public BasicResponse()
        {
            IsSuccessful = false;
        }
        public BasicResponse(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }



    }
}
