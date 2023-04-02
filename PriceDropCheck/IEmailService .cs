namespace PriceDropCheck
{
    public interface IEmailService
    {
        void SendEmail(string body, string subject);
    }
}
