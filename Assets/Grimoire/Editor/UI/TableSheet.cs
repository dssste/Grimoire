using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			var resultContainer = this.Q<ScrollView>("result-container");
			resultContainer.Clear();

			var rows = new Dictionary<string, Dictionary<int, PropertyField>>();
			for (int i = 0; i < assetIds.Length; i++) {
				var guid = assetIds[i];
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
				if (asset != null) {
					var ve = new VisualElement();
					ve.AddToClassList("result-column");
					var fieldIterator = new SerializedObject(asset).GetIterator();
					fieldIterator.NextVisible(true);
					while (fieldIterator.NextVisible(false)) {
						var displayName = fieldIterator.displayName;
						if (!rows.ContainsKey(displayName)) {
							rows[displayName] = new();
						}
						var field = new PropertyField();
						field.BindProperty(fieldIterator);
						rows[displayName][i] = field;
					}
					resultContainer.Add(ve);
				}
			}

			var sb = new StringBuilder();
			foreach (var kvp in rows) {
				sb.Clear();
				sb.Append(kvp.Key + ": ");
				sb.AppendJoin(", ", kvp.Value.Select(kvp2 => $"{kvp2.Key} {kvp2.Value}"));
				Debug.Log(sb.ToString());
			}
		}
	}
}


// var ve = new VisualElement();
// ve.AddToClassList("result-column");
// var so = new SerializedObject(asset);
// var objectField = new ObjectField();
// objectField.value = asset;
// objectField.SetEnabled(false);
// ve.Add(objectField);
// var editor = Editor.CreateEditor(so.targetObject);
// var inspector = editor.CreateInspectorGUI();
// if (inspector == null) {
// 	Debug.LogWarning($"{path} has no CreateInspectorGUI on it's inspector, and is not added to the table");
// 	continue;
// } else {
// 	inspector.Bind(so);
// 	var header = this.Q<VisualElement>("header");
// 	inspector.RegisterCallbackOnce<GeometryChangedEvent>(ev => {
// 		inspector.Query<Label>(className: "unity-property-field__label").Visible().ForEach(ve => {
// 			ve.RemoveFromHierarchy();
// 			if (!header.Children().Any(c => (c as Label).text == ve.text)) {
// 				header.Add(ve);
// 				ve.pickingMode = PickingMode.Ignore;
// 			}
// 		});
// 	});
// 	inspector.RegisterCallbackOnce<GeometryChangedEvent>(ev => {
// 		inspector.Query<Label>(className: "unity-foldout__text").Visible().ForEach(ve => {
// 			ve.RemoveFromHierarchy();
// 			if (!header.Children().Any(c => (c as Label).text == ve.text)) {
// 				header.Add(ve);
// 				ve.pickingMode = PickingMode.Ignore;
// 			}
// 		});
// 	});
// }
// ve.Add(inspector);
