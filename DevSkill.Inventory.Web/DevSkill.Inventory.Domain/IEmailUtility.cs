using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Domain
{
    public interface IEmailUtility
    {
        void SendEmail(string receiverEmail, string receiverName, string subject, string body);
    }
}
