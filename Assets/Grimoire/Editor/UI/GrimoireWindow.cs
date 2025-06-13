using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Grimoire.Inspector {
	public class GrimoireWindow : EditorWindow {
#if (USE_DEV_PATH)
		public static string start_path = "Assets/Grimoire/";
#else
		public static string start_path = "Packages/com.dss.grimoire/";
#endif

		private static string uxml_path = start_path + "Editor/UI/GrimoireWindow.uxml";

		[MenuItem("Window/Grimoire")]
		public static void ShowHome() {
			var window = GetWindow<GrimoireWindow>();
			var icon = EditorGUIUtility.IconContent("tree_icon_leaf").image;
			window.titleContent = new GUIContent("Grimoire", icon);
		}

		private TabView _tabView;
		private TabView tabView => _tabView ??= rootVisualElement.Q<TabView>();

		private void CreateGUI() {
			rootVisualElement.Add(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxml_path).Instantiate());

			AddTab("new tab");

			var tabHeaderContainer = tabView.Q<VisualElement>(className: TabView.headerContainerClassName);
			var newTabButton = new Button();
			newTabButton.text = "+";
			newTabButton.AddToClassList("new-tab-button");
			newTabButton.RegisterCallback<ClickEvent>(ev => {
				AddTab("new tab");
				newTabButton.RemoveFromHierarchy();
				tabHeaderContainer.Add(newTabButton);
			});
			tabHeaderContainer.Add(newTabButton);
		}

		private void AddTab(string name) {
			var tab = new Tab(label: name);
			tab.Add(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ColumnSheet.uxml_path).Instantiate());

			// hijack the closing interaction with config
			tab.closeable = true;
			tab.Q<VisualElement>(className: Tab.closeButtonUssClassName).style.backgroundImage = new StyleBackground(EditorGUIUtility.IconContent("d__Popup@2x").image as Texture2D);
			tab.closing += () => {
				ShowConfig(tab);
				return false;
			};

			tabView.Add(tab);
		}

		private void ShowConfig(Tab tab) {
			tab.Add(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(QueryBox.uxml_path).Instantiate());
			var queryField = tab.Q<TextField>(className: QueryBox.queryFieldUssClassName);
			var refreshButton = tab.Q<Button>(className: QueryBox.refreshButtonUssClassName);
			queryField.value = "t:Monster";
			refreshButton.RegisterCallback<ClickEvent>(ev => {
				var cs = tab.Q<ColumnSheet>();
				cs.data = AssetDatabase.FindAssets(queryField.value);
				cs.Rebuild();
			});
		}

		// foreach (var guid in AssetDatabase.FindAssets(ev.newValue)) {
		// 	var asset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid));
		//
		// 	if (asset != null) {
		// 		var ve = new VisualElement();
		// 		var so = new SerializedObject(asset);
		// 		ve.Add(new Label(asset.name));
		// 		var inspector = AlchemyEditor.CreateEditor(so.targetObject).CreateInspectorGUI();
		// 		inspector.Bind(so);
		// 		inspector.RegisterCallbackOnce<GeometryChangedEvent>(ev => {
		// 			inspector.Query<Label>(className: "unity-property-field__label").ForEach(ve => {
		// 				ve.RemoveFromHierarchy();
		// 				v1.Add(ve);
		// 				ve.pickingMode = PickingMode.Ignore;
		// 			});
		// 		});
		// 		ve.Add(inspector);
		// 		v2.Add(ve);
		// 	}
		// }
		// });
	}
}
