namespace BookingSalon.Services.Interface
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
