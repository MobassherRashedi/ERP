namespace DevSkill.Inventory.Web.Models
{
    public interface IEmailSender
    {
        public void SendEmail(string email, string subject, string body);
    }
}
