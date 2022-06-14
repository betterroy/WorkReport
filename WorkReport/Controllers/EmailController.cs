using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WorkReport.Commons.EmailHelper;
using WorkReport.Utility.Filters.Attributes;

namespace WorkReport.Controllers
{
    public class EmailController : Controller
    {
        private readonly SendServerConfigurationEntity _sendServerConfigurationEntity;
        public EmailController(IOptionsMonitor<SendServerConfigurationEntity> sendServerConfigurationEntity)
        {
            _sendServerConfigurationEntity = sendServerConfigurationEntity.CurrentValue;
        }


        [CustomAllowAnonymousAttribute]
        public IActionResult Index()
        {
            var mailBodyEntity = new MailBodyEntity()
            {
                Subject = "说点啥当做标题吧！！！",
                Body = "邮件内容：<BR>博客：<a href='https://blog.csdn.net/hello_mr_anan?type=blog'>这是小王的个人主页</a>",
                Recipients = new List<string>() { "***@163.com","***@qq.com"},      //收件人
                SenderAddress = _sendServerConfigurationEntity.SenderAccount,     //发件人
            };
            
            var result = MailHelper.SendMail(mailBodyEntity, _sendServerConfigurationEntity);

            return new JsonResult(result);
        }
    }
}
