using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Grimoire.Inspector {
	[UxmlElement]
	[RegisterGrimoireSheet(key = "Table", initHook = nameof(InitNonTransposed))]
	[RegisterGrimoireSheet(key = "Table Transposed")]
	public partial class TableSheet : VisualElement, IGrimoireSheet {
		public static string assetHeaderUssClassName = "asset-header";

		private static string uxml_path = "Editor/UI/TableSheet.uxml";

		public IEnumerable<Object> assets { get; set; }

		private bool isNonTransposed;

		public TableSheet() {
			Add(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(GrimoireWindow.start_path + uxml_path).Instantiate());
		}

		private void InitNonTransposed() {
			isNonTransposed = true;
		}

		public void Rebuild() {
			var resultContainer = this.Q<VisualElement>("result-container");
			resultContainer.Clear();

			var rows = new Dictionary<string, Dictionary<int, VisualElement>>();
			var list = assets.ToList();
			for (int i = 0; i < list.Count; i++) {
				var asset = list[i];
				if (asset != null) {
					var so = new SerializedObject(asset);

					var objectField = new ObjectField();
					objectField.value = asset;
					objectField.RegisterCallback<DragUpdatedEvent>(ev => ev.StopPropagation(), TrickleDown.TrickleDown);
					objectField.RegisterCallback<DragPerformEvent>(ev => ev.StopPropagation(), TrickleDown.TrickleDown);
					objectField.RegisterCallback<DragLeaveEvent>(ev => ev.StopPropagation(), TrickleDown.TrickleDown);
					objectField.RegisterCallback<KeyDownEvent>(ev => ev.StopPropagation(), TrickleDown.TrickleDown);
					objectField.AddToClassList(assetHeaderUssClassName);

					var objectDisplayLabel = objectField.Q<TextElement>(className: "unity-object-field-display__label");
					var mainAsset = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(asset));
					if (mainAsset != null && mainAsset != asset) {
						((INotifyValueChanged<string>)objectDisplayLabel).SetValueWithoutNotify($"{mainAsset.name} / {asset.name}");
					} else if (asset is ScriptableObject) {
						((INotifyValueChanged<string>)objectDisplayLabel).SetValueWithoutNotify(asset.name);
					}

					if (!rows.ContainsKey("Asset")) {
						rows["Asset"] = new();
					}
					rows["Asset"][i] = objectField;

					var scriptProperty = so.FindProperty("m_Script");
					if (scriptProperty != null) {
						var scriptField = new PropertyField(scriptProperty, "");
						scriptField.Bind(so);
						scriptField.SetEnabled(false);
						if (!rows.ContainsKey("Script")) {
							rows["Script"] = new();
						}
						rows["Script"][i] = scriptField;
					}

					var fieldIterator = so.GetIterator();
					fieldIterator.NextVisible(true);
					while (fieldIterator.NextVisible(false)) {
						var displayName = fieldIterator.displayName;
						var field = new PropertyField(fieldIterator);
						field.Bind(so);
						if (fieldIterator.type == "LocalizedString") {
							field.label = "Localized String";
							field.style.marginLeft = new Length(-5f);
							field.style.marginRight = new Length(-1f);
							field.style.minWidth = new Length(280f);
						} else if (fieldIterator.isArray && fieldIterator.type != "string") {
							field.style.marginRight = new Length(3f);
							field.style.minWidth = new Length(62f);
							field.label = "List";
						} else if (fieldIterator.type == "bool") {
							field.style.alignItems = Align.Center;
							field.label = "";
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
			if (isNonTransposed) {
				tableView.colsSource = TableView.Transpose(ToTableRows(rows));
			} else {
				tableView.colsSource = ToTableRows(rows);
			}
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
