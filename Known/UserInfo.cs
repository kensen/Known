﻿namespace Known;

public class UserInfo
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string EnglishName { get; set; }
    public string Gender { get; set; }
    public string Phone { get; set; }
    public string Mobile { get; set; }
    public string Email { get; set; }
    public string Note { get; set; }
    public bool Enabled { get; set; }
    public DateTime? FirstLoginTime { get; set; }
    public string FirstLoginIP { get; set; }
    public DateTime? LastLoginTime { get; set; }
    public string LastLoginIP { get; set; }
    public string IPName { get; set; }
    public string Token { get; set; }
    public string AvatarUrl { get; set; }
    public string AppId { get; set; }
    public string AppName { get; set; }
    public string CompNo { get; set; }
    public string CompName { get; set; }
    public string OrgNo { get; set; }
    public string OrgName { get; set; }
    public string Type { get; set; }
    public string Role { get; set; }
    public bool IsTenant { get; set; }
    public bool IsAdmin => IsSystemAdmin() || IsTenantAdmin();
    public bool IsSystemAdmin() => UserName == Constants.SysUserName.ToLower();
    public bool IsTenantAdmin() => CompNo == UserName;
    public bool IsGroupUser() => CompNo == OrgNo;
    public bool IsOperation() => Type == Constants.UTOperation;
    public bool HasRole(string role) => !string.IsNullOrWhiteSpace(Role) && Role.Split(',').Contains(role);
}