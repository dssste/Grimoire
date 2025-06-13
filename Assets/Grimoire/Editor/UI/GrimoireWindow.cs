using System;
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

			AddTab(new() {
				name = "monsters",
				query = "t:Object glob:Editor/**",
				sheetType = ISheet.Type.column_sheet
			});

			var tabHeaderContainer = tabView.Q<VisualElement>(className: TabView.headerContainerClassName);
			var newTabButton = new Button();
			newTabButton.text = "+";
			newTabButton.AddToClassList("new-tab-button");
			newTabButton.RegisterCallback<ClickEvent>(ev => {
				var (tab, header) = AddTab(new() {
					name = "new tab",
					sheetType = ISheet.Type.column_sheet
				});
				tab.RegisterCallbackOnce<GeometryChangedEvent>(ev => {
					ShowConfig(tab, EditorGUIUtility.GUIToScreenRect(header.worldBound));
				});
				newTabButton.RemoveFromHierarchy();
				tabHeaderContainer.Add(newTabButton);
			});
			tabHeaderContainer.Add(newTabButton);
		}

		private (Tab tab, VisualElement header) AddTab(TabData tabData) {
			var tab = new Tab(label: tabData.name);
			tab.userData = tabData;

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
			return (tab, header);
		}

		private void ShowConfig(Tab tab, Rect rect) {
			var tabData = tab.userData as TabData;
			var window = CreateInstance<EditorWindow>();
			window.ShowAsDropDown(rect, new Vector2(240f, 98f));
			window.rootVisualElement.Add(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(QueryBox.uxml_path).Instantiate());

			var typeDropdown = window.rootVisualElement.Q<DropdownField>(className: QueryBox.sheetTypeDropdownUssClassName);
			typeDropdown.choices = ((ISheet.Type[])Enum.GetValues(typeof(ISheet.Type)))
				.OrderBy(e => e)
				.Select(e => e.ToString())
				.ToList();
			typeDropdown.value = tabData.sheetType.ToString();

			var nameField = window.rootVisualElement.Q<TextField>(className: QueryBox.nameFieldUssClassName);
			nameField.value = tabData.name;
			nameField.RegisterCallback<ChangeEvent<string>>(ev => {
				tabData.name = ev.newValue;
				tab.label = tabData.name;
			});

			var queryField = window.rootVisualElement.Q<TextField>(className: QueryBox.queryFieldUssClassName);
			queryField.value = tabData.query;

			window.rootVisualElement.Q<Button>(className: QueryBox.refreshButtonUssClassName).RegisterCallback<ClickEvent>(ev => {
				tabData.query = queryField.value;
				tabData.sheetType = Enum.Parse<ISheet.Type>(typeDropdown.value);

				if (string.IsNullOrWhiteSpace(tabData.query)) {
					tab.Clear();
				} else {
					var sheet = tab.Q<VisualElement>(className: ISheet.ussClass) as ISheet;
					if (sheet == null || sheet.sheetType != tabData.sheetType) {
						tab.Clear();
						tab.Add(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(tabData.sheetType switch {
							_ => ColumnSheet.uxml_path,
						}).Instantiate());
						sheet = tab.Q<VisualElement>(className: ISheet.ussClass) as ISheet;
					}
					sheet.assetIds = AssetDatabase.FindAssets(tabData.query);
					sheet.Rebuild();
				}
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
