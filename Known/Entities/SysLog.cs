﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Known.Entities;

/// <summary>
/// 系统日志实体类。
/// </summary>
public class SysLog : EntityBase
{
    /// <summary>
    /// 取得或设置操作类型。
    /// </summary>
    [Column(IsGrid = true, IsQuery = true, CodeType = nameof(LogType))]
    [DisplayName("操作类型")]
    [Required(ErrorMessage = "操作类型不能为空！")]
    [MinLength(1), MaxLength(50)]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置操作对象。
    /// </summary>
    [Column(IsGrid = true, IsQuery = true)]
    [DisplayName("操作对象")]
    [Required(ErrorMessage = "操作对象不能为空！")]
    [MinLength(1), MaxLength(50)]
    public string Target { get; set; }

    /// <summary>
    /// 取得或设置操作内容。
    /// </summary>
    [Column(IsGrid = true)]
    [DisplayName("操作内容")]
    public string Content { get; set; }
}