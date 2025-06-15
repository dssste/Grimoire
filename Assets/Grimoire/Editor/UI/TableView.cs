using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Grimoire.Inspector {
	public class TableView : VisualElement {
		public Dictionary<int, Dictionary<int, object>> colsData;

		private Dictionary<int, List<VisualElement>> rows = new();

		public TableView() {
			style.flexDirection = FlexDirection.Row;
		}

		public void Rebuild() {
			Clear();
			rows.Clear();
			if (colsData.Count <= 0) return;

			var maxRowIndex = colsData.Max(kvp => kvp.Key);
			var maxColIndex = colsData.Max(kvp => kvp.Value.Max(kvp => kvp.Key));

			for (int i = 0; i <= maxRowIndex; i++) {
				var colContainer = new VisualElement();
				Add(colContainer);

				if (!colsData.TryGetValue(i, out var col)) continue;

				for (int j = 0; j <= maxColIndex; j++) {
					var cellContainer = new VisualElement();
					cellContainer.style.borderBottomWidth = 1f;
					cellContainer.style.borderBottomColor = Color.gray;
					cellContainer.style.borderRightWidth = 1f;
					cellContainer.style.borderRightColor = Color.gray;
					colContainer.Add(cellContainer);
					if (!rows.TryGetValue(j, out var row)) {
						row = new();
						rows[j] = row;
					}
					rows[j].Add(cellContainer);

					col.TryGetValue(j, out var cellData);
					cellContainer.Add(cellData switch {
						string s => new Label(s),
						VisualElement ve => ve,
						System.Func<VisualElement> func => func(),
						_ => new VisualElement(),
					});
				}
			}
			RegisterCallbackOnce<GeometryChangedEvent>(_ => {
				foreach (var (j, cells) in rows) {
					var height = cells.Max(ve => {
						return ve[0].resolvedStyle.height + 3;
					});
					cells.ForEach(cell => {
						cell.style.minHeight = height;
						if (cell[0] is PropertyField pf) {
							pf.RegisterCallback<GeometryChangedEvent, VisualElement>(OnCellGeometryChanged, cell);
						}
					});
				}
			});
		}

		private void OnCellGeometryChanged(GeometryChangedEvent ev, VisualElement cell) {
			foreach (var (j, cells) in rows) {
				if (cells.Contains(cell)) {
					var height = cells.Max(ve => {
						return cell[0].resolvedStyle.height + 3;
					});
					cells.ForEach(cell => {
						cell.style.minHeight = height;
					});
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
			foreach (var (i, col) in colsData.OrderBy(kvp => kvp.Key)) {
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
}
