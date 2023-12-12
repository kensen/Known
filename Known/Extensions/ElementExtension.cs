﻿using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Extensions;

public static class ElementExtension
{
    public static RenderTreeBuilder OpenElement(this RenderTreeBuilder builder, string elementName)
    {
        builder.OpenElement(0, elementName);
        return builder;
    }

    public static RenderTreeBuilder Attribute(this RenderTreeBuilder builder, string name, object value)
    {
        builder.AddAttribute(1, name, value);
        return builder;
    }

    public static RenderTreeBuilder Id(this RenderTreeBuilder builder, string id) => builder.Attribute("id", id);
    public static RenderTreeBuilder Class(this RenderTreeBuilder builder, string className) => builder.Attribute("class", className);
    public static RenderTreeBuilder OnClick(this RenderTreeBuilder builder, object onclick) => builder.Attribute("onclick", onclick);
}