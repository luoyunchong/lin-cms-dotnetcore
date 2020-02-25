using System;
using System.IO;
using LinCms.Core.Common;
using LinCms.Core.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinCms.Test.Controller.Cms
{
    public class UserControllerTests : BaseControllerTests
    {
        private readonly IFreeSql FreeSql;

        public UserControllerTests() : base()
        {
            FreeSql = serviceProvider.GetService<IFreeSql>();
        }

        [Fact]
        public void InsertLinUserCommunity()
        {
            FreeSql.Transaction((Action)(() =>
            {
                long insertId = FreeSql.Insert(new LinUser
                {
                    Admin = 1,
                    Active = 1,
                    Avatar = "122",
                    CreateTime = DateTime.Now,
                    Email = "122",
                    Introduction = "122",
                    Nickname = "122",
                    Username = "122"
                }).ExecuteIdentity();

                FreeSql.Insert(new LinUserIdentity
                {
                    CreateTime = DateTime.Now,
                    Credential = "122",
                    IdentityType = LinUserIdentity.WeiXin,
                    Identifier = "122",
                    CreateUserId = insertId
                }).ExecuteAffrows();
            }));
        }
    }
}
