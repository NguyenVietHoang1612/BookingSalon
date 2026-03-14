using BookingSalon.Services.Interface;
using Twilio;
using Twilio.Rest.Api.V2010.Account; 

namespace BookingSalon.Services
{
    public class TwilioSmsSender : ISmsSender
    {
        public async Task SendSmsAsync(string number, string message)
        {
            var cleanNumber = number.Replace(" ", "");

            if (cleanNumber.StartsWith("0"))
            {
                cleanNumber = "+84" + cleanNumber.Substring(1);
            }

            System.Diagnostics.Debug.WriteLine($"DEBUG NUMBER: {cleanNumber}");

            var accountSid = "";
            var authToken = "";

            TwilioClient.Init(accountSid, authToken);

            await MessageResource.CreateAsync(
                body: message,
                from: new Twilio.Types.PhoneNumber(""),
                to: new Twilio.Types.PhoneNumber(cleanNumber)
            );
        }
    }
}
