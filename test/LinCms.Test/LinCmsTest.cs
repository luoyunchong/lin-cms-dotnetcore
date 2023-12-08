using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using DotNetCore.Security;
using FreeSql;
using LinCms.Entities.Base;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace LinCms.Test
{
    public class LinCmsTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFreeSql _fsql;
        private readonly IJwtService _jsonWebTokenService;
        private readonly ICryptographyService _cryptographyService;
        public LinCmsTest(ITestOutputHelper testOut, IJwtService jsonWebTokenService, IFreeSql freeSql, ICryptographyService cryptographyService)
        {
            _testOutputHelper = testOut;
            _jsonWebTokenService = jsonWebTokenService;
            _fsql = freeSql;
            _cryptographyService = cryptographyService;
        }

        //guid:9fd248c8-e9da-412f-bad9-aa5f7f1d7b80,passowrd:IWxIlqMAE3SU3JTogdDAJw==
        [Fact]
        public void CryptographyServiceEncrypt()
        {
            string guid = Guid.NewGuid().ToString();
            string encrptypassword = _cryptographyService.Encrypt("123qwe", guid);

            _testOutputHelper.WriteLine($"guid:{guid},passowrd:{encrptypassword}");
        }


        /// <summary>
        /// 主要负责工作单元的创建
        /// </summary>
        [Fact]
        public void CreateUnitOfWorkTest()
        {
            using IUnitOfWork uow = _fsql.CreateUnitOfWork();
            uow.GetOrBeginTransaction();

            using (IUnitOfWork uow2 = _fsql.CreateUnitOfWork())
            {
                uow2.GetOrBeginTransaction();

                uow2.Commit();
            }
            uow.Commit();
        }

        [Theory]
        [InlineData("F")]
        [InlineData("Bearer")]
        [InlineData("Bearer eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6WyI3MyIsIjE4NjEzMjY2Il0sImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6WyJsdW95dW5jaG9uZ0Bmb3htYWlsLmNvbSIsImx1b3l1bmNob25nQGZveG1haWwuY29tIl0sImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2dpdmVubmFtZSI6IklHZWVrRmFuIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6WyIiLCJsdW95dW5jaG9uZyJdLCJMaW5DbXNDbGFpbVR5cGVzLkdyb3VwcyI6IjMiLCJ1cm46Z2l0aHViOm5hbWUiOiJJR2Vla0ZhbiIsInVybjpnaXRodWI6dXJsIjoiaHR0cHM6Ly9hcGkuZ2l0aHViLmNvbS91c2Vycy9sdW95dW5jaG9uZyIsInVybjpnaXRodWI6YXZhdGFyX3VybCI6Imh0dHBzOi8vYXZhdGFyczEuZ2l0aHVidXNlcmNvbnRlbnQuY29tL3UvMTg2MTMyNjY_dj00IiwidXJuOmdpdGh1YjpiaW8iOiLkuobkuI3otbfnmoTnm5bojKjmr5QiLCJ1cm46Z2l0aHViOmJsb2ciOiJodHRwczovL2Jsb2cuaWdlZWtmYW4uY24vIiwibmJmIjoxNTk0NTY4ODQ3LCJleHAiOjE1OTcxNjA4NDcsImlzcyI6Imxpbi1jbXMtZG90bmV0Y29yZS1Jc3N1ZXIiLCJhdWQiOiJMaW5DbXMuV2ViIn0._WbTaOaftqCXHE95RoNTYm8c2Yq1tlOqJDALaqEQjgRIA7EtJKqgI72a2g6furX7B3SLjzA9Bwr5OHOfdV3qlw")]
        public void Jwt(string token)
        {

            if (token != "" && token.StartsWith("Bearer "))
            {
                token = token.Remove(0, 7);
                Dictionary<string, object> dict = _jsonWebTokenService.Decode(token);
                JwtPayload JwtPayload = new JwtSecurityTokenHandler().ReadJwtToken(token).Payload;
                object nameobj = JwtPayload[ClaimTypes.NameIdentifier];

                string[] names = JsonConvert.DeserializeObject<string[]>(nameobj.ToString());

                object c2 = JwtPayload[ClaimTypes.Name];
                object GivenName = JwtPayload[ClaimTypes.GivenName];

                string id = JwtPayload.Claims.FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier)?.Value;
            }


        }

        [Fact]
        public void FormatQueryString()
        {

            var result = new
            {
                code = "success",
                message = "绑定成功"
            };

            string msg = GetQueryString(result);

            Assert.Equal("code=message&message=绑定成功", msg);

        }

        private static string GetQueryString(object obj)
        {
            //HttpUtility
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + p.GetValue(obj, null).ToString();
            return string.Join("&", properties.ToArray());
        }
    }
}
