﻿using System.ComponentModel.DataAnnotations;
using Known.Helpers;

namespace Known;

public class LoginFormInfo
{
    [Required] public string UserName { get; set; }
    [Required] public string Password { get; set; }
    public string ClientId { get; set; }
    public bool Remember { get; set; }
    public bool IsForce { get; set; }
    public bool IsMobile { get; set; }
    public string IPAddress { get; set; }
}

public class PwdFormInfo
{
    [Required] public string OldPwd { get; set; }
    [Required] public string NewPwd { get; set; }
    [Required] public string NewPwd1 { get; set; }
}

public class RoleFormInfo
{
    public dynamic Model { get; set; }
    public List<MenuInfo> Menus { get; set; }
    public List<string> MenuIds { get; set; }
}

public class SettingFormInfo
{
    public string Type { get; set; }
    public string Name { get; set; }
    public string Data { get; set; }
}

public class UploadInfo
{
    public string Name { get; set; }
    public string Type { get; set; }
    public byte[] Data { get; set; }
}

public class UploadFormInfo
{
    public UploadFormInfo(ImportFormInfo model)
    {
        Model = model;
        Files = [];
    }

    public ImportFormInfo Model { get; }
    public Dictionary<string, List<IAttachFile>> Files { get; }
}

public class FileFormInfo
{
    public string Category { get; set; }
    public string BizId { get; set; }
    public string BizName { get; set; }
    public string BizType { get; set; }
    public string BizPath { get; set; }
    public string IsThumb { get; set; }
}

public class ImportFormInfo : FileFormInfo
{
    public string Name { get; set; }
    public string Type { get; set; }
    public bool IsAsync { get; set; }
    public bool IsFinished { get; set; } = true;
    public string Message { get; set; }
    public string Error { get; set; }
    public List<string> Columns { get; set; }

    public List<string> GetImportColumns()
    {
        if (Columns != null && Columns.Count > 0)
            return Columns;

        return GetImportColumns(Type);
    }

    public static List<string> GetImportColumns(string modelType)
    {
        var baseProperties = typeof(EntityBase).GetProperties();
        var attrs = TypeHelper.GetColumnAttributes(modelType);
        return attrs.Where(a => !baseProperties.Any(p => p.Name == a.Property.Name)).Select(a => a.Description).ToList();
    }
}