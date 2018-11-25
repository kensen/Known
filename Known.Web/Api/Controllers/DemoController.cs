﻿using System.Collections.Generic;
using Known.Extensions;
using Known.Platform;

namespace Known.Web.Api.Controllers
{
    public class DemoController : BaseApiController
    {
        public ApiResult QueryUsers(PagingCriteria criteria)
        {
            var users = new List<User>();
            users.Add(new User
            {
                Id = "1",
                UserName = "admin",
                Name = "管理员",
                Email = "admin@known.com",
                Mobile = "18988888888",
                Phone = "68888888",
                Department = new Department
                {
                    Name = "研发中心"
                }
            });
            users.Add(new User
            {
                Id = "2",
                UserName = "zhangsan",
                Name = "张三",
                Email = "zhangsan@known.com",
                Department = new Department
                {
                    Name = "管理中心"
                }
            });
            for (int i = 3; i < 188; i++)
            {
                users.Add(new User
                {
                    Id = i.ToString(),
                    UserName = $"account{i}",
                    Name = $"操作员{i}",
                    Department = new Department
                    {
                        Name = "操作部"
                    }
                });
            }

            var data = users.ToPageList(criteria.PageIndex, criteria.PageSize);
            return ApiResult.Success(new { total = users.Count, data });
        }
    }
}