using HealthCare.Common.Models;

namespace HealthCare.API.Helpers
{
    public interface IMailHelper
    {
        Response SendMail(string to, string subject, string body);
    }
}
