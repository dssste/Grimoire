using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Grimoire.Inspector {
	[UxmlElement]
	public partial class TableSheet : VisualElement, ISheet {
		public static string uxml_path = "Editor/UI/TableSheet.uxml";

		public string[] assetIds { get; set; }

		public void Rebuild() {
			var resultContainer = this.Q<VisualElement>("result-container");
			resultContainer.Clear();

			var rows = new Dictionary<string, Dictionary<int, VisualElement>>();
			for (int i = 0; i < assetIds.Length; i++) {
				var guid = assetIds[i];
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
				if (asset != null) {
					var objectField = new ObjectField();
					objectField.value = asset;
					objectField.SetEnabled(false);

					if (!rows.ContainsKey("Asset")) {
						rows["Asset"] = new();
					}
					rows["Asset"][i] = objectField;

					var fieldIterator = new SerializedObject(asset).GetIterator();
					fieldIterator.NextVisible(true);
					while (fieldIterator.NextVisible(false)) {
						var displayName = fieldIterator.displayName;
						var field = new PropertyField();
						field.BindProperty(fieldIterator);
						if (fieldIterator.type == "LocalizedString") {
							field.label = "Localized String";
							field.style.marginLeft = new Length(-5f);
							field.style.marginRight = new Length(5f);
							field.style.minWidth = new Length(280);
						} else {
							field.label = "";
						}

						if (!rows.ContainsKey(displayName)) {
							rows[displayName] = new();
						}
						rows[displayName][i] = field;
					}
				}
			}

			var tableView = new TableView();
			// tableView.colsData = TableView.Transpose(ToTableRows(rows));
			tableView.colsData = ToTableRows(rows);
			tableView.Rebuild();
			resultContainer.Add(tableView);
		}

		private static Dictionary<int, Dictionary<int, object>> ToTableRows(Dictionary<string, Dictionary<int, VisualElement>> from) {
			var rows = new Dictionary<int, Dictionary<int, object>>();
			int i = 0;
			foreach (var (displayName, row) in from) {
				rows[i] = row.ToDictionary(kvp => kvp.Key + 1, kvp => kvp.Value as object);
				rows[i][0] = displayName;
				i++;
			}
			return rows;
		}
	}
}
