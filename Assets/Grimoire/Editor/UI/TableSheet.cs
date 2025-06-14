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
			foreach (var guid in assetIds) {
				var asset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid));
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
