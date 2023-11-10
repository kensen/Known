﻿using System.Reflection;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

public interface IUIService
{
    Type GetInputType(ColumnAttribute column);
    Task Toast(string message, StyleType style = StyleType.Success);
    Task Result(Result result, Action action = null);
    void Alert(string message);
    void Confirm(string message, Func<Task> action);
    void ShowForm<TItem>(FormModel<TItem> model);
    void BuildTag(RenderTreeBuilder builder, string text, string color);
    void BuildResult(RenderTreeBuilder builder, string status, string message);
    void BuildPage<TItem>(RenderTreeBuilder builder, PageModel<TItem> model);
}