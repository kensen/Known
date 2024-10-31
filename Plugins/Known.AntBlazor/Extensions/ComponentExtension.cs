﻿namespace Known.AntBlazor.Extensions;

static class ComponentExtension
{
    internal static void AntTag(this RenderTreeBuilder builder, string text, string color)
    {
        builder.Component<Tag>()
               .Set(c => c.Color, color)
               .Set(c => c.ChildContent, b => b.Text(text))
               .Build();
    }

    internal static void AntIcon(this RenderTreeBuilder builder, string type, EventCallback<MouseEventArgs>? onClick = null)
    {
        if (onClick == null)
        {
            builder.Component<Icon>().Set(c => c.Type, type).Set(c => c.Theme, "outline").Build();
            return;
        }

        builder.Component<Icon>()
               .Set(c => c.Type, type)
               .Set(c => c.Theme, "outline")
               .Set(c => c.OnClick, onClick.Value)
               .Build();
    }

    internal static void AntResult(this RenderTreeBuilder builder, string status, string message)
    {
        builder.Component<AntDesign.Result>()
               .Set(c => c.Status, status)
               .Set(c => c.Title, status)
               .Set(c => c.SubTitle, message)
               .Build();
    }

    internal static void AntButton(this RenderTreeBuilder builder, string name, EventCallback<MouseEventArgs> onClick, string type = ButtonType.Primary)
    {
        builder.Component<AntButton>()
               .Set(c => c.Name, name)
               .Set(c => c.Type, type)
               .Set(c => c.OnClick, onClick)
               .Build();
    }

    internal static void AntButton(this RenderTreeBuilder builder, ActionInfo info)
    {
        builder.Component<AntButton>()
               .Set(c => c.Enabled, info.Enabled)
               .Set(c => c.Icon, info.Icon)
               .Set(c => c.Name, info.Name)
               .Set(c => c.Type, info.Style)
               .Set(c => c.OnClick, info.OnClick)
               .Build();
    }

    internal static void AntSearch(this RenderTreeBuilder builder, InputModel<string> model)
    {
        builder.Component<Search>()
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.ClassicSearchIcon, true)
               .Set(c => c.Placeholder, model.Placeholder)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    internal static void AntText(this RenderTreeBuilder builder, InputModel<string> model)
    {
        builder.Component<Input<string>>()
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.Placeholder, model.Placeholder)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    internal static void AntTextArea(this RenderTreeBuilder builder, InputModel<string> model)
    {
        builder.Component<TextArea>()
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.Placeholder, model.Placeholder)
               .Set(c => c.Rows, model.Rows)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    internal static void AntPassword(this RenderTreeBuilder builder, InputModel<string> model)
    {
        builder.Component<InputPassword>()
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.Placeholder, model.Placeholder)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    internal static void AntDatePicker<TValue>(this RenderTreeBuilder builder, InputModel<TValue> model)
    {
        if (typeof(TValue) == typeof(string))
        {
            builder.Component<AntRangePicker>()
                   .Set(c => c.Disabled, model.Disabled)
                   .Set(c => c.Placeholder, model.Placeholder)
                   .Set(c => c.RangeValue, model.Value?.ToString())
                   .Set(c => c.RangeValueChanged, v=>
                   {
                       var value = Utils.ConvertTo<TValue>(v);
                       model.ValueChanged.InvokeAsync(value);
                   })
                   .Build();
        }
        else
        {
            builder.Component<DatePicker<TValue>>()
                   .Set(c => c.Disabled, model.Disabled)
                   .Set(c => c.Placeholder, model.Placeholder)
                   .Set(c => c.Value, model.Value)
                   .Set(c => c.ValueChanged, model.ValueChanged)
                   .Build();
        }
    }

    internal static void AntNumber<TValue>(this RenderTreeBuilder builder, InputModel<TValue> model)
    {
        builder.Component<InputNumber<TValue>>()
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    internal static void AntCheckBox(this RenderTreeBuilder builder, InputModel<bool> model)
    {
        builder.Component<Checkbox>()
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.Checked, model.Value)
               .Set(c => c.Indeterminate, model.Indeterminate)
               .Set(c => c.Label, model.Label)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    internal static void AntSwitch(this RenderTreeBuilder builder, InputModel<bool> model)
    {
        builder.Component<Switch>()
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.Checked, model.Value)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    internal static void AntSelect(this RenderTreeBuilder builder, InputModel<string> model)
    {
        builder.Component<AntSelectCode>()
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.Placeholder, model.Placeholder)
               .Set(c => c.DataSource, model.Codes)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    internal static void AntRadioList(this RenderTreeBuilder builder, InputModel<string> model)
    {
        builder.Component<AntRadioGroup>()
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.Codes, model.Codes)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    internal static void AntCheckList(this RenderTreeBuilder builder, InputModel<string[]> model)
    {
        builder.Component<AntCheckboxGroup>()
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.Codes, model.Codes)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }
}