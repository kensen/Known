﻿using AntDesign;
using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;

namespace Known.AntBlazor.Components;

public class AntTree : Tree<MenuItem>
{
	[Parameter] public TreeModel Model { get; set; }

	protected override void OnInitialized()
	{
        Model.OnRefresh = RefreshAsync;
        ShowIcon = true;
        DisabledExpression = x => !x.DataItem.Enabled || Model.IsView;
        KeyExpression = x => x.DataItem.Id;
        TitleExpression = x => x.DataItem.Name;
        IconExpression = x => x.DataItem.Icon;
        ChildrenExpression = x => x.DataItem.Children;
        IsLeafExpression = x => x.DataItem.Children?.Count == 0;
        OnClick = this.Callback<TreeEventArgs<MenuItem>>(OnTreeClick);
		OnCheck = this.Callback<TreeEventArgs<MenuItem>>(OnTreeCheck);

        Checkable = Model.Checkable;
        DefaultExpandParent = Model.ExpandRoot;
        //DefaultExpandedKeys = [Model.Data[0].Id];
        DefaultSelectedKeys = Model.SelectedKeys;
        DefaultCheckedKeys = Model.DefaultCheckedKeys;
        DataSource = Model.Data;

        base.OnInitialized();
	}

    private Task RefreshAsync()
    {
        Model.OnModelChanged?.Invoke(Model);
        DataSource = Model.Data;
        DefaultSelectedKeys = Model.SelectedKeys;
        StateHasChanged();
        return Task.CompletedTask;
    }

    private void OnTreeClick(TreeEventArgs<MenuItem> e)
	{
		var item = e.Node.DataItem;
        item.Checked = e.Node.Checked;
        Model.OnNodeClick?.Invoke(item);
	}

	private void OnTreeCheck(TreeEventArgs<MenuItem> e)
	{
		var item = e.Node.DataItem;
		item.Checked = e.Node.Checked;
        Model.OnNodeCheck?.Invoke(item);
    }
}