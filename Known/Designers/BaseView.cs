﻿namespace Known.Designers;

class BaseView<TModel> : BaseComponent
{
    protected TabModel Tab { get; } = new();
    [Inject] internal ICodeGenerator Generator { get; set; }
    [CascadingParameter] internal ModuleForm Form { get; set; }

    [Parameter] public TModel Model { get; set; }
    [Parameter] public Action<TModel> OnChanged { get; set; }

    internal virtual void SetModel(TModel model) => Model = model;
    protected override void BuildRender(RenderTreeBuilder builder) => UI.BuildTabs(builder, Tab);

    protected void BuildList<TItem>(RenderTreeBuilder builder, TableModel<TItem> model) where TItem : class, new()
    {
        builder.Div("list-view", () => UI.BuildTable(builder, model));
    }

    protected void BuildCode(RenderTreeBuilder builder, string code)
    {
        var html = $"<div class=\"highlight kui-code\"><pre class=\"language-csharp\"><code>{code}</code></pre></div>";
        builder.Markup(html);
    }

    protected void BuildPropertyItem(RenderTreeBuilder builder, string label, Action<RenderTreeBuilder> template)
    {
        builder.Div("item", () =>
        {
            if (!string.IsNullOrWhiteSpace(label))
                builder.Label(Language[label]);
            builder.Div(() => template?.Invoke(builder));
        });
    }
}