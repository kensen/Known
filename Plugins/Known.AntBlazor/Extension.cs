﻿using Microsoft.Extensions.DependencyInjection;

namespace Known.AntBlazor;

/// <summary>
/// 依赖注入扩展类。
/// </summary>
public static class Extension
{
    /// <summary>
    /// 添加基于AntDesign Blazor的实现框架界面。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">配置选项。</param>
    public static void AddKnownAntDesign(this IServiceCollection services, Action<AntDesignOption> action = null)
    {
        //添加AntDesign
        services.AddAntDesign();
        services.AddScoped<IUIService, UIService>();

        AntConfig.Option = new AntDesignOption();
        action?.Invoke(AntConfig.Option);

        Config.AddModule(typeof(Extension).Assembly);

        KStyleSheet.AddStyle("_content/AntDesign/css/ant-design-blazor.variable.css");
        KStyleSheet.AddStyle("_content/Known.AntBlazor/css/theme/default.css");
        KStyleSheet.AddStyle("_content/Known.AntBlazor/css/size/default.css");
        KStyleSheet.AddStyle("_content/Known.AntBlazor/css/antblazor.css");
        KScript.AddScript("_content/AntDesign/js/ant-design-blazor.js");

        UIConfig.Sizes = [
            new ActionInfo { Id = "Default", Style = "size", Url = "_content/Known.AntBlazor/css/size/default.css" },
            new ActionInfo { Id = "Compact", Style = "size", Url = "_content/Known.AntBlazor/css/size/compact.css" }
        ];
        UIConfig.Icons["AntDesign"] = typeof(IconType.Outline).GetProperties().Select(x => (string)x.GetValue(null)).Where(x => x is not null).ToList();
        UIConfig.FillHeightScript = @"
var height = $('.kui-body').outerHeight(true);
$('.kui-body .kui-card').css('height', (height-20)+'px');
height -= ($('.kui-body .kui-query').outerHeight(true) || 0);
height -= ($('.kui-body .kui-toolbar').outerHeight(true) || 0);
height -= ($('.kui-body .ant-tabs-nav').outerHeight(true) || 0);
$('.kui-body .ant-tabs-tabpane .kui-card').css('height', (height-20)+'px');
height -= ($('.kui-body .ant-table-pagination').outerHeight(true) || 0);
$('.kui-body .ant-table-body').css('height', (height-58.5)+'px');
$('.Compact .kui-body .ant-table-body').css('height', (height-49.5)+'px');";
    }
}

/// <summary>
/// AntDesign配置选项类。
/// </summary>
public class AntDesignOption
{
    /// <summary>
    /// 取得或设置是否显示页面页脚组件。
    /// </summary>
    public bool ShowFooter { get; set; }

    /// <summary>
    /// 取得或设置页面页脚组件。
    /// </summary>
    public RenderFragment Footer { get; set; }
}

class AntConfig
{
    public static AntDesignOption Option { get; set; }
}