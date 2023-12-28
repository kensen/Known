﻿using Known.Blazor;
using Known.Demo.Entities;
using Known.Demo.Services;
using Known.Extensions;
using Known.WorkFlows;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Demo.Pages.BizApply;

//业务申请列表
class BaApplyList : BaseTablePage<TbApply>
{
    private ApplyService Service => new() { CurrentUser = CurrentUser };

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
		Table.Form.Width = 800;    //定义表单宽度
		Table.Form.NoFooter = true;//表单不显示默认底部按钮
        Table.RowActions = row => Table.GetFlowRowActions(row); //根据数据状态显示操作按钮
        Table.OnQuery = QueryApplysAsync;
		Table.Column(c => c.BizStatus).Template(BuildBizStatus);//自定义状态列
    }

    //新增按钮事件
    [Action]
    public async void New()
    {
        var row = await Service.GetDefaultApplyAsync(ApplyType.Test);
		Table.NewForm(Service.SaveApplyAsync, row);
    }

    //编辑操作
    [Action] public void Edit(TbApply row) => Table.EditForm(Service.SaveApplyAsync, row);
    //删除操作
    [Action] public void Delete(TbApply row) => Table.Delete(Service.DeleteApplysAsync, row);
    //批量删除操作
    [Action] public void DeleteM() => Table.DeleteM(Service.DeleteApplysAsync);
    //提交审核
    [Action] public void Submit(TbApply row) => this.SubmitFlow(row);
    //撤回
    [Action] public void Revoke(TbApply row) => this.RevokeFlow(row);

	//列表分页查询
	private Task<PagingResult<TbApply>> QueryApplysAsync(PagingCriteria criteria)
	{
		criteria.Parameters[nameof(PageType)] = PageType.Apply;
		return Service.QueryApplysAsync(criteria);
	}

    private void BuildBizStatus(RenderTreeBuilder builder, TbApply row) => UI.BizStatus(builder, row.BizStatus);
}