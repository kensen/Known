﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Known.Entities;

/// <summary>
/// 系统用户实体类。
/// </summary>
public class SysUser : EntityBase
{
    public SysUser()
    {
        Enabled = true;
        Gender = GenderType.Female.ToString();
    }

    /// <summary>
    /// 取得或设置组织编码。
    /// </summary>
    [MaxLength(50)]
    public string OrgNo { get; set; }

    /// <summary>
    /// 取得或设置用户名。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string UserName { get; set; }

    /// <summary>
    /// 取得或设置密码。
    /// </summary>
    [MaxLength(50)]
    public string Password { get; set; }

    /// <summary>
    /// 取得或设置姓名。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置英文名。
    /// </summary>
    [MaxLength(50)]
    public string EnglishName { get; set; }

    /// <summary>
    /// 取得或设置性别。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Gender { get; set; }

    /// <summary>
    /// 取得或设置固定电话。
    /// </summary>
    [MaxLength(50)]
    [Regex(RegexPattern.Phone, "固定电话格式不正确！")]
    public string Phone { get; set; }

    /// <summary>
    /// 取得或设置移动电话。
    /// </summary>
    [MaxLength(50)]
    [Regex(RegexPattern.Mobile, "移动电话格式不正确！")]
    public string Mobile { get; set; }

    /// <summary>
    /// 取得或设置电子邮件。
    /// </summary>
    [MaxLength(50)]
    [Regex(RegexPattern.Email, "电子邮件格式不正确！")]
    public string Email { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [Required]
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [MaxLength(500)]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置首次登录时间。
    /// </summary>
    public DateTime? FirstLoginTime { get; set; }

    /// <summary>
    /// 取得或设置首次登录IP。
    /// </summary>
    [MaxLength(50)]
    public string FirstLoginIP { get; set; }

    /// <summary>
    /// 取得或设置最近登录时间。
    /// </summary>
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// 取得或设置最近登录IP。
    /// </summary>
    [MaxLength(50)]
    public string LastLoginIP { get; set; }

    /// <summary>
    /// 取得或设置类型。
    /// </summary>
    [MaxLength(500)]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置角色。
    /// </summary>
    [MaxLength(500)]
    public string Role { get; set; }

    /// <summary>
    /// 取得或设置数据。
    /// </summary>
    public string Data { get; set; }

    [Form(Type = "CheckList")]
    [Category("Roles")]
    public virtual string[] RoleIds { get; set; }
    public virtual string[] DataIds { get; set; }

    internal virtual List<CodeInfo> Roles { get; set; }
    internal virtual List<CodeInfo> Datas { get; set; }
}