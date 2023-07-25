﻿namespace Known.Razor.Pages.Forms;

class SysAccountForm : BaseForm<SysUser>
{
    private bool isEdit = false;

    public SysAccountForm()
    {
        Style = "ss-form";
    }

    [CascadingParameter] private SysAccount Account { get; set; }

    protected override void OnInitialized()
    {
        Model = CurrentUser;
    }

    protected override void BuildFields(FieldBuilder<SysUser> builder)
    {
        builder.Hidden(f => f.Id);
        builder.Field<Text>(f => f.UserName).ReadOnly(true).Build();
        builder.Field<Text>(f => f.Name).ReadOnly(!isEdit).Build();
        builder.Field<Input>(f => f.Phone).ReadOnly(!isEdit).Set(f => f.Type, InputType.Tel).Build();
        builder.Field<Input>(f => f.Mobile).ReadOnly(!isEdit).Set(f => f.Type, InputType.Tel).Build();
        builder.Field<Input>(f => f.Email).ReadOnly(!isEdit).Set(f => f.Type, InputType.Email).Build();
        builder.Field<Text>(f => f.Role).ReadOnly(true).Build();
        builder.Field<TextArea>(f => f.Note).ReadOnly(!isEdit).Build();
        builder.Div("form-button", attr => BuildButton(builder.Builder));
    }

    protected override void BuildButtons(RenderTreeBuilder builder) { }

    private void BuildButton(RenderTreeBuilder builder)
    {
        if (!isEdit)
        {
            builder.Button(FormButton.Edit, Callback(e => isEdit = true));
        }
        else
        {
            builder.Button(FormButton.Save, Callback(OnSave));
            builder.Button(FormButton.Cancel, Callback(e => isEdit = false));
        }
    }

    private void OnSave()
    {
        SubmitAsync(Platform.User.UpdateUserAsync, result =>
        {
            var curUser = CurrentUser;
            var user = result.DataAs<SysUser>();
            if (curUser != null && user != null)
            {
                curUser.Name = user.Name;
                curUser.Phone = user.Phone;
                curUser.Mobile = user.Mobile;
                curUser.Email = user.Email;
            }
            isEdit = false;
            Account.RefreshUserInfo();
            StateChanged();
        });
    }
}