using Microsoft.AspNetCore.Authorization;

namespace WorkReport.Utility.Filters.Policy
{
    /// <summary>
    /// 策略授权参数
    /// </summary>
    public class CustomAuthorizationRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// 
        /// </summary>
        public CustomAuthorizationRequirement(string policyname)
        {
            this.Name = policyname;
        }

        public string Name { get; set; }
    }
}
