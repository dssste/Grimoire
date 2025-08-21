using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Grimoire.Inspector {
	[UxmlElement, RegisterGrimoireSheet(key = "Inspector Column")]
	public partial class ColumnSheet : VisualElement, IGrimoireSheet {
		private static string uxml_path = "Editor/UI/ColumnSheet.uxml";

		public ColumnSheet() {
			Add(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(GrimoireWindow.start_path + uxml_path).Instantiate());
		}

		public IEnumerable<Object> assets { get; set; }

		public void Rebuild() {
			var resultContainer = this.Q<ScrollView>("result-container");
			resultContainer.Clear();

			foreach (var asset in assets) {
				if (asset != null) {
					var ve = new VisualElement();
					ve.AddToClassList("result-column");
					var so = new SerializedObject(asset);
					var objectField = new ObjectField();
					objectField.value = asset;
					objectField.SetEnabled(false);
					ve.Add(objectField);
					var editor = Editor.CreateEditor(so.targetObject);
					var inspector = editor.CreateInspectorGUI();
					if (inspector == null) {
						inspector = new IMGUIContainer(() => {
							editor.OnInspectorGUI();
						});
					} else {
						inspector.Bind(so);
					}
					ve.Add(inspector);
					resultContainer.Add(ve);
				}
			}
		}
	}
}
