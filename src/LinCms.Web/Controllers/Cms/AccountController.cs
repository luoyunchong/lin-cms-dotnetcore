using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using DotNetCore.Security;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer4.Models;
using LinCms.Aop.Attributes;
using LinCms.Cms.Account;
using LinCms.Cms.Users;
using LinCms.Data;
using LinCms.Data.Enums;
using LinCms.Entities;
using LinCms.Exceptions;
using LinCms.IRepositories;
using LinCms.Middleware;
using LinCms.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Owl.reCAPTCHA;
using Owl.reCAPTCHA.v3;
using Serilog;

namespace LinCms.Controllers.Cms
{
    [ApiExplorerSettings(GroupName = "cms")]
    [AllowAnonymous]
    [ApiController]
    [Route("cms/user")]
    public class AccountController : ApiControllerBase
    {
        private readonly ITokenService _tokenService;
        public AccountController(IComponentContext componentContext, IConfiguration configuration)
        {
            bool isIdentityServer4 = configuration.GetSection("Service:IdentityServer4").Value?.ToBoolean() ?? false;
            _tokenService = componentContext.ResolveNamed<ITokenService>(isIdentityServer4 ? typeof(IdentityServer4Service).Name : typeof(JwtTokenService).Name);
        }
        /// <summary>
        /// 登录接口
        /// </summary>
        /// <param name="loginInputDto">用户名/密码：admin/123qwe</param>
        [DisableAuditing]
        [ServiceFilter(typeof(RecaptchaVerifyActionFilter))]
        [HttpPost("login")]
        public async Task<Tokens> Login(LoginInputDto loginInputDto)
        {
            return await _tokenService.LoginAsync(loginInputDto);
        }

        /// <summary>
        /// 刷新用户的token
        /// </summary>
        /// <returns></returns>
        [HttpGet("refresh")]
        public async Task<Tokens> GetRefreshToken()
        {
            string refreshToken;
            string authorization = Request.Headers["Authorization"];

            if (authorization != null && authorization.StartsWith(JwtBearerDefaults.AuthenticationScheme))
            {
                refreshToken = authorization.Substring(JwtBearerDefaults.AuthenticationScheme.Length + 1).Trim();
            }
            else
            {
                throw new LinCmsException(" 请先登录.", ErrorCode.RefreshTokenError);
            }
            return await _tokenService.GetRefreshTokenAsync(refreshToken);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        [HttpGet("logout")]
        public UnifyResponseDto Logout()
        {
            return UnifyResponseDto.Success("退出登录");
        }


        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="registerDto"></param>
        [Logger("用户注册")]
        [ServiceFilter(typeof(RecaptchaVerifyActionFilter))]
        [HttpPost("account/register")]
        public async Task<UnifyResponseDto> Register([FromBody] RegisterDto registerDto, [FromServices] IMapper _mapper, [FromServices] IUserService _userSevice)
        {
            LinUser user = _mapper.Map<LinUser>(registerDto);
            await _userSevice.CreateAsync(user, new List<long>(), registerDto.Password);
            return UnifyResponseDto.Success("注册成功");
        }
    }
}