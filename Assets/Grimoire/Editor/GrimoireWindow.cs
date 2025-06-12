using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Grimoire.Editor {
	public class GrimoireWindow : EditorWindow {
		private static string uxml_path = "Editor/GrimoireWindow.uxml";

		[MenuItem("Window/Grimoire")]
		public static void ShowHome() {
			var window = GetWindow<GrimoireWindow>();
			var icon = EditorGUIUtility.IconContent("tree_icon_leaf").image;
			window.titleContent = new GUIContent("Grimoire", icon);
		}

		private void CreateGUI() {
			rootVisualElement.Add(new Label("hello"));

			var vta = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.dss.grimoire/" + uxml_path);
			if (vta == null) vta = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Grimoire/" + uxml_path);
			if (vta == null) return;

			rootVisualElement.Add(vta.Instantiate());
		}
	}
}
