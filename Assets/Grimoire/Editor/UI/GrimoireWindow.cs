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
			var header = tab.Q<VisualElement>(className: Tab.tabHeaderUssClassName);
			tab.closing += () => {
				ShowConfig(tab, EditorGUIUtility.GUIToScreenRect(header.worldBound));
				return false;
			};

			tabView.Add(tab);
			tabView.selectedTabIndex = tabView.IndexOf(tab);
		}

		private void ShowConfig(Tab tab, Rect rect) {
			var window = CreateInstance<EditorWindow>();
			window.ShowAsDropDown(rect, new Vector2(240f, 80f));
			window.rootVisualElement.Add(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(QueryBox.uxml_path).Instantiate());

			var nameField = window.rootVisualElement.Q<TextField>(className: QueryBox.nameFieldUssClassName);
			nameField.value = tab.label;
			nameField.RegisterCallback<ChangeEvent<string>>(ev => {
				tab.label = ev.newValue;
			});

			var queryField = window.rootVisualElement.Q<TextField>(className: QueryBox.queryFieldUssClassName);
			queryField.value = "t:Monster";

			window.rootVisualElement.Q<Button>(className: QueryBox.refreshButtonUssClassName).RegisterCallback<ClickEvent>(ev => {
				var cs = tab.Q<ColumnSheet>();
				cs.data = AssetDatabase.FindAssets(queryField.value);
				cs.Rebuild();
			});

			window.rootVisualElement.Q<Button>(className: QueryBox.closeButtonUssClassName).RegisterCallback<ClickEvent>(ev => {
				window.Close();
				var index = tabView.IndexOf(tab);
				if (tabView.selectedTabIndex == index) {
					tabView.selectedTabIndex = Mathf.Max(0, index - 1);
				}
				tab.RemoveFromHierarchy();
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
