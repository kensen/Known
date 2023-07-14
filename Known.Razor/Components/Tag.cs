﻿namespace Known.Razor.Components;

public class Tag : BaseComponent
{
    [Parameter] public StyleType Style { get; set; }
    [Parameter] public string Text { get; set; }
    [Parameter] public Action<RenderTreeBuilder> Content { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var css = CssBuilder.Default("tag").AddClass(Style.ToString().ToLower()).Build();
        if (Content == null)
            builder.Span(css, Text);
        else
            builder.Span(css, attr => Content.Invoke(builder));
    }
}