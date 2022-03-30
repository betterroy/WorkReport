using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Commons.Api
{
    /// <summary>
    /// 接口返回通用对象
    /// </summary>
    public class HttpResponseResult
    {
        /// <summary>
        /// 状态码(Code为200时代表成功，非200代表失败，如果失败，Msg为失败原因。)
        /// 200 成功
        /// 400 参数错误
        /// 401 鉴权失败(eg：未登录、登录过期)
        /// 500 服务器异常
        /// 0
        /// </summary>
        public HttpResponseCode Code { get; set; } = HttpResponseCode.Success;
        public string Msg { get; set; }
        public object Data { get; set; }
    }

    //public class HttpResponseResult<T> : HttpResponseResult
    //{
    //    public T Data { get; set; }
    //}

    public enum HttpResponseCode
    {
        /// <summary>
        /// 正常
        /// </summary>
        Success = 200,
        /// <summary>
        /// 请求参数无效
        /// </summary>
        BadRequest = 400,
        /// <summary>
        /// 授权失败
        /// </summary>
        Unauthorized = 401,
        /// <summary>
        /// 服务器异常
        /// </summary>
        InternalServerError = 500,
        /// <summary>
        /// 操作失败
        /// </summary>
        Failed = 500
    }
}
