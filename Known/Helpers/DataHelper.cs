﻿namespace Known.Helpers;

class DataHelper
{
    private static List<SysModule> Modules = [];
    internal static List<EntityInfo> Models = [];
    internal static List<FlowInfo> Flows = [];

    internal static void Initialize(List<SysModule> modules)
    {
        if (modules == null || modules.Count == 0)
            return;

        Modules = modules;
        Models.Clear();
        var models = modules.Where(m => !string.IsNullOrWhiteSpace(m.EntityData) && m.EntityData.Contains('|')).Select(m => m.EntityData).ToList();
        foreach (var item in models)
        {
            var model = GetEntityInfo(item);
            Models.Add(model);
        }

        Flows.Clear();
        var flows = modules.Where(m => !string.IsNullOrWhiteSpace(m.FlowData) && m.FlowData.Contains('|')).Select(m => m.FlowData).ToList();
        foreach (var item in flows)
        {
            var flow = GetFlowInfo(item);
            Flows.Add(flow);
        }
    }

    #region Entity
    internal static EntityInfo GetEntityByModuleId(string moduleId)
    {
        var module = Modules.FirstOrDefault(m => m.Id == moduleId);
        return GetEntity(module?.EntityData);
    }

    internal static EntityInfo GetEntity(string model)
    {
        var info = new EntityInfo();
        if (string.IsNullOrWhiteSpace(model))
            return info;

        if (!model.Contains('|'))
            return Models.FirstOrDefault(m => m.Id == model);

        return GetEntityInfo(model);
    }

    private static EntityInfo GetEntityInfo(string model)
    {
        var info = new EntityInfo();
        var lines = model.Split([.. Environment.NewLine])
                         .Where(s => !string.IsNullOrWhiteSpace(s))
                         .ToArray();

        if (lines.Length > 0)
        {
            var values = lines[0].Split('|').Select(x => x.Trim()).ToArray();
            if (values.Length > 0) info.Name = values[0];
            if (values.Length > 1) info.Id = values[1];
            if (values.Length > 2) info.IsFlow = values[2] == "Y";
        }

        if (lines.Length > 1)
        {
            for (int i = 1; i < lines.Length; i++)
            {
                var field = new FieldInfo();
                var values = lines[i].Split('|').Select(x => x.Trim()).ToArray();
                if (values.Length > 0) field.Name = values[0];
                if (values.Length > 1) field.Id = values[1];
                if (values.Length > 2) field.Type = Utils.ConvertTo<FieldType>(values[2]);
                if (values.Length > 3) field.Length = values[3];
                if (values.Length > 4) field.Required = values[4] == "Y";

                if (field.Type == FieldType.CheckBox || field.Type == FieldType.Switch)
                {
                    field.Length = "50";
                    field.Required = true;
                }

                info.Fields.Add(field);
            }
        }

        return info;
    }
    #endregion

    #region Flow
    internal static FlowInfo GetFlow(string model)
    {
        var info = new FlowInfo();
        if (string.IsNullOrWhiteSpace(model))
            return info;

        if (!model.Contains('|'))
            return Flows.FirstOrDefault(m => m.Id == model);

        return GetFlowInfo(model);
    }

    private static FlowInfo GetFlowInfo(string model)
    {
        var info = new FlowInfo();
        var lines = model.Split([.. Environment.NewLine])
                         .Where(s => !string.IsNullOrWhiteSpace(s))
                         .ToArray();

        if (lines.Length > 0)
        {
            var values = lines[0].Split('|');
            if (values.Length > 0) info.Name = values[0];
            if (values.Length > 1) info.Id = values[1];
        }

        if (lines.Length > 1)
        {
            for (int i = 1; i < lines.Length; i++)
            {
                var step = new FlowStepInfo();
                var values = lines[i].Split('|');
                if (values.Length > 0) step.Name = values[0];
                if (values.Length > 1) step.Id = values[1];
                if (values.Length > 2) step.User = values[2];
                if (values.Length > 3) step.Role = values[3];
                if (values.Length > 4) step.Pass = values[4];
                if (values.Length > 5) step.Fail = values[5];
                info.Steps.Add(step);
            }
        }

        return info;
    }
    #endregion

    #region Dictionary
    internal static object GetValue(Dictionary<string, object> data, string id)
    {
        if (!data.TryGetValue(id, out object value))
        {
            if (!data.TryGetValue(id.ToLower(), out value))
                data.TryGetValue(id.ToUpper(), out value);
        }

        return value;
    }

    internal static void SetValue(Dictionary<string, object> data, string id, object value)
    {
        var key = id;
        if (data.ContainsKey(id.ToLower()))
            key = id.ToLower();
        else if (data.ContainsKey(id.ToUpper()))
            key = id.ToUpper();
        data[key] = value;
    }

    internal static Result Validate(Context context, string tableName, Dictionary<string, object> model)
    {
        var entity = Models.FirstOrDefault(m => m.Id == tableName);
        if (entity == null)
            return Result.Error(context.Language.Required(tableName));

        var dicError = new Dictionary<string, List<string>>();

        foreach (var field in entity.Fields)
        {
            var errors = new List<string>();
            var value = GetValue(model, field.Id);
            var valueString = value == null ? "" : value.ToString().Trim();
            if (field.Required && string.IsNullOrWhiteSpace(valueString))
                errors.Add(context.Language.Required(field.Name));

            if (errors.Count > 0)
                dicError.Add(field.Name, errors);
        }

        if (dicError.Count > 0)
        {
            var result = Result.Error("", dicError);
            foreach (var item in dicError.Values)
            {
                item.ForEach(result.AddError);
            }
            return result;
        }

        return Result.Success("");
    }
    #endregion
}