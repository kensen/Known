﻿namespace Known;

/// <summary>
/// 设置信息类。
/// </summary>
public class SettingInfo
{
    /// <summary>
    /// 构造函数，创建一个设置信息类的实例。
    /// </summary>
    public SettingInfo()
    {
        Id = Utils.GetGuid();
    }

    /// <summary>
    /// 取得或设置ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置创建人。
    /// </summary>
    public string CreateBy { get; set; }

    /// <summary>
    /// 取得或设置业务类型。
    /// </summary>
    public string BizType { get; set; }

    /// <summary>
    /// 取得或设置业务名称。
    /// </summary>
    public string BizName { get; set; }

    /// <summary>
    /// 取得或设置业务数据。
    /// </summary>
    public string BizData { get; set; }

    /// <summary>
    /// 将业务数据JSON转换成泛型对象。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <returns>泛型对象。</returns>
    public T DataAs<T>() => Utils.FromJson<T>(BizData);
}

/// <summary>
/// 查询信息类。
/// </summary>
public class QueryInfo
{
    /// <summary>
    /// 构造函数，创建一个查询信息类的实例。
    /// </summary>
    public QueryInfo() { }

    /// <summary>
    /// 构造函数，创建一个查询信息类的实例，查询操作类型默认为包含于。
    /// </summary>
    /// <param name="id">查询字段ID。</param>
    /// <param name="value">查询字段值。</param>
    public QueryInfo(string id, string value) : this(id, QueryType.Contain, value) { }

    /// <summary>
    /// 构造函数，创建一个查询信息类的实例。
    /// </summary>
    /// <param name="id">查询字段ID。</param>
    /// <param name="type">查询操作类型。</param>
    /// <param name="value">查询字段值。</param>
    public QueryInfo(string id, QueryType type, string value)
    {
        Id = id;
        Type = type;
        Value = value;
    }

    internal QueryInfo(ColumnInfo column)
    {
        Id = column.Id;
        Type = QueryType.Contain;
        Value = "";
        if (!column.IsQueryAll)
        {
            var codes = Cache.GetCodes(column.Category);
            if (codes != null && codes.Count > 0)
                Value = codes[0].Code;
        }
    }

    /// <summary>
    /// 取得或设置查询字段ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置查询字段值。
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// 取得或设置查询操作类型。
    /// </summary>
    public QueryType Type { get; set; }

    internal object ParamValue { get; set; }
}

/// <summary>
/// 系统布局模式枚举。
/// </summary>
public enum LayoutMode
{
    /// <summary>
    /// 纵向菜单布局。
    /// </summary>
    [Display(Name = "纵向")]
    Vertical,
    /// <summary>
    /// 横向菜单布局。
    /// </summary>
    [Display(Name = "横向")]
    Horizontal,
    /// <summary>
    /// 浮动菜单布局。
    /// </summary>
    [Display(Name = "浮动")]
    Float
}

/// <summary>
/// 系统用户设置信息类。
/// </summary>
public class UserSettingInfo
{
    /// <summary>
    /// 构造函数，创建一个系统设置信息类的实例。
    /// </summary>
    public UserSettingInfo()
    {
        Reset();
    }

    /// <summary>
    /// 取得或设置系统当前字体大小。
    /// </summary>
    public string Size { get; set; }

    /// <summary>
    /// 取得或设置系统当前语言。
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// 取得或设置系统当前主题。
    /// </summary>
    public string Theme { get; set; }

    /// <summary>
    /// 取得或设置系统是否多标签页模式。
    /// </summary>
    public bool MultiTab { get; set; }

    /// <summary>
    /// 取得或设置系统菜单是否是手风琴， 默认是。
    /// </summary>
    public bool Accordion { get; set; } = true;

    /// <summary>
    /// 取得或设置系统菜单是否收缩。
    /// </summary>
    public bool Collapsed { get; set; }

    /// <summary>
    /// 取得或设置系统菜单主题，默认亮色（Light）。
    /// </summary>
    public string MenuTheme { get; set; } = "Light";

    /// <summary>
    /// 取得或设置系统主题颜色。
    /// </summary>
    public string ThemeColor { get; set; } = "default";

    /// <summary>
    /// 取得或设置系统布局模式。
    /// </summary>
    public LayoutMode LayoutMode { get; set; }

    /// <summary>
    /// 取得或设置是否显示页面底部，默认否。
    /// </summary>
    public bool ShowFooter { get; set; }

    internal void Reset()
    {
        MultiTab = false;
        Accordion = true;
        Collapsed = false;
        MenuTheme = "Light";
        Config.OnSetting?.Invoke(this);
    }
}

/// <summary>
/// 表格列设置信息类。
/// </summary>
public class TableSettingInfo
{
    /// <summary>
    /// 取得或设置栏位ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置栏位是否可见。
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// 取得或设置栏位宽度。
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// 取得或设置栏位显示顺序。
    /// </summary>
    public int Sort { get; set; }
}