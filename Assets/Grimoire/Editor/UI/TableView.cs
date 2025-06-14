using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class TableView : VisualElement {
	public Dictionary<int, Dictionary<int, object>> cellsSourceByCol;

	public TableView() {
		style.flexDirection = FlexDirection.Row;
	}

	public void Rebuild() {
		foreach (var (i, col) in cellsSourceByCol.OrderBy(kvp => kvp.Key)) {
			var colContainer = new VisualElement();
			Add(colContainer);
			foreach (var (j, cell) in col.OrderBy(kvp => kvp.Key)) {
				var cellContainer = new VisualElement();
				cellContainer.style.borderBottomWidth = 1f;
				cellContainer.style.borderBottomColor = Color.gold;
				cellContainer.style.borderRightWidth = 1f;
				cellContainer.style.borderRightColor = Color.gold;
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
}
