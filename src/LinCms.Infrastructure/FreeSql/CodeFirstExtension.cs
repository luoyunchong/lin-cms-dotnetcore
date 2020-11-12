﻿using FreeSql;
using LinCms.Common;
using LinCms.Data.Enums;
using LinCms.Entities;
using LinCms.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinCms.FreeSql
{
    public static class CodeFirstExtension
    {

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
