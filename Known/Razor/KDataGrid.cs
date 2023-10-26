﻿namespace Known.Razor;

public class KDataGrid<TItem> : DataComponent<TItem>
{
    private readonly string qvAdvQueryId;
    private Table<TItem> table;
    private bool shouldRender = true;
    internal List<Column<TItem>> GridColumns;
    internal string GridId;
    internal int CurRow = -1;
    internal bool CheckAll = false;

    public KDataGrid()
    {
        var id = Utils.GetGuid();
        qvAdvQueryId = $"qv-{id}";
        ViewId = $"gv-{id}";
        GridId = $"dg-{id}";
        ContainerStyle = "grid-view";
        ContentStyle = "grid";
        InitMenu();
    }

    [Parameter] public Action<object> OnPicked { get; set; }

    public List<Column<TItem>> Columns { get; set; }
    public List<ButtonInfo> Actions { get; set; }
    public Action<RenderTreeBuilder> ActionHead { get; set; }
    public Action<RenderTreeBuilder> CustHead { get; set; }
    public bool IsEdit { get; set; }
    public bool IsFixed { get; set; } = true;
    public bool IsSort { get; set; } = true;
    public bool ShowEmpty { get; set; } = true;
    public bool ShowSetting { get; set; }
    public bool ShowCheckBox { get; set; }
    public string OrderBy { get; set; }
    internal string RowTitle { get; set; }

    protected virtual void BuildQueryExts(RenderTreeBuilder builder) { }
    public virtual void View(TItem item) { }
    public virtual void Import() => ShowImport(Name, typeof(TItem));
    public virtual async void Export() => await ExportByModeAsync(ExportMode.Query);
    public virtual bool CheckAction(ButtonInfo action, TItem item) => true;
    public virtual void OnRowClick(int row, TItem item) { }
    public virtual void OnRowDoubleClick(int row, TItem item) { }

    protected override bool ShouldRender() => shouldRender;

    public override void Refresh()
    {
        query = null;
        QueryData();
    }

    protected void SetGridPicker()
    {
        ShowSetting = false;
        ShowCheckBox = false;
        RowTitle = "双击选择数据。";
        if (HasButton(ToolButton.New))
            Tools = new List<ButtonInfo> { ToolButton.New };
        Actions = null;
        Columns?.ForEach(c => c.IsAdvQuery = false);
    }

    protected void SetEdit(bool isEdit)
    {
        IsEdit = isEdit;
        foreach (var item in GridColumns)
        {
            item.IsEdit = isEdit;
        }
        StateChanged();
    }

    public void SetColumn(string id, bool isVisible)
    {
        if (GridColumns == null || GridColumns.Count == 0)
            return;

        var column = GridColumns.FirstOrDefault(c => c.Id == id);
        if (column == null)
            return;

        column.IsVisible = isVisible;
        StateChanged();
    }

    protected void SetColumns(List<Column<TItem>> columns)
    {
        GridColumns = columns;
        HasQuery = GridColumns != null && GridColumns.Any(c => c.IsQuery);
        StateChanged();
    }

    protected ColumnBuilder<TItem> Column<TValue>(Expression<Func<TItem, TValue>> selector)
    {
        var property = TypeHelper.Property(selector);
        var column = Columns?.FirstOrDefault(c => c.Id == property.Name);
        if (column == null)
            return new ColumnBuilder<TItem>();

        return new ColumnBuilder<TItem>(column);
    }

    protected override List<string> GetSumColumns()
    {
        if (GridColumns == null || GridColumns.Count == 0)
            return null;

        return GridColumns.Where(c => c.IsSum).Select(c => c.Id).ToList();
    }

    protected void ShowForm<T>(string title, object model, Size? size = null, Action<AttributeBuilder<T>> action = null) where T : KForm
    {
        UI.ShowForm(title, model, CloseForm, size, action);
    }

    protected virtual void CloseForm(Result result)
    {
        if (result.IsClose)
            UI.CloseDialog();

        Refresh();
    }

    protected void SelectRow(Action<TItem> action)
    {
        var selected = SelectedItems;
        if (!selected.Any() || selected.Count > 1)
        {
            UI.Toast(Language.SelectOne);
            return;
        }
        action.Invoke(selected[0]);
    }

    protected void SelectRows(Action<List<TItem>> action)
    {
        var selected = SelectedItems;
        if (!selected.Any())
        {
            UI.Toast(Language.SelectOneAtLeast);
            return;
        }
        action.Invoke(selected);
    }

    protected void DeleteRow(TItem item, Func<List<TItem>, Task<Result>> action)
    {
        UI.Confirm("确定要删除？", async () =>
        {
            var result = await action.Invoke(new List<TItem> { item });
            UI.Result(result, Refresh);
        });
    }

    protected void DeleteRows(Func<List<TItem>, Task<Result>> action)
    {
        SelectRows(items =>
        {
            UI.Confirm($"确定要删除选中的{items.Count}条记录？", async () =>
            {
                var result = await action?.Invoke(items);
                UI.Result(result, Refresh);
            });
        });
    }

    protected async void MoveRow(TItem item, bool isMoveUp, Func<TItem, Task<Result>> action = null, Action<TItem, TItem> success = null)
    {
        var index = Data.IndexOf(item);
        var index1 = isMoveUp ? index - 1 : index + 1;
        if (index1 < 0 || index1 > Data.Count - 1)
            return;

        if (action != null)
        {
            var result = await action(item);
            if (result.IsValid)
                OnMoveRow(item, success, index, index1);
        }
        else
        {
            OnMoveRow(item, success, index, index1);
        }
    }

    protected async void ShowImport(string name, Type type, bool isAsync = true, string param = null)
    {
        var id = $"{type.Name}Import";
        if (!string.IsNullOrWhiteSpace(param))
            id += $"_{param}";
        var model = await Platform.File.GetImportAsync(id);
        model.BizName = $"导入{name}";
        model.Type = type?.AssemblyQualifiedName;
        model.IsAsync = isAsync;
        UI.ShowImport(new ImportOption
        {
            Id = id,
            Name = name,
            Model = model,
            OnSuccess = Refresh
        });
    }

    protected override Task InitPageAsync()
    {
        if (OnPicked != null)
            SetGridPicker();

        GridColumns = Setting.GetUserColumns(Id, Columns);
        HasTool = Tools != null && Tools.Count > 0;
        HasQuery = GridColumns != null && GridColumns.Any(c => c.IsQuery);

        if (!string.IsNullOrWhiteSpace(OrderBy))
            OrderBys = new string[] { OrderBy };

        return base.InitPageAsync();
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            UI.InitTable(ViewId);

        UI.SetTable(ViewId);
        return base.OnAfterRenderAsync(firstRender);
    }

    internal override void BuildQuery(RenderTreeBuilder builder)
    {
        if (!HasQuery)
            return;

        builder.Div("query", attr => builder.Cascading(QueryContext, BuildQuerys));
    }

    internal override void BuildContent(RenderTreeBuilder builder)
    {
        var css = CssBuilder.Default("table").AddClass("fixed", IsFixed).Build();
        builder.Div(css, attr =>
        {
            builder.Cascading(this, b => b.Component<Table<TItem>>().Build(value => table = value));
        });
    }

    internal override void BuildPager(RenderTreeBuilder builder)
    {
        base.BuildPager(builder);
        BuildAdvQuery(builder);
    }

    private void BuildQuerys(RenderTreeBuilder builder)
    {
        var columns = GridColumns?.Where(c => c.IsQuery).ToList();
        if (columns == null || columns.Count == 0)
            return;

        foreach (var item in columns)
        {
            item.BuildQuery(builder, "", this);
        }

        BuildQueryExts(builder);
        builder.Button(FormButton.Query, Callback(OnQuery));

        if (HasAdvQuery())
            builder.Button(FormButton.AdvQuery, Callback(ShowAdvQuery), style: "qvtrigger");
    }

    internal override void RefreshData() => table?.Changed();

    private void OnQuery()
    {
        shouldRender = false;
        query = null;
        QueryData(true);
    }

    internal bool HasFoot()
    {
        if (GridColumns == null || GridColumns.Count == 0)
            return false;

        var columns = GridColumns.Where(c => c.IsSum).ToList();
        if (columns != null && columns.Count > 0)
            return true;

        return false;
    }

    internal bool HasAction()
    {
        if (IsEdit && ReadOnly)
            return false;

        if (Actions == null || Actions.Count == 0)
            return false;

        return true;
    }

    private bool HasAdvQuery() => GridColumns != null && GridColumns.Any(c => c.IsAdvQuery);

    private void OnMoveRow(TItem item, Action<TItem, TItem> success, int index, int index1)
    {
        CurRow = index1;
        var temp = Data[index1];
        Data[index1] = item;
        Data[index] = temp;
        success?.Invoke(item, temp);
        StateChanged();
    }

    internal void OnSort(ColumnInfo item, string curOrder)
    {
        OrderBys = new string[] { $"{item.Id} {curOrder}" };
        QueryData();
        StateChanged();
    }

    private void BuildAdvQuery(RenderTreeBuilder builder)
    {
        if (!HasAdvQuery())
            return;

        builder.Component<KQuickView>()
               .Set(c => c.Id, qvAdvQueryId)
               .Set(c => c.Style, "query-adv")
               .Set(c => c.ChildContent, b =>
               {
                   b.Component<AdvQuery<TItem>>()
                    .Set(c => c.PageId, Id)
                    .Set(c => c.Columns, Columns)
                    .Set(c => c.OnSetting, value =>
                    {
                        query = value;
                        QueryData(true);
                    })
                    .Build();
               })
               .Build();
    }

    private void ShowAdvQuery()
    {
        shouldRender = false;
        UI.ShowQuickView(qvAdvQueryId);
    }

    internal void ShowColumnSetting()
    {
        var menu = GetPageMenu();
        UI.Show<ColumnSetting>("表格设置", new(800, 500), action: attr =>
        {
            attr.Set(c => c.PageId, Id)
                .Set(c => c.PageColumns, menu?.Columns)
                .Set(c => c.OnSetting, () =>
                {
                    var columns = Setting.GetUserColumns(Id, Columns);
                    SetColumns(columns);
                });
        });
    }

    protected Task ExportByModeAsync(ExportMode mode) => ExportDataAsync(Name, mode);


    protected async Task ExportDataAsync(string name, ExportMode mode, string extension = null)
    {
        criteria.ExportMode = mode;
        criteria.ExportExtension = extension;
        criteria.ExportColumns = GetExportColumns();
        var result = await OnQueryDataAsync(criteria);
        var bytes = result.ExportData;
        if (bytes == null || bytes.Length == 0)
        {
            UI.Alert("无数据可导出！");
            return;
        }

        var stream = new MemoryStream(bytes);
        UI.DownloadFile($"{name}.xlsx", stream);
    }

    private Dictionary<string, string> GetExportColumns()
    {
        var columns = new Dictionary<string, string>();
        if (GridColumns == null || GridColumns.Count == 0)
            return columns;

        foreach (var item in GridColumns)
        {
            columns.Add(item.Id, item.Name);
        }
        return columns;
    }

    private void InitMenu()
    {
        var menu = GetPageMenu();
        if (menu == null)
            return;

        Id = menu.Id;
        Name = menu.Name;
        if (menu.Buttons != null && menu.Buttons.Count > 0)
            Tools = menu.Buttons.Select(n => ToolButton.Buttons.FirstOrDefault(b => b.Id == n || b.Name == n)).ToList();
        if (menu.Actions != null && menu.Actions.Count > 0)
            Actions = menu.Actions.Select(n => GridAction.Actions.FirstOrDefault(b => b.Id == n || b.Name == n)).ToList();
        Columns = menu.Columns?.Select(c => new Column<TItem>(c)).ToList();
    }

    private MenuInfo GetPageMenu()
    {
        if (KRConfig.UserMenus == null)
            return null;

        var type = GetType();
        return KRConfig.UserMenus.FirstOrDefault(m => m.Target == type.FullName);
    }
}

public class KDataGrid<TItem, TForm> : KDataGrid<TItem> where TItem : EntityBase, new() where TForm : BaseForm<TItem>
{
    public KDataGrid()
    {
        ShowSetting = true;
        ShowCheckBox = true;
    }

    public override void View(TItem row) => View(row, true);
    protected virtual Task<TItem> GetDefaultModelAsync() => Task.FromResult(new TItem());
    protected virtual void ShowForm(TItem model = null) => ShowForm(model, true);

    protected virtual async void ShowForm(TItem model, bool showInDialog, string title = null)
    {
        var action = model == null || model.IsNew ? "新增" : "编辑";
        var actionName = $"{action}{Name}";
        if (!string.IsNullOrWhiteSpace(title))
            actionName += $"-{title}";
        model ??= await GetDefaultModelAsync();
        if (showInDialog)
            ShowForm<TForm>(actionName, model);
        //else
        //    Context.Navigate<TItem, TForm>(actionName, "", model, false, CloseForm);
    }

    protected void View(TItem model, bool showInDialog, string title = null)
    {
        var actionName = $"查看{Name}";
        if (!string.IsNullOrWhiteSpace(title))
            actionName += $"-{title}";
        if (showInDialog)
            UI.ShowForm<TForm>(actionName, model);
        //else
        //    Context.Navigate<TItem, TForm>(actionName, "", model, true);
    }
}