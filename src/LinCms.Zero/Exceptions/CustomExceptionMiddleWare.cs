using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinCms.Zero.Data;
using LinCms.Zero.Data.Enums;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LinCms.Zero.Exceptions
{
    public class CustomExceptionMiddleWare
    {
        /// <summary>
        /// 管道请求委托
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// 需要处理的状态码字典
        /// </summary>
        private readonly IDictionary<int, string> _exceptionStatusCodeDic;

        private IDictionary<int, string> _errCodes;
        public CustomExceptionMiddleWare(RequestDelegate next)
        {
            _next = next;
            _exceptionStatusCodeDic = new Dictionary<int, string>
            {
                { 400 ,"BadRequest" },
                { 401, "未授权的请求" },
                { 404, "找不到该页面" },
                { 403, "访问被拒绝" },
                { 500, "服务器发生意外的错误" }
            };

            //_errCodes= Enum.GetValues(typeof(ErrorCode))
            //    .Cast<ErrorCode>()
            //    .ToDictionary(t => (int)t, t => t.ToString());

            //不知道lin-cms-vue这样设计的作用，使用errCode判断是否成功？
            _errCodes = new Dictionary<int, string>()
            {
                { 0, "成功" },
                { 1007, "未知错误" },
                { 999, "服务器未知错误" },
                { 9999, "失败" },
                { 10000, "认证失败" },
                { 10020, "资源不存在" },
                { 10030, "参数错误" },
                { 10040, "令牌失效" },
                { 10050, "令牌过期" },
                { 10060, "字段重复" },
                { 10070, "不可操作" },
                { 10100, "refreshToken异常" }
                //其余状态自行扩展
            };
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);   //调用管道执行下一个中间件
            }
            catch (Exception ex)
            {
                //自定义业务异常
                if (ex is LinCmsException cmsException)
                {
                    context.Response.StatusCode = cmsException.GetCode();

                    string errorMsg = $"{(cmsException.InnerException != null ? cmsException.InnerException.Message : cmsException.Message)}";
                    await JsonHandle(context, errorMsg, cmsException.GetErrorCode());
                }
                else
                {
                    string errorMsg = "";
                    if (_exceptionStatusCodeDic.ContainsKey(context.Response.StatusCode) &&
                        !context.Items.ContainsKey("ExceptionHandled"))
                    {
                        errorMsg = $"{_exceptionStatusCodeDic[context.Response.StatusCode]}";
                    }
                    else
                    {
                        errorMsg = $"异常信息:"+ex.Message;
                    }
                    await JsonHandle(context, errorMsg, ErrorCode.UnknownError.GetHashCode());

                }
            }
        }

        /// <summary>
        /// 处理方式：返回Json格式
        /// </summary>
        /// <param name="context"></param>
        /// <param name="errorMsg"></param>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        private async Task JsonHandle(HttpContext context, string errorMsg, int errorCode)
        {
            ResultDto apiResponse = new ResultDto()
            {
                Msg = errorMsg,
                Request = $"{context.Request.Method} {context.Request.Path}",
                ErrorCode = errorCode
            }; ;
            string jsonResult = JsonConvert.SerializeObject(apiResponse, new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            });

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(jsonResult, Encoding.UTF8);
        }

    }
}
