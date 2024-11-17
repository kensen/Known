﻿namespace Known.Core.Services;

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
                //await Platform.DeleteFlowAsync(db, id);
                await Admin.DeleteFilesAsync(db, id, oldFiles);
                await db.DeleteAsync(tableName, id);
            }
        });
        if (result.IsValid)
            oldFiles.ForEach(AttachFile.DeleteFile);
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
                    await Admin.AddFilesAsync(db, files, id, bizType);
                    model[file.Key] = $"{id}_{bizType}";
                }
            }
            model.SetValue(nameof(EntityBase.Id), id);
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
            return Result.Error(ex.Message);
        }
    }
}