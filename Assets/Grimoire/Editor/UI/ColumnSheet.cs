using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Grimoire.Inspector {
	[UxmlElement]
	public partial class ColumnSheet : VisualElement {
		public static string uxml_path = GrimoireWindow.start_path + "Editor/UI/ColumnSheet.uxml";

		public ColumnSheet() {
			RegisterCallbackOnce<GeometryChangedEvent>(OnGeometryChanged);
		}

		private void OnGeometryChanged(GeometryChangedEvent ev) {
			var q = this.Q<QueryBox>();
			q.queryField.value = "t:Monster";
			q.queryButton.RegisterCallback<ClickEvent>(ev => {
				var resultContainer = this.Q<VisualElement>("result-container");
				resultContainer.Clear();
				foreach (var guid in AssetDatabase.FindAssets(q.queryField.value)) {
					var asset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid));
					if (asset != null) {
						var ve = new VisualElement();
						ve.AddToClassList("result-column");
						var so = new SerializedObject(asset);
						ve.Add(new Label(asset.name));
						var inspector = Editor.CreateEditor(so.targetObject).CreateInspectorGUI();
						inspector.Bind(so);
						ve.Add(inspector);
						resultContainer.Add(ve);
					}
				}
			});
		}
	}
}
