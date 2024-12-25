﻿namespace Known.Plugins;

class NavPluginAction : BaseComponent
{
    private List<ActionInfo> items = [];

    [Parameter] public List<string> Values { get; set; }
    [Parameter] public Func<TopNavInfo, Task<Result>> OnAdded { get; set; }

    internal void SetValues(List<string> values)
    {
        Values = values;
        items = GetActionItems();
        StateChanged();
    }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        items = GetActionItems();
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        var model = new DropdownModel
        {
            Icon = "plus",
            Items = items,
            TriggerType = "Click",
            Tooltip = "添加导航",
            OnItemClick = OnNavbarClickAsync
        };
        builder.Component<AntDropdown>()
               .Set(c => c.Model, model)
               .Set(c => c.Placement, AntDesign.Placement.BottomRight)
               .Build();
    }

    private List<ActionInfo> GetActionItems()
    {
        var plugins = Config.Plugins.Where(p => p.IsNav && !Values.Contains(p.Id)).ToList();
        return plugins.ToActions();
    }

    private async Task OnNavbarClickAsync(ActionInfo info)
    {
        var plugin = Config.Plugins.FirstOrDefault(p => p.Id == info.Id);
        if (plugin == null)
            return;

        if (plugin.IsNavComponent)
        {
            await OnAdded?.Invoke(new TopNavInfo { PluginId = info.Id });
            return;
        }

        var instance = Activator.CreateInstance(plugin.Type) as IPlugin;
        if (instance != null)
        {
            instance.Parent = this;
            instance.Config(data => OnAdded?.Invoke(new TopNavInfo
            {
                PluginId = info.Id,
                Parameters = Utils.ToJson(data)
            }));
        }
    }
}