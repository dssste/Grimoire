using Alchemy.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace Grimoire.Inspector {
	public class GrimoireWindow : EditorWindow {
		private static string uxml_path = "Editor/UI/GrimoireWindow.uxml";

		[MenuItem("Window/Grimoire")]
		public static void ShowHome() {
			var window = GetWindow<GrimoireWindow>();
			var icon = EditorGUIUtility.IconContent("tree_icon_leaf").image;
			window.titleContent = new GUIContent("Grimoire", icon);
		}

		private void CreateGUI() {
			var vta = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.dss.grimoire/" + uxml_path);
			if (vta == null) vta = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Grimoire/" + uxml_path);
			if (vta == null) return;

			var queryField = new TextField();
			rootVisualElement.Add(queryField);

			var v0 = new VisualElement();
			v0.style.flexDirection = FlexDirection.Row;
			rootVisualElement.Add(v0);

			var v1 = new VisualElement();
			v0.Add(v1);

			var v2 = new VisualElement();
			v2.style.flexDirection = FlexDirection.Row;
			v0.Add(v2);
			queryField.RegisterCallback<ChangeEvent<string>>(ev => {
				v1.Clear();
				v2.Clear();
				foreach (var guid in AssetDatabase.FindAssets(ev.newValue)) {
					var asset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid));

					if (asset != null) {
						var ve = new VisualElement();
						var so = new SerializedObject(asset);
						ve.Add(new Label(asset.name));
						var inspector = AlchemyEditor.CreateEditor(so.targetObject).CreateInspectorGUI();
						inspector.Bind(so);
						inspector.RegisterCallbackOnce<GeometryChangedEvent>(ev => {
							inspector.Query<Label>(className: "unity-property-field__label").ForEach(ve => {
								ve.RemoveFromHierarchy();
								v1.Add(ve);
							});
						});
						ve.Add(inspector);
						v2.Add(ve);
					}
				}
			});

			queryField.value = "t:Monster";
		}
	}
}
