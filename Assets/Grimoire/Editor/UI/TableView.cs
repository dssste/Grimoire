using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class TableView : VisualElement {
	public Dictionary<int, Dictionary<int, object>> cols;

	public TableView() {
		style.flexDirection = FlexDirection.Row;
	}

	public void Rebuild() {
		foreach (var (i, col) in cols.OrderBy(kvp => kvp.Key)) {
			var colContainer = new VisualElement();
			Add(colContainer);
			foreach (var (j, cell) in col.OrderBy(kvp => kvp.Key)) {
				var cellContainer = new VisualElement();
				cellContainer.style.borderBottomWidth = 1f;
				cellContainer.style.borderBottomColor = Color.gray;
				cellContainer.style.borderRightWidth = 1f;
				cellContainer.style.borderRightColor = Color.gray;
				cellContainer.Add(cell switch {
					string s => new Label(s),
					VisualElement ve => ve,
					System.Func<VisualElement> func => func(),
					_ => new Label("empty cell"),
				});
				colContainer.Add(cellContainer);
			}
		}
	}

	public static Dictionary<int, Dictionary<int, object>> Transpose(Dictionary<int, Dictionary<int, object>> from) {
		var to = new Dictionary<int, Dictionary<int, object>>();
		foreach (var (i, col) in from) {
			foreach (var (j, cell) in col) {
				if (!to.TryGetValue(j, out var row)) {
					row = new();
					to[j] = row;
				}
				row[i] = cell;
			}
		}
		return to;
	}

	private void InspectCols() {
		var sb = new System.Text.StringBuilder();
		foreach (var (i, col) in cols.OrderBy(kvp => kvp.Key)) {
			sb.Clear();
			sb.Append(i + ": ");
			foreach (var (j, cell) in col.OrderBy(kvp => kvp.Key)) {
				sb.Append(j + " " + cell switch {
					string s => s,
					VisualElement ve => ve.GetType(),
					System.Func<VisualElement> func => func().GetType(),
					_ => new Label("empty cell"),
				} + ", ");
			}
			Debug.Log(sb);
		}
	}

}
