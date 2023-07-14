﻿namespace Known.Razor.Components;

public class Badge : BaseComponent
{
    [Parameter] public StyleType Style { get; set; }
    [Parameter] public string Text { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var css = CssBuilder.Default("badge").AddClass(Style.ToString().ToLower()).Build();
        builder.Span(css, Text);
    }
}