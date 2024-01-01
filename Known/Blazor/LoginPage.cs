﻿using Known.Extensions;
using Microsoft.AspNetCore.Components;

namespace Known.Blazor;

public class LoginPage : BaseComponent
{
    protected LoginFormInfo Model = new();

    [Parameter] public Action<UserInfo> OnLogin { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var info = await JS.GetLoginInfo<LoginInfo>();
        if (info != null)
        {
            Model.UserName = info.UserName;
            Model.Remember = info.Remember;
        }
    }

    protected async Task OnUserLogin()
    {
        if (!Model.Remember)
            JS.SetLoginInfo(null);
        else
            JS.SetLoginInfo(new LoginInfo { UserName = Model.UserName, Remember = Model.Remember });

        Model.IPAddress = HttpContext?.Connection?.RemoteIpAddress?.ToString();
        var result = await Platform.Auth.SignInAsync(Model);
        if (!result.IsValid)
        {
            UI.Error(result.Message);
        }
        else
        {
            var user = result.DataAs<UserInfo>();
            OnLogin?.Invoke(user);
        }
    }

    class LoginInfo
    {
        public string UserName { get; set; }
        public bool Remember { get; set; }
    }
}