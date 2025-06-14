using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TableColumn : VisualElement {
	private readonly List<TableHeader> rows = new();
	public delegate void RowClicked(int col, int row);

	public int ColumnId = 0;

	public TableColumn(string name) {
		style.width = Length.Percent(100);
		TableHeader header = new(name);
		rows.Add(header);
		Add(header);
	}

	public void Set(TableHeader header) {
		rows.Add(header);
		Add(header);
		header.RegisterCallback<MouseUpEvent>((_) => {
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
