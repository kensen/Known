﻿using System.Linq.Expressions;

namespace Known.Razor;

public class ColumnBuilder<T>
{
    private readonly Column<T> column = new();

    internal Dictionary<string, Column<T>> Columns { get; }

    public ColumnBuilder()
    {
        Columns = new Dictionary<string, Column<T>>();
    }

    internal ColumnBuilder(Column<T> column)
    {
        this.column = column;
    }

    public List<Column<T>> ToColumns()
    {
        return Columns.Values.ToList();
    }

    public ColumnBuilder<T> Field<TValue>(Expression<Func<T, TValue>> selector, bool isQuery = false)
    {
        var property = TypeHelper.Property(selector);
        var attr = property.GetCustomAttribute<ColumnAttribute>(true);
        var column = new Column<T>(property, attr) { IsQuery = isQuery };
        Columns[column.Id] = column;
        return new ColumnBuilder<T>(column);
    }

    public ColumnBuilder<T> Field(string name, string id, bool isQuery = false)
    {
        var column = new Column<T>(name, id) { IsQuery = isQuery };
        Columns[column.Id] = column;
        return new ColumnBuilder<T>(column);
    }

    public ColumnBuilder<T> Field(ColumnInfo info)
    {
        var column = new Column<T>(info);
        Columns[column.Id] = column;
        return new ColumnBuilder<T>(column);
    }

    public ColumnBuilder<T> Template(Action<RenderTreeBuilder, T> template)
    {
        column.Template = template;
        return this;
    }

    public ColumnBuilder<T> Set(Action<Column<T>> action)
    {
        action.Invoke(column);
        return this;
    }

    public ColumnBuilder<T> Name(string name)
    {
        column.Name = name;
        return this;
    }

    public ColumnBuilder<T> Type(ColumnType type)
    {
        column.Type = type;
        column.SetColumnStyle();
        return this;
    }

    public ColumnBuilder<T> IsVisible(bool isVisible)
    {
        column.IsVisible = isVisible;
        return this;
    }

    public ColumnBuilder<T> IsSum()
    {
        column.IsSum = true;
        return this;
    }

    public ColumnBuilder<T> ReadOnly()
    {
        column.ReadOnly = true;
        return this;
    }

    public ColumnBuilder<T> Fixed()
    {
        column.IsFixed = true;
        column.AddClass("fixed");
        return this;
    }

    public ColumnBuilder<T> Width(int width)
    {
        column.Width = width;
        return this;
    }

    public ColumnBuilder<T> Center(int? width = null)
    {
        if (width != null)
            column.Width = width.Value;
        column.Class("txt-center");
        return this;
    }

    public ColumnBuilder<T> Select(SelectOption select)
    {
        column.Select = select;
        return this;
    }

    public ColumnBuilder<T> Edit(EditOption<T> edit = null)
    {
        column.IsEdit = true;
        column.Edit = edit;
        return this;
    }

    public ColumnBuilder<T> Control<TField>() where TField : Field
    {
        var type = typeof(TField);
        column.Control = type;
        return this;
    }
}