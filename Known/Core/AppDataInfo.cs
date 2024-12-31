﻿namespace Known.Core;

/// <summary>
/// 框架配置数据信息类。
/// </summary>
public class AppDataInfo
{
    /// <summary>
    /// 取得或设置语言信息列表。
    /// </summary>
    public List<LanguageInfo> Languages { get; set; } = [];

    /// <summary>
    /// 取得或设置按钮信息列表。
    /// </summary>
    public List<ButtonInfo> Buttons { get; set; } = [];

    /// <summary>
    /// 取得或设置顶部导航信息列表。
    /// </summary>
    public List<PluginInfo> TopNavs { get; set; } = [];

    /// <summary>
    /// 取得或设置模块信息列表。
    /// </summary>
    public List<ModuleInfo> Modules { get; set; } = [];
}

/// <summary>
/// 系统语言信息类。
/// </summary>
public class LanguageInfo
{
    /// <summary>
    /// 取得或设置语言ID。
    /// </summary>
    [Form, Required]
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置语言名称。
    /// </summary>
    [Form, Required]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置语言图标。
    /// </summary>
    [Form, Required]
    public string Icon { get; set; }
}

/// <summary>
/// 系统按钮信息类。
/// </summary>
public class ButtonInfo
{
    /// <summary>
    /// 取得或设置操作ID。
    /// </summary>
    [Form, Required]
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置操作名称。
    /// </summary>
    [Form, Required]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置操作图标。
    /// </summary>
    [Required]
    [Form(Type = nameof(FieldType.Custom), CustomField = nameof(IconPicker))]
    public string Icon { get; set; }

    /// <summary>
    /// 取得或设置操作样式，如：primary，danger等。
    /// </summary>
    [Form(Type = nameof(FieldType.RadioList))]
    [Category("primary，danger")]
    public string Style { get; set; }

    /// <summary>
    /// 取得或设置操作位置，如：Toolbar，Action。
    /// </summary>
    [Form(Type = nameof(FieldType.CheckList))]
    [Category("Toolbar，Action")]
    public string[] Position { get; set; }
}

/// <summary>
/// 框架插件信息类。
/// </summary>
public class PluginInfo
{
    /// <summary>
    /// 取得或设置插件实例ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置插件类型。
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置插件配置JSON。
    /// </summary>
    public string Setting { get; set; }
}

/// <summary>
/// 框架模块信息类。
/// </summary>
public class ModuleInfo
{
    /// <summary>
    /// 取得或设置ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置上级。
    /// </summary>
    public string ParentId { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置图标。
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// 取得或设置类型（Menu/Page/Link）。
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置目标（None/Blank/IFrame）。
    /// </summary>
    public string Target { get; set; }

    /// <summary>
    /// 取得或设置Url地址。
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 取得或设置顺序。
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 取得或设置插件配置信息列表。
    /// </summary>
    public List<PluginInfo> Plugins { get; set; } = [];

    /// <summary>
    /// 取得或设置可用。
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 获取模块的字符串表示。
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{Name}({Url})";
    }
}