﻿namespace Known.Designers;

class PageProperty : BaseProperty<PageColumnInfo>
{
    protected override void BuildForm(RenderTreeBuilder builder)
    {
        Model ??= new();
        builder.Div("caption", () => builder.Div("title", $"{Language["Designer.FieldProperty"]} - {Model.Id}"));
        builder.Div("property", () =>
        {
            BuildPropertyItem(builder, "Name", b => b.Span(Model.Name));
            BuildPropertyItem(builder, "IsViewLink", b => UI.BuildSwitch(b, new InputModel<bool>
            {
                Disabled = IsReadOnly,
                Value = Model.IsViewLink,
                ValueChanged = this.Callback<bool>(value => { Model.IsViewLink = value; OnChanged?.Invoke(Model); })
            }));
            BuildPropertyItem(builder, "IsQuery", b => UI.BuildSwitch(b, new InputModel<bool>
            {
                Disabled = IsReadOnly,
                Value = Model.IsQuery,
                ValueChanged = this.Callback<bool>(value => { Model.IsQuery = value; OnChanged?.Invoke(Model); })
            }));
            if (Model.IsQuery)
            {
                BuildPropertyItem(builder, "IsQueryAll", b => UI.BuildSwitch(b, new InputModel<bool>
                {
                    Disabled = IsReadOnly,
                    Value = Model.IsQueryAll,
                    ValueChanged = this.Callback<bool>(value => { Model.IsQueryAll = value; OnChanged?.Invoke(Model); })
                }));
            }
            BuildPropertyItem(builder, "IsSum", b => UI.BuildSwitch(b, new InputModel<bool>
            {
                Disabled = IsReadOnly,
                Value = Model.IsSum,
                ValueChanged = this.Callback<bool>(value => { Model.IsSum = value; OnChanged?.Invoke(Model); })
            }));
            BuildPropertyItem(builder, "IsSort", b => UI.BuildSwitch(b, new InputModel<bool>
            {
                Disabled = IsReadOnly,
                Value = Model.IsSort,
                ValueChanged = this.Callback<bool>(value => { Model.IsSort = value; OnChanged?.Invoke(Model); })
            }));
            if (Model.IsSort)
            {
                BuildPropertyItem(builder, "DefaultSort", b => UI.BuildSelect(b, new InputModel<string>
                {
                    Disabled = IsReadOnly,
                    Codes = Cache.GetCodes(",Ascend,Descend"),
                    Value = Model.DefaultSort,
                    ValueChanged = this.Callback<string>(value => { Model.DefaultSort = value; OnChanged?.Invoke(Model); })
                }));
            }
            BuildPropertyItem(builder, "Fixed", b => UI.BuildSelect(b, new InputModel<string>
            {
                Disabled = IsReadOnly,
                Codes = Cache.GetCodes(",left,right"),
                Value = Model.Fixed,
                ValueChanged = this.Callback<string>(value => { Model.Fixed = value; OnChanged?.Invoke(Model); })
            }));
            BuildPropertyItem(builder, "Width", b => UI.BuildNumber(b, new InputModel<int?>
            {
                Disabled = IsReadOnly,
                Value = Model.Width,
                ValueChanged = this.Callback<int?>(value => { Model.Width = value; OnChanged?.Invoke(Model); })
            }));
            BuildPropertyItem(builder, "Align", b => UI.BuildSelect(b, new InputModel<string>
            {
                Disabled = IsReadOnly,
                Codes = Cache.GetCodes(",left,center,right"),
                Value = Model.Align,
                ValueChanged = this.Callback<string>(value => { Model.Align = value; OnChanged?.Invoke(Model); })
            }));
        });
    }
}