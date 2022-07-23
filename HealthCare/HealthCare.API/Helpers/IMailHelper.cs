using HealthCare.Common.Models;

namespace HealthCare.API.Helpers
{
    public interface IMailHelper
    {
        Response SendMail(string toName, string toEmail, string subject, string body);

    }
}
