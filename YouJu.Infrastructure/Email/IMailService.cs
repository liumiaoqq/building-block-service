using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouJu.Infrastructure.Email
{
    public  interface IMailService
    {
        public  Task SendEmailAsync(MailRequest mailRequest);
        
        }
}
