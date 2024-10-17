﻿namespace Known.Services;

/// <summary>
/// 无代码模块服务接口。
/// </summary>
public interface IAutoService : IService
{
    /// <summary>
    /// 异步分页查询数据。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<Dictionary<string, object>>> QueryModelsAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步删除数据。
    /// </summary>
    /// <param name="info">删除对象。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteModelsAsync(AutoInfo<List<Dictionary<string, object>>> info);

    /// <summary>
    /// 异步保存数据。
    /// </summary>
    /// <param name="info">保存表单对象。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveModelAsync(UploadInfo<Dictionary<string, object>> info);

    /// <summary>
    /// 异步创建数据库表。
    /// </summary>
    /// <param name="info">建表脚本对象。</param>
    /// <returns>创建结果。</returns>
    Task<Result> CreateTableAsync(AutoInfo<string> info);
}

class AutoService(Context context) : ServiceBase(context), IAutoService
{
    public Task<PagingResult<Dictionary<string, object>>> QueryModelsAsync(PagingCriteria criteria)
    {
        var tableName = criteria.GetParameter<string>("TableName");
        if (string.IsNullOrWhiteSpace(tableName))
        {
            var pageId = criteria.GetParameter<string>("PageId");
            tableName = DataHelper.GetEntityByModuleId(pageId)?.Id;
        }

        if (string.IsNullOrWhiteSpace(tableName))
            return Task.FromResult(new PagingResult<Dictionary<string, object>>());

        criteria.SetQuery(nameof(EntityBase.CompNo), QueryType.Equal, CurrentUser?.CompNo);
        return Database.QueryPageAsync(tableName, criteria);
    }

    public async Task<Result> DeleteModelsAsync(AutoInfo<List<Dictionary<string, object>>> info)
    {
        var tableName = DataHelper.GetEntityByModuleId(info.PageId)?.Id;
        if (string.IsNullOrWhiteSpace(tableName))
            return Result.Error(Language.Required("tableName"));

        if (info.Data == null || info.Data.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var oldFiles = new List<string>();
        var result = await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in info.Data)
            {
                var id = item.GetValue<string>("Id");
                await Platform.DeleteFlowAsync(db, id);
                await Platform.DeleteFilesAsync(db, id, oldFiles);
                await db.DeleteAsync(tableName, id);
            }
        });
        if (result.IsValid)
            Platform.DeleteFiles(oldFiles);
        return result;
    }

    public async Task<Result> SaveModelAsync(UploadInfo<Dictionary<string, object>> info)
    {
        var tableName = DataHelper.GetEntityByModuleId(info.PageId)?.Id;
        if (string.IsNullOrWhiteSpace(tableName))
            return Result.Error(Language.Required("tableName"));

        var model = info.Model;
        var vr = DataHelper.Validate(Context, tableName, model);
        if (!vr.IsValid)
            return vr;

        var user = CurrentUser;
        return await Database.TransactionAsync(Language.Save, async db =>
        {
            var id = model.GetValue<string>(nameof(EntityBase.Id));
            if (string.IsNullOrWhiteSpace(id))
                id = Utils.GetGuid();
            if (info.Files != null && info.Files.Count > 0)
            {
                foreach (var file in info.Files)
                {
                    var bizType = $"{tableName}.{file.Key}";
                    var files = info.Files.GetAttachFiles(user, file.Key, tableName);
                    await Platform.AddFilesAsync(db, files, id, bizType);
                    model[file.Key] = $"{id}_{bizType}";
                }
            }
            DataHelper.SetValue(model, nameof(EntityBase.Id), id);
            await db.SaveAsync(tableName, model);
        }, model);
    }

    public async Task<Result> CreateTableAsync(AutoInfo<string> info)
    {
        try
        {
            var database = Database;
            var tableName = info.PageId;
            var script = info.Data;
            try
            {
                var sql = $"select count(*) from {tableName}";
                var count = await database.ScalarAsync<int>(sql);
                if (count > 0)
                    return Result.Error(Language["Tip.TableHasData"]);

                sql = $"drop table {tableName}";
                await database.ExecuteAsync(sql);
            }
            catch
            {
            }

            await database.ExecuteAsync(script);
            return Result.Success(Language["Tip.ExecuteSuccess"]);
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
            return Result.Error(ex.Message);
        }
    }
}