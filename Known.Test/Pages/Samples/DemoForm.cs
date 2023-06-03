﻿using CheckBox = Known.Razor.Components.Fields.CheckBox;

namespace Known.Test.Pages.Samples;

class DemoForm : Razor.Components.Form
{
    private readonly string Codes = "孙膑,后羿,妲己";
    private string formData;

    public DemoForm()
    {
        IsTable = true;
    }

    protected override void BuildFields(RenderTreeBuilder builder)
    {
        builder.Div("demo-caption", "默认表单");
        builder.Div("grid", attr =>
        {
            builder.Field<Text>("文本", "Text", true).Build();
            builder.Field<Number>("数值", "Number").Build();
            builder.Field<Select>("下拉", "Select", true).Set(f => f.Codes, Codes).Build();
        });
        builder.Div("grid", attr =>
        {
            builder.Field<Date>("日期", "Date").Build();
            builder.Field<Date>("月份", "Month").Set(f => f.DateType, DateType.Month).Build();
            builder.Field<Date>("日期时间", "DateTime").Set(f => f.DateType, DateType.DateTime).Build();
        });
        builder.Div("grid", attr =>
        {
            builder.Field<RadioList>("单选", "RadioList").Set(f => f.Codes, Codes).Build();
            builder.Field<CheckBox>("选项", "CheckBox").IsInput(true).Set(f => f.Text, "启用").Build();
            builder.Field<CheckList>("多选", "CheckList", true).Set(f => f.Codes, Codes).Build();
        });
        builder.Div("grid", attr =>
        {
            builder.Field<Picker>("选择（单选）", "Picker1").Build();
            builder.Field<Picker>("选择（多选）", "Picker2").Build();
            builder.Field<Upload>("附件", "Upload").Build();
        });
        builder.Field<TextArea>("文本域", "TextArea").ColSpan(5).Build();
        builder.Div("form-button", attr =>
        {
            builder.Button("加载", "fa fa-refresh", Callback(OnLoadData), KRStyle.Primary);
            builder.Button("验证", "fa fa-check", Callback(OnCheckData), KRStyle.Orange);
            builder.Button("保存", "fa fa-save", Callback(OnSaveData), KRStyle.Success);
            builder.Button("清空", "fa fa-trash-o", Callback(Clear), KRStyle.Danger);
        });
        builder.Div("demo-tips", formData);
    }

    private void OnLoadData()
    {
        SetData(new
        {
            Text = "test",
            Number = 20,
            Select = "孙膑",
            Date = new DateTime(2020, 01, 01),
            Month = $"{DateTime.Now:yyyy-MM}",
            DateTime = DateTime.Now,
            RadioList = "后羿",
            CheckBox = true,
            CheckList = "孙膑,妲己",
            Picker1 = "test1",
            Picker2 = "test1,test2",
            TextArea = "Test Note"
        });
    }

    private void OnCheckData() => Validate();
    private void OnSaveData() => Submit(data => formData = Utils.ToJson(data));
}