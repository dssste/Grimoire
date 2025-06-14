using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class TableView : VisualElement {
	private readonly List<TableColumn> columns = new();

	public TableView() {
		style.flexDirection = FlexDirection.Row;
	}

	/// <summary>
	/// Setup columns for your table
	/// </summary>
	/// <param name="col">paramas columns</param>
	public void Set(params string[] col) {
		columns.Clear();
		Clear();
		for (int i = 0; i < col.Length; i++) {
			TableColumn column;
			if (i == col.Length - 1) {
				column = new(col[i]);
			} else {
				column = new(col[i]);
			}
			column.ColumnId = i;
			columns.Add(column);
			Add(column);
		}
	}

	public string GetColumnValue(int col) {
		if (col >= 0 && col < columns.Count)
			return columns[col].GetHeader();
		return null;
	}

	public string GetRowValue(int col, int row) {
		if (col >= 0 && col < columns.Count)
			return columns[col].GetValue(row);
		return null;
	}

	/// <summary>
	/// Set values to index, note the length should be same column length.
	/// </summary>
	/// <param name="values">params values</param>
	public void VSet(params string[] values) {
		if (values.Length != columns.Count) {
			Debug.LogError("When using VSet the values should be same as column length.");
			return;
		}
		for (int i = 0; i < values.Length; i++) {
			columns[i].Set(new(values[i]));
		}
	}

	/// <summary>
	/// Add new row in column
	/// </summary>
	/// <param name="column">index of column</param>
	/// <param name="val">value</param>
	public void Set(int column, string val) {
		if (column >= 0 && column < columns.Count) {
			columns[column].Set(new(val));
		}
	}

	/// <summary>
	/// Update value in tableview
	/// </summary>
	/// <param name="column">index of column</param>
	/// <param name="row">index of row</param>
	/// <param name="val">new value</param>
	public void Set(int column, int row, string val) {
		if (column >= 0 && column < columns.Count) {
			columns[column].Set(row, val);
		}
	}
}
