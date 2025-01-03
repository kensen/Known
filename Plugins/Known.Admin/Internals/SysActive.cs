﻿namespace Known.Internals;

class SysActive : BaseComponent
{
    private ISystemService Service;
    private FormModel<SystemInfo> model;

    [Parameter] public Action<bool> OnCheck { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<ISystemService>();
        model = new FormModel<SystemInfo>(this);
        model.Data = new SystemInfo();
        model.AddRow().AddColumn(c => c.ProductId, c => c.ReadOnly = true);
        model.AddRow().AddColumn(c => c.ProductKey, c => c.Required = true);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            var info = await Service.GetProductAsync();
            model.Data.ProductId = info?.ProductId;
            model.Data.ProductKey = info?.ProductKey;
            await StateChangedAsync();
        }
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-card", () =>
        {
            builder.Component<AntDesign.Result>()
                   .Set(c => c.Status, "403")
                   .Set(c => c.Title, AdminConfig.AuthStatus)
                   .Build();
            builder.Div("kui-form-auth", () =>
            {
                builder.Form(model);
                builder.FormPageButton(() =>
                {
                    builder.Button(new ActionInfo(Context, "OK"), this.Callback<MouseEventArgs>(OnAuthAsync));
                });
            });
        });
    }

    private async Task OnAuthAsync(MouseEventArgs args)
    {
        if (!model.Validate())
            return;

        var result = await Service.SaveProductKeyAsync(model.Data);
        UI.Result(result, () =>
        {
            OnCheck?.Invoke(result.IsValid);
            return Task.CompletedTask;
        });
    }
}