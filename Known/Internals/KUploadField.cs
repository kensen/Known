﻿namespace Known.Internals;

class KUploadField<TItem> : KUpload where TItem : class, new()
{
    [Parameter] public FieldModel<TItem> Model { get; set; }

    protected override async Task OnInitAsync()
    {
        Id = Model.Column.Id;
        ReadOnly = Model.Form.IsView;
        Value = Model.Value?.ToString();
        MultiFile = Model.Column.MultiFile;
        OnFilesChanged = files =>
        {
            Model.Form.Files[Id] = files;
            Model.Value = Id;
            return Task.CompletedTask;
        };
        await base.OnInitAsync();
    }
}