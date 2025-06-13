using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class TableView : VisualElement {
	private Color tableItemBorderColor = Color.clear;
	private Color tableItemBackgroundColor = Color.clear;
	private Color columnBgColor = Color.clear;
	private Color columnTextColor = Color.black;
	private Color rowTextColor = Color.black;
	private float columnTextSize;
	private float rowTextSize;
	private float tableItemBorderWidth;

	private bool columnTextBold;
	private bool rowTextBold;
	private bool isRtl;

	public delegate void TableViewClickEvent(int col, int row);
	public event TableViewClickEvent OnClickEvent;

	private readonly List<TableColumn> columns = new();

	public Color ItemBorderColor {
		get => tableItemBorderColor;
		set => tableItemBorderColor = value;
	}

	public Color ColumnBackgroundColor {
		get => columnBgColor;
		set => columnBgColor = value;
	}

	public Color ItemBackgroundColor {
		get => tableItemBackgroundColor;
		set => tableItemBackgroundColor = value;
	}

	public Color ColumnTextColor {
		get => columnTextColor;
		set => columnTextColor = value;
	}

	public Color RowTextColor {
		get => rowTextColor;
		set => rowTextColor = value;
	}

	public float ItemBorderWidth {
		get => tableItemBorderWidth;
		set => tableItemBorderWidth = value;
	}

	public float ColumnTextSize {
		get => columnTextSize;
		set => columnTextSize = value;
	}

	public float RowTextSize {
		get => rowTextSize;
		set => rowTextSize = value;
	}

	public bool EnableRtl {
		get => isRtl;
		set {
			isRtl = value;
			if (isRtl)
				style.flexDirection = FlexDirection.RowReverse;
			else
				style.flexDirection = FlexDirection.Row;
		}
	}

	public bool RowTextBold {
		get => rowTextBold;
		set => rowTextBold = value;
	}

	public bool ColumnTextBold {
		get => columnTextBold;
		set => columnTextBold = value;
	}

	public TableView() {
		if (isRtl)
			style.flexDirection = FlexDirection.RowReverse;
		else
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
				column = new(col[i], ColumnBackgroundColor, ColumnTextColor, ColumnTextSize, ItemBorderColor, ItemBorderWidth, isRtl, false, columnTextBold);
			} else {
				column = new(col[i], ColumnBackgroundColor, ColumnTextColor, ColumnTextSize, ItemBorderColor, ItemBorderWidth, isRtl, true, columnTextBold);
			}
			column.ColumnId = i;
			column.OnRowClicked += Column_OnRowClicked;
			columns.Add(column);
			Add(column);
		}
	}

	private void Column_OnRowClicked(int col, int row) {
		OnClickEvent?.Invoke(col, row);
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
			columns[i].Set(new(values[i], ItemBackgroundColor, RowTextColor, RowTextSize, isRtl, rowTextBold));
		}
	}

	/// <summary>
	/// Add new row in column
	/// </summary>
	/// <param name="column">index of column</param>
	/// <param name="val">value</param>
	public void Set(int column, string val) {
		if (column >= 0 && column < columns.Count) {
			columns[column].Set(new(val, ItemBackgroundColor, RowTextColor, RowTextSize, isRtl, rowTextBold));
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

	#region UXML
	[Preserve]
	public new class UxmlFactory : UxmlFactory<TableView, UxmlTraits> { }
	[Preserve]
	public new class UxmlTraits : VisualElement.UxmlTraits {
		private readonly UxmlColorAttributeDescription columnBgColor = new() { name = "column-background-color", defaultValue = Color.clear };
		private readonly UxmlColorAttributeDescription tableItemBgColor = new() { name = "item-background-color", defaultValue = Color.clear };

		private readonly UxmlColorAttributeDescription tableItemTextColor = new() { name = "column-text-color", defaultValue = Color.black };
		private readonly UxmlColorAttributeDescription tableItemRowColor = new() { name = "row-text-color", defaultValue = Color.black };

		private readonly UxmlColorAttributeDescription tableItemBorderColor = new() { name = "item-border-color", defaultValue = Color.clear };
		private readonly UxmlFloatAttributeDescription tableItemBorderWidth = new() { name = "item-border-width" };

		private readonly UxmlFloatAttributeDescription tableColumnTextSize = new() { name = "column-text-size", defaultValue = 18 };
		private readonly UxmlFloatAttributeDescription tableRowTextSize = new() { name = "row-text-size", defaultValue = 18 };
		private readonly UxmlBoolAttributeDescription columnTextBold = new() { name = "column-text-bold" };
		private readonly UxmlBoolAttributeDescription rowTextBold = new() { name = "row-text-bold" };
		private readonly UxmlBoolAttributeDescription tableIsRtl = new() { name = "enable-rtl" };

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
			base.Init(ve, bag, cc);
			TableView parent = (ve as TableView);
			parent.ColumnBackgroundColor = columnBgColor.GetValueFromBag(bag, cc);
			parent.ItemBorderColor = tableItemBorderColor.GetValueFromBag(bag, cc);
			parent.ColumnTextColor = tableItemTextColor.GetValueFromBag(bag, cc);
			parent.RowTextColor = tableItemRowColor.GetValueFromBag(bag, cc);
			parent.ColumnTextSize = tableColumnTextSize.GetValueFromBag(bag, cc);
			parent.RowTextSize = tableRowTextSize.GetValueFromBag(bag, cc);
			parent.ItemBackgroundColor = tableItemBgColor.GetValueFromBag(bag, cc);
			parent.ItemBorderWidth = tableItemBorderWidth.GetValueFromBag(bag, cc);
			parent.EnableRtl = tableIsRtl.GetValueFromBag(bag, cc);
			parent.ColumnTextBold = columnTextBold.GetValueFromBag(bag, cc);
			parent.RowTextBold = rowTextBold.GetValueFromBag(bag, cc);
		}
	}
	#endregion
}
