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
			var resultContainer = this.Q<ScrollView>("result-container");
			resultContainer.Clear();
			foreach (var guid in assetIds) {
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
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
						Debug.LogWarning($"{path} has no CreateInspectorGUI on it's inspector, and is not added to the table");
						continue;
					} else {
						inspector.Bind(so);
						var header = this.Q<VisualElement>("header");
						inspector.RegisterCallbackOnce<GeometryChangedEvent>(ev => {
							inspector.Query<Label>(className: "unity-property-field__label").Visible().ForEach(ve => {
								ve.RemoveFromHierarchy();
								if (!header.Children().Any(c => (c as Label).text == ve.text)) {
									header.Add(ve);
									ve.pickingMode = PickingMode.Ignore;
								}
							});
						});
						inspector.RegisterCallbackOnce<GeometryChangedEvent>(ev => {
							inspector.Query<Label>(className: "unity-foldout__text").Visible().ForEach(ve => {
								ve.RemoveFromHierarchy();
								if (!header.Children().Any(c => (c as Label).text == ve.text)) {
									header.Add(ve);
									ve.pickingMode = PickingMode.Ignore;
								}
							});
						});
					}
					ve.Add(inspector);
					resultContainer.Add(ve);
				}
			}
		}
	}
}
