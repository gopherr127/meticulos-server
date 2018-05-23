using Meticulos.Api.App.Items;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Meticulos.Api.App.WorkflowFunctions.PostFunctions
{
    public class SendEmailPostFunction : WorkflowFunction, IWorkflowFunction
    {
        private readonly Item _item;

        public SendEmailPostFunction(Item item)
        {
            _item = item;
        }

        public Task<OperationResult<bool>> Execute(string argsObject)
        {
            return Task.Run(() =>
            {
                SendEmailPostFunctionArgs args =
                    JsonConvert.DeserializeObject<SendEmailPostFunctionArgs>(argsObject);

                if (args == null || string.IsNullOrEmpty(args.EmailAddresses))
                {
                    return new OperationResult<bool>()
                    {
                        Value = false,
                        ErrorMessage = "Post-function is not configured correctly."
                    };
                }

                SmtpClient client = new SmtpClient("smtp.sendgrid.net", 465);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("azure_5bd33395d4838ef91380e73ef7d2fa32@azure.com", "kd93W8K3UFo2ITHi3FB3I1");

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("noreply@meticulos.com");

                foreach (string address in args.EmailAddresses.Split(','))
                    mailMessage.To.Add(address.Trim());

                mailMessage.Subject = $"[Meticulos] {_item.Type.Name}: {_item.Name}";
                mailMessage.Body = JsonConvert.SerializeObject(_item);
                client.Send(mailMessage);

                return new OperationResult<bool>() { Value = true };
            });
        }
    }

    public class SendEmailPostFunctionArgs
    {
        public string EmailAddresses { get; set; }
    }
}
