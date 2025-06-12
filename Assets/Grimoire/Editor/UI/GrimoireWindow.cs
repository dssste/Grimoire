using Alchemy.Editor;
using UnityEditor;
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
			queryField.RegisterCallback<ChangeEvent<string>>(ev => {
				string[] guids = AssetDatabase.FindAssets($"t:{ev.newValue}");
				foreach (string guid in guids) {
					var asset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid));

					if (asset != null) {
						var editor = AlchemyEditor.CreateEditor(asset);
						rootVisualElement.Add(new IMGUIContainer(() => {
							editor.OnInspectorGUI();
						}));
						rootVisualElement.Add(editor.CreateInspectorGUI());
					}
				}
			});
		}
	}
}
