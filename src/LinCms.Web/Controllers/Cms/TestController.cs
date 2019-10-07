using System;
using System.Collections.Generic;
using LinCms.Zero.Aop;
using LinCms.Zero.Data;
using LinCms.Zero.Data.Enums;
using LinCms.Zero.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.Cms
{
    [Route("cms/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("info")]
        [LinCmsAuthorize("查看lin的信息", "信息")]
        public ResultDto Info()
        {
            return ResultDto.Success("Lin 是一套基于 Python-Flask 的一整套开箱即用的后台管理系统（CMS）。Lin 遵循简洁、高效的原则，通过核心库加插件的方式来驱动整个系统高效的运行");
        }

        [HttpGet("")]
        public string Slogan()
        {
            return @"<style type=""text/css"">*{ padding: 0; margin: 0; } div{ padding: 4px 48px;} a{color:#2E5CD5;cursor: 
                    pointer;text-decoration: none} a:hover{text-decoration:underline; } body{ background: #fff; font-family:
                    ""Century Gothic"",""Microsoft yahei""; color: #333;font-size:18px;} h1{ font-size: 100px; font-weight: normal; 
                    margin-bottom: 12px; } p{ line-height: 1.6em; font-size: 42px }</style><div style=""padding: 24px 48px;"" ><p>
                    Lin <br/><span style=""font -size:30px"" > 心上无垢，林间有风。</span></p></div> ";
        }

        /// <summary>
        /// 下划线，首字母会小写
        /// </summary>
        /// <returns></returns>
        [HttpGet("get")]
        public dynamic Get()
        {
            return new
            {
                Content = new
                {
                    Url = Request.Path.Value,
                    NewUrlTest = "test in new url test"
                }
            };
        }

        /// <summary>
        /// 这种方式不会变小写，怀疑人生，是不是他的key不是键？ https://github.com/JamesNK/Newtonsoft.Json/issues/2088
        /// </summary>
        /// <returns></returns>
        [HttpGet("getDictionary")]
        public IDictionary<string, string> GetDictionary()
        {
            IDictionary<string, string> dics = new Dictionary<string, string>();

            dics.Add("Key", "Value");
            dics.Add("Key_Test", "Value_Test");

            return dics;
        }

        [HttpGet("lincms-exception")]
        public ResultDto TestLinCmsException()
        {
            throw new LinCmsException("我报异常了-NotFound！", ErrorCode.NotFound,StatusCodes.Status404NotFound);

            //return ResultDto.Success();
        }

        [HttpGet("test-time")]
        public void TestTime(DateTime dateTime)
        {
            Console.WriteLine(dateTime);
        }
    }
}
