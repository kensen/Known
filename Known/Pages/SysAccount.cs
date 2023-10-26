﻿using Known.Extensions;
using Known.Razor;

namespace Known.Pages;

[Route("/account")]
public class SysAccount : PageComponent
{
    private readonly List<KMenuItem> items = new();
    private SysUserInfo userInfo;

    protected override void OnInitialized()
    {
        if (KRConfig.IsWeb)
        {
            items.Add(new KMenuItem("待办事项", "fa fa-tasks", typeof(SysMyFlowList)));
            items.Add(new KMenuItem("我的消息", "fa fa-envelope-o", typeof(SysMyMessage)));
        }
        items.Add(new KMenuItem("我的信息", "fa fa-user", typeof(SysAccountForm)));
        items.Add(new KMenuItem("安全设置", "fa fa-lock", typeof(SysUserPwdForm)));
    }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Div("lr-view", attr =>
        {
            builder.Cascading(this, b =>
            {
                b.Div("left-view", attr => BuildUserInfo(b));
                b.Div("right-view", attr => BuildUserTabs(b));
            });
        });
    }

    internal void RefreshUserInfo() => userInfo.Refresh();

    private void BuildUserInfo(RenderTreeBuilder builder)
    {
        builder.Component<SysUserInfo>().Build(value => userInfo = value);
    }

    private void BuildUserTabs(RenderTreeBuilder builder)
    {
        builder.Component<KTabs>()
               .Set(c => c.CurItem, items[0])
               .Set(c => c.Items, items)
               .Set(c => c.Body, (b, m) => b.DynamicComponent(m.ComType))
               .Build();
    }
}