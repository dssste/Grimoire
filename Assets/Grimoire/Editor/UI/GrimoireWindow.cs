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
			queryField.SetValueWithoutNotify("t:");
			rootVisualElement.Add(queryField);
			var v0 = new VisualElement();
			v0.style.flexDirection = FlexDirection.Row;
			rootVisualElement.Add(v0);
			queryField.RegisterCallback<ChangeEvent<string>>(ev => {
				v0.Clear();
				string[] guids = AssetDatabase.FindAssets(ev.newValue);
				foreach (string guid in guids) {
					var asset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid));

					if (asset != null) {
						var ve = new VisualElement();
						var so = new SerializedObject(asset);
						ve.Bind(so);
						ve.Add(AlchemyEditor.CreateEditor(so.targetObject).CreateInspectorGUI());
						v0.Add(ve);
					}
				}
			});
		}
	}
}
