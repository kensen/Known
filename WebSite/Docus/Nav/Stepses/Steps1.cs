﻿namespace WebSite.Docus.Nav.Stepses;

class Steps1 : BaseComponent
{
    private string? message;
    private readonly List<MenuItem> items = new()
    {
        new MenuItem { Icon = "fa fa-home", Name = "步骤一" },
        new MenuItem { Icon = "fa fa-home", Name = "步骤二" },
        new MenuItem { Icon = "fa fa-home", Name = "步骤三" }
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Steps>()
               .Set(c => c.Items, items)
               .Set(c => c.OnChanged, OnChanged)   //每步保存事件
               .Set(c => c.OnFinished, OnFinished) //完成按钮事件
               .Set(c => c.Body, BuildStep)        //建造每步内容
               .Build();
        builder.Div("tips", message);
    }

    private void BuildStep(RenderTreeBuilder builder, MenuItem item) => builder.Span(item.Name);
    private void OnChanged(MenuItem item) => ShowMessage($"保存{item.Name}");
    private void OnFinished() => ShowMessage("完成分步表单");

    private void ShowMessage(string message)
    {
        this.message = message;
        StateChanged();
    }
}