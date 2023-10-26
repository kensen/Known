﻿namespace Known.Razor;

public class KForm : BaseComponent
{
    private bool isInitialized;

    public KForm()
    {
        FormContext = new FormContext();
    }

    [Parameter] public bool InDialog { get; set; }
    [Parameter] public string Style { get; set; }
    [Parameter] public object Model { get; set; }
    [Parameter] public Action<Result> OnSuccess { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }

    [CascadingParameter] internal PageTabs Tabs { get; set; }

    internal FormContext FormContext { get; }
    protected string CheckFields { get; set; }
    public dynamic Data => FormContext.Data;
    public Dictionary<string, Field> Fields => FormContext.Fields;
    public T FieldAs<T>(string id) where T : Field => FormContext.FieldAs<T>(id);

    protected virtual Task InitFormAsync() => Task.CompletedTask;

    protected override async Task OnInitializedAsync()
    {
        await AddVisitLogAsync();
        await InitFormAsync();
        isInitialized = true;
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        FormContext.ReadOnly = ReadOnly;
        FormContext.Model = Model;
        FormContext.CheckFields = CheckFields;
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        UI.InitForm();
        return base.OnAfterRenderAsync(firstRender);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!isInitialized)
            return;

        BuildForm(builder);
    }

    internal virtual void BuildForm(RenderTreeBuilder builder)
    {
        var css = CssBuilder.Default("form").AddClass(Style).Build();
        builder.Div(css, attr =>
        {
            if (ChildContent == null)
            {
                builder.Div("form-body", attr =>
                {
                    builder.Cascading(FormContext, BuildFields);
                });
                builder.Div("form-button", attr => BuildButtons(builder));
            }
            else
            {
                builder.Cascading(FormContext, ChildContent);
            }
        });
    }

    protected virtual void BuildFields(RenderTreeBuilder builder) { }
    protected virtual void BuildButtons(RenderTreeBuilder builder) => builder.Button(FormButton.Close, Callback(OnCancel));

    protected virtual void OnOK()
    {
        Submit(data =>
        {
            var result = Result.SuccessAsync("", data);
            OnSuccess?.Invoke(result);
            UI.CloseDialog();
        });
    }

    protected virtual void OnCancel()
    {
        FormContext.Clear();
        if (InDialog)
            UI.CloseDialog();
        else if (Tabs != null)
            Tabs.CloseCurrent();
        else
            Context.Back();
    }

    protected bool HasButton(ButtonInfo button)
    {
        var user = CurrentUser;
        if (user == null)
            return false;

        return button.IsInMenu(Id);
    }

    public bool Validate() => FormContext.Validate();
    public bool ValidateCheck(bool isPass) => FormContext.ValidateCheck(isPass);

    public void Clear()
    {
        FormContext.Clear();
        StateChanged();
    }

    public void SetData(object data)
    {
        Model = data;
        FormContext.SetData(data);
        StateChanged();
    }

    public void SetReadOnly(bool readOnly)
    {
        FormContext.SetReadOnly(readOnly);
        StateChanged();
    }

    public void SetEnabled(bool enabled)
    {
        FormContext.SetEnabled(enabled);
        StateChanged();
    }

    public void Submit(Action<dynamic> action)
    {
        if (!Validate())
            return;

        action.Invoke(Data);
    }

    public async Task SubmitAsync(Func<dynamic, Task<Result>> action, Action<Result> onSuccess = null)
    {
        if (!Validate())
            return;

        var result = await action.Invoke(Data);
        OnSubmited(result, onSuccess);
    }

    public async Task SubmitFilesAsync(Func<UploadFormInfo, Task<Result>> action, Action<Result> onSuccess = null)
    {
        if (!Validate())
            return;

        var info = new UploadFormInfo
        {
            Model = Data,
            Files = GetFiles()
        };
        var result = await action.Invoke(info);
        OnSubmited(result, onSuccess);
    }

    private void OnSubmited(Result result, Action<Result> onSuccess)
    {
        if (onSuccess != null)
        {
            onSuccess.Invoke(result);
            return;
        }

        UI.Result(result, () => OnSuccess?.Invoke(result));
    }

    private void AddFiles(MultipartFormDataContent content)
    {
        var files = FormContext.Files;
        foreach (var item in files)
        {
            foreach (var file in item.Value)
            {
                AddFileContent(content, item.Key, file);
            }
        }
    }

    private static void AddFileContent(MultipartFormDataContent content, string key, IBrowserFile file)
    {
        var fileContent = new StreamContent(file.OpenReadStream(KUpload.MaxLength));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, $"\"file{key}\"", file.Name);
    }

    private Dictionary<string, List<IAttachFile>> GetFiles()
    {
        var dicFiles = FormContext.Files;
        var files = new Dictionary<string, List<IAttachFile>>();
        foreach (var item in dicFiles)
        {
            var values = GetAttachFiles(item.Value);
            files.Add(item.Key, values);
        }
        return files;
    }

    private static List<IAttachFile> GetAttachFiles(List<IBrowserFile> bFiles)
    {
        var files = new List<IAttachFile>();
        foreach (var item in bFiles)
        {
            files.Add(new BlazorAttachFile(item));
        }
        return files;
    }
}