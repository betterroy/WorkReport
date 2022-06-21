using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WorkReport.Commons.EmailHelper;
using WorkReport.Commons.RedisHelper.Service;
using WorkReport.Models.Query;
using WorkReport.Utility.Filters.Attributes;

namespace WorkReport.Controllers
{
    public class EmailController : Controller
    {
        private readonly SendServerConfigurationEntity _sendServerConfigurationEntity;
        private readonly RedisStringService _RedisStringService;

        public EmailController(IOptionsMonitor<SendServerConfigurationEntity> sendServerConfigurationEntity,
            RedisStringService redisStringService)
        {
            _sendServerConfigurationEntity = sendServerConfigurationEntity.CurrentValue;
            _RedisStringService = redisStringService;

            InitSendServerConfigurationEntity();
        }

        /// <summary>
        /// 如果config中为空，则从Redis中取，为防止上传至github私密信息泄露。
        /// </summary>
        public void InitSendServerConfigurationEntity()
        {
            if (string.IsNullOrEmpty(_sendServerConfigurationEntity.SenderPassword))
            {
                string emailKey = "WorkReport.SendServerEmailConfigurationEntity";
                var redisSemail = _RedisStringService.Get<SendServerConfigurationEntity>(emailKey);
                _sendServerConfigurationEntity.SenderAccount = redisSemail.SenderAccount;
                _sendServerConfigurationEntity.SenderPassword = redisSemail.SenderPassword;
            }
        }
        public IActionResult Index()
        {
            return View();
        }

        [CustomAllowAnonymousAttribute]
        public IActionResult Test()
        {
            var mailBodyEntity = new MailBodyEntity()
            {
                Subject = "说点啥当做标题吧！！！",
                Body = "邮件内容：<BR>博客：<a href='https://blog.csdn.net/hello_mr_anan?type=blog'>这是小王的个人主页</a>",
                Recipients = new List<string>() { "betterroy@163.com", "1121695511@qq.com" },      //收件人
                SenderAddress = _sendServerConfigurationEntity.SenderAccount,     //发件人
            };

            var result = MailHelper.SendMail(mailBodyEntity, _sendServerConfigurationEntity);

            return new JsonResult(result);
        }

        public IActionResult SendEmail([FromBody]SEmailQuery sEmailQuery)
        {
            sEmailQuery.recipients = sEmailQuery.recipients.Replace("，", ",");
            var mailBodyEntity = new MailBodyEntity()
            {
                Subject = sEmailQuery.subject,
                Body = sEmailQuery.body,
                Recipients = sEmailQuery.recipients.Split(",").ToList(),      //收件人
                SenderAddress = _sendServerConfigurationEntity.SenderAccount,     //发件人
            };

            var result = MailHelper.SendMail(mailBodyEntity, _sendServerConfigurationEntity);

            return new JsonResult(result);
        }
    }
}
