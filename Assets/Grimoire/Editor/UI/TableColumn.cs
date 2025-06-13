using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TableColumn : VisualElement {
	private readonly List<TableHeader> rows = new();
	public delegate void RowClicked(int col, int row);

	public event RowClicked OnRowClicked;

	public int ColumnId = 0;

	public TableColumn(string name, Color colmen, Color text, float textSize, Color border, float size, bool rtl, bool state, bool bold) {
		if (state) {
			if (rtl) {
				style.borderLeftColor = border;
				style.borderLeftWidth = size;
			} else {
				style.borderRightColor = border;
				style.borderRightWidth = size;
			}
		}
		style.width = Length.Percent(100);
		TableHeader header = new(name, colmen, text, textSize, rtl, bold);
		header.SetAsColumn(border, size);
		rows.Add(header);
		Add(header);
	}

	public void Set(TableHeader header) {
		rows.Add(header);
		Add(header);
		header.RegisterCallback<MouseUpEvent>((_) => {
			OnRowClicked?.Invoke(ColumnId, IndexOf(header));
			Color def = header.style.backgroundColor.value;
			header.style.backgroundColor = def * .8f;
			header.style.backgroundColor = def;
		});
	}

	public void Set(int index, string val) {
		if (index >= 0 && index < rows.Count) {
			rows[index].UpdateValue(val);
		}
	}

	public void Del(TableHeader header) {
		if (IndexOf(header) == -1) {
			Remove(header);
			rows.Remove(header);
		}
	}

	public string GetHeader() {
		if (rows.Count == 0) return null;
		return rows[0].Value;
	}

	public string GetValue(int row) {
		if (row >= 0 && row < rows.Count) {
			return rows[row].Value;
		}
		return null;
	}
}
