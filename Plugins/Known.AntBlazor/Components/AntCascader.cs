﻿namespace Known.AntBlazor.Components;

/// <summary>
/// 扩展Ant级联选择框组件类。
/// </summary>
public class AntCascader : Cascader
{
    [CascadingParameter] private IAntForm AntForm { get; set; }
    [CascadingParameter] private DataItem Item { get; set; }

    /// <summary>
    /// 初始化组件。
    /// </summary>
    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(string);
        base.OnInitialized();
    }
}