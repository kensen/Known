﻿using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class InputField<TItem> : BaseComponent where TItem : class, new()
{
    [Parameter] public FieldModel<TItem> Model { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Model.OnStateChanged = StateChanged;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Model.Column.Template != null)
        {
            builder.Fragment(Model.Column.Template);
        }
        else if (Model.Column.IsFile || Model.Column.IsMultiFile)
        {
            builder.Component<UploadField<TItem>>().Set(c => c.Model, Model).Build();
        }
        else
        {
            var inputType = UI.GetInputType(Model.Column);
            if (inputType != null)
            {
                builder.OpenComponent(0, inputType);
                builder.AddMultipleAttributes(1, Model.InputAttributes);
                builder.CloseComponent();
            }
        }
    }
}