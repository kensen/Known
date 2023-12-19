﻿using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class BaseView<TModel> : BaseComponent
{
    protected TabModel Tab { get; } = new();
    internal CodeService Service => new();

    [Parameter] public TModel Model { get; set; }

    internal virtual void SetModel(TModel model) => Model = model;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Tab.Items.Add(new ItemModel("视图") { Content = BuildView });
        Tab.Items.Add(new ItemModel("代码") { Content = BuildCode });
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder) => UI.BuildTabs(builder, Tab);
    protected virtual void BuildView(RenderTreeBuilder builder) { }
    protected virtual void BuildCode(RenderTreeBuilder builder) { }

    protected void BuildCode(RenderTreeBuilder builder, string code) => builder.Markup($"<pre class=\"kui-code\">{code}</pre>");
}