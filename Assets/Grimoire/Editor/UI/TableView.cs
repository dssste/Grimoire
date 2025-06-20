using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Grimoire.Inspector {
	public class TableView : VisualElement {
		public static string ussClassName = "table-view";
		public static string cellUssClassName = ussClassName + "__cell";
		public static string cellAlternatedRowsUssClassName = cellUssClassName + "-alternated_rows";
		public static string textDataUssClassName = "text-data";
		public static string columnHeaderCellUssClassName = cellUssClassName + "-column-header";
		public static string rowHeaderCellUssClassName = cellUssClassName + "-row-header";

		public Dictionary<int, Dictionary<int, object>> colsSource;

		private Dictionary<int, List<VisualElement>> rows = new();

		public TableView() {
			style.flexDirection = FlexDirection.Row;
			AddToClassList(ussClassName);
		}

		public void Rebuild() {
			Clear();
			rows.Clear();
			if (colsSource.Count <= 0) return;

			var maxRowIndex = colsSource.Max(kvp => kvp.Key);
			var maxColIndex = colsSource.SelectMany(r => r.Value.Keys).Max();

			for (int i = 0; i <= maxRowIndex; i++) {
				var colContainer = new VisualElement();
				Add(colContainer);

				if (!colsSource.TryGetValue(i, out var col)) continue;

				for (int j = 0; j <= maxColIndex; j++) {
					var cellContainer = new VisualElement();
					cellContainer.AddToClassList(cellUssClassName);
					if (i == 0) {
						cellContainer.AddToClassList(columnHeaderCellUssClassName);
					}
					if (j == 0) {
						cellContainer.AddToClassList(rowHeaderCellUssClassName);
					}
					cellContainer.style.borderRightWidth = 1f;
					cellContainer.style.borderBottomWidth = 1f;
					if (i == 0) {
						cellContainer.style.borderLeftWidth = 1f;
					}
					if (j == 0) {
						cellContainer.style.borderTopWidth = 1f;
					}
					if (j % 2 == 0) {
						cellContainer.AddToClassList(cellAlternatedRowsUssClassName);
					}
					colContainer.Add(cellContainer);
					if (!rows.TryGetValue(j, out var row)) {
						row = new();
						rows[j] = row;
					}
					rows[j].Add(cellContainer);

					col.TryGetValue(j, out var cellData);

					cellContainer.Add(cellData switch {
						string s => CreateTextData(s),
						VisualElement ve => ve,
						System.Func<VisualElement> func => func(),
						null => null,
						_ => CreateTextData("?"),
					});
				}
			}
			RegisterCallbackOnce<GeometryChangedEvent>(_ => {
				foreach (var (j, row) in rows) {
					Align(row);
					row.ForEach(cell => {
						var pf = cell.Q<PropertyField>();
						if (pf != null) {
							pf.RegisterCallback<GeometryChangedEvent, VisualElement>(OnCellGeometryChanged, cell);
						}
					});
				}
			});
		}

		private void OnCellGeometryChanged(GeometryChangedEvent ev, VisualElement cell) {
			foreach (var (j, row) in rows) {
				if (row.Contains(cell)) {
					Align(row);
					return;
				}
			}
		}

		private static Label CreateTextData(string s) {
			var label = new Label(s);
			label.AddToClassList(textDataUssClassName);
			return label;
		}

		private static void Align(List<VisualElement> row) {
			var height = row.Max(ve => {
				var firstChild = ve.Query().Visible().Build().Skip(1).FirstOrDefault();
				if (firstChild == null) {
					return 0;
				} else {
					return firstChild.resolvedStyle.height + firstChild.resolvedStyle.paddingTop + firstChild.resolvedStyle.paddingBottom + firstChild.resolvedStyle.marginTop + firstChild.resolvedStyle.marginBottom + 3;
				}
			});
			row.ForEach(cell => {
				cell.style.minHeight = height;
			});
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
			foreach (var (i, col) in colsSource.OrderBy(kvp => kvp.Key)) {
				sb.Clear();
				sb.Append(i + ": ");
				foreach (var (j, cell) in col.OrderBy(kvp => kvp.Key)) {
					sb.Append(j + " " + cell switch {
						string s => s,
						VisualElement ve => ve.GetType(),
						System.Func<VisualElement> func => func().GetType(),
						_ => "empty cell",
					} + ", ");
				}
				Debug.Log(sb);
			}
		}
	}
}
