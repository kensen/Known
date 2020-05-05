﻿using Known.Core;
using Known.Core.Entities;

namespace Known.Web
{
    static class ModelExtension
    {
        public static object ToTree(this MenuInfo menu)
        {
            return new
            {
                id = menu.Id,
                pid = menu.ParentId,
                code = menu.Code,
                title = menu.Name,
                icon = menu.Icon,
                url = menu.Url,
                target = menu.Target,
                @checked = menu.Checked,
            };
        }

        public static object ToTree(this SysModule module)
        {
            return new
            {
                id = module.Id,
                pid = module.ParentId,
                title = module.Name,
                icon = module.Icon,
                spread = string.IsNullOrWhiteSpace(module.ParentId),
                module
            };
        }
    }
}