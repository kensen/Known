﻿namespace Known.Razor.Components.Fields;

public class CheckBox : Field
{
    private bool IsChecked => Checked || Value == "True";

    [Parameter] public string Text { get; set; }
    [Parameter] public bool Checked { get; set; }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        builder.Input(attr =>
        {
            attr.Type("checkbox").Id(Id).Name(Id).Role("switch")
                .Disabled(!Enabled).Required(Required).Checked(IsChecked)
                .OnChange(EventCallback.Factory.CreateBinder<bool>(this, isCheck =>
                {
                    Value = isCheck ? "True" : "False";
                    OnValueChange();
                }, IsChecked));
        });
        builder.Span(Text);
    }
}