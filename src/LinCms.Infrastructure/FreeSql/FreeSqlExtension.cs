using System;
using System.Collections.Generic;
using FreeSql;
using LinCms.Common;
using LinCms.Data.Enums;
using LinCms.Entities;
using LinCms.Entities.Base;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace LinCms.FreeSql
{
    public static class FreeSqlExtension
    {
        public static FreeSqlBuilder UseConnectionString(this FreeSqlBuilder @this, IConfiguration configuration)
        {
            IConfigurationSection dbTypeCode = configuration.GetSection("ConnectionStrings:DefaultDB");
            if (Enum.TryParse(dbTypeCode.Value, out DataType dataType))
            {
                if (!Enum.IsDefined(typeof(DataType), dataType))
                {
                    Log.Error($"数据库配置ConnectionStrings:DefaultDB:{dataType}无效");
                }
                IConfigurationSection configurationSection = configuration.GetSection($"ConnectionStrings:{dataType}");
                @this.UseConnectionString(dataType, configurationSection.Value);
            }
            else
            {
                Log.Error($"数据库配置ConnectionStrings:DefaultDB:{dbTypeCode.Value}无效");
            }
            return @this;
        }

        public static ICodeFirst SeedData(this ICodeFirst @this)
        {
            @this.Entity<LinGroup>(e =>
                 {
                     e.HasData(new List<LinGroup>()
                    {
                        new LinGroup(LinGroup.Admin,"系统管理员",true),
                        new LinGroup(LinGroup.CmsAdmin,"CMS管理员",true),
                        new LinGroup(LinGroup.User,"普通用户",true)
                    });
                 })
                .Entity<LinUser>(e =>
                {
                    e.HasData(new List<LinUser>()
                    {
                        new LinUser()
                        {
                            Nickname="系统管理员",
                            Username="admin",
                            Active=UserActive.Active,
                            CreateTime=DateTime.Now,
                            IsDeleted=false,
                            LinUserIdentitys=new List<LinUserIdentity>()
                            {
                               new LinUserIdentity(LinUserIdentity.Password,"admin",EncryptUtil.Encrypt("123qwe"),DateTime.Now)
                            },
                            LinUserGroups=new List<LinUserGroup>()
                            {
                                new LinUserGroup(1,LinConsts.Group.Admin)
                            },
                        },
                         new LinUser()
                         {
                             Nickname="CMS管理员",
                             Username="CmsAdmin",
                             Active=UserActive.Active,
                             CreateTime=DateTime.Now,
                             IsDeleted=false,
                             LinUserIdentitys=new List<LinUserIdentity>()
                             {
                                 new LinUserIdentity(LinUserIdentity.Password,"CmsAdmin",EncryptUtil.Encrypt("123qwe"),DateTime.Now)
                             },
                             LinUserGroups=new List<LinUserGroup>()
                             {
                                 new LinUserGroup(2,LinConsts.Group.CmsAdmin)
                             },
                         }
                    });
                })
                .Entity<BaseType>(e =>
                {
                    e.HasData(new List<BaseType>()
                    {
                        new BaseType("Article.Type","文章类型",1)
                        {
                            CreateTime=DateTime.Now,IsDeleted=false,CreateUserId = 1,
                            BaseItems=new List<BaseItem>()
                            {
                                new BaseItem("0","原创",1,1,true){CreateUserId = 1,CreateTime=DateTime.Now,IsDeleted=false},
                                new BaseItem("1","转载",2,1,true){CreateUserId = 1,CreateTime=DateTime.Now,IsDeleted=false},
                                new BaseItem("2","翻译",3,1,true){CreateUserId = 1,CreateTime=DateTime.Now,IsDeleted=false}
                            }
                        },
                         new BaseType("Sex","性别",2)
                         {
                             CreateTime=DateTime.Now,IsDeleted=false,CreateUserId = 1,
                             BaseItems=new List<BaseItem>()
                             {
                                 new BaseItem("0","男",1,2,true){CreateTime=DateTime.Now,IsDeleted=false},
                                 new BaseItem("1","女",2,2,true){CreateTime=DateTime.Now,IsDeleted=false},
                                 new BaseItem("2","保密",3,2,true){CreateTime=DateTime.Now,IsDeleted=false}
                             }
                         },
                    });
                })
                ;
            return @this;
        }
    }
}
