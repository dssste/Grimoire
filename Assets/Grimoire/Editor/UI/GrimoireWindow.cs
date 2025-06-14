using System;
using System.Linq;
using UnityEditor;
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

			var session = ProjectSettings.grimoireSession;
			if (session == null) {
				ShowNotification(new GUIContent("no persistant data selected, go to Project Settings/Grimoire to pick a persistant data asset"));
			} else {
				if (session.tabs.Count > 0) {
					foreach (var tabData in session.tabs) {
						AddTab(tabData);
					}
					tabView.selectedTabIndex = session.selectedTabIndex;
					RefreshTab(tabView.activeTab);
				}
				tabView.activeTabChanged += (fromTab, toTab) => {
					session.SetSelected(tabView.selectedTabIndex);
					var sheet = tabView.activeTab.GetSheet();
					if (sheet == null) {
						RefreshTab(tabView.activeTab);
					}
				};
				tabView.tabReordered += (fromIndex, toIndex) => {
					session.Reorder(fromIndex, toIndex, tabView.selectedTabIndex);
				};
			}

			var tabHeaderContainer = tabView.Q<VisualElement>(className: TabView.headerContainerClassName);
			var newTabButton = new Button();
			newTabButton.text = "+";
			newTabButton.AddToClassList("new-tab-button");
			newTabButton.RegisterCallback<ClickEvent>(ev => {
				var tabData = new TabData() {
					name = "new tab",
					sheetType = ISheet.Type.inspector_columns
				};
				var (tab, header) = AddTab(tabData);
				tab.RegisterCallbackOnce<GeometryChangedEvent>(ev => {
					ShowConfig(tab, EditorGUIUtility.GUIToScreenRect(header.worldBound));
				});
				tabView.selectedTabIndex = tabView.IndexOf(tab);
				var session = ProjectSettings.grimoireSession;
				if (session != null) {
					session.UpdateOrAdd(tabData);
					session.SetSelected(tabView.selectedTabIndex);
				}
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
			return (tab, header);
		}

		private void RefreshTab(Tab tab) {
			var tabData = tab.userData as TabData;
			if (string.IsNullOrWhiteSpace(tabData.query)) {
				tab.Clear();
			} else {
				var sheet = tab.GetSheet();
				if (sheet == null || sheet.GetSheetType() != tabData.sheetType) {
					tab.Clear();
					tab.Add(tabData.sheetType.GetVisualTreeAsset().Instantiate());
					sheet = tab.GetSheet();
				}
				sheet.assetIds = AssetDatabase.FindAssets(tabData.query);
				sheet.Rebuild();
			}
		}

		private void ShowConfig(Tab tab, Rect rect) {
			var tabData = tab.userData as TabData;
			var window = CreateInstance<EditorWindow>();
			window.ShowAsDropDown(rect, new Vector2(240f, 98f));
			window.rootVisualElement.Add(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(start_path + QueryBox.uxml_path).Instantiate());

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
				var session = ProjectSettings.grimoireSession;
				if (session == null) {
					ShowNotification(new GUIContent("no persistant data selected, go to Project Settings/Grimoire to pick a persistant data asset"));
				} else {
					session.UpdateOrAdd(tab.userData as TabData);
				}
				RefreshTab(tab);
			});

			window.rootVisualElement.Q<Button>(className: QueryBox.closeButtonUssClassName).RegisterCallback<ClickEvent>(ev => {
				window.Close();
				if (tabView.activeTab == tab && tabView.selectedTabIndex > 0) {
					tabView.selectedTabIndex -= 1;
				}
				tab.RemoveFromHierarchy();
				var session = ProjectSettings.grimoireSession;
				if (session == null) {
					ShowNotification(new GUIContent("no persistant data selected, go to Project Settings/Grimoire to pick a persistant data asset"));
				} else {
					session.Remove(tab.userData as TabData);
					session.SetSelected(tabView.selectedTabIndex);
				}
			});
		}
	}
}
