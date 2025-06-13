using System.Linq;
using Alchemy.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Grimoire.Inspector {
	public class GrimoireWindow : EditorWindow {
#if (USE_DEV_PATH)
		private static string start_path = "Assets/Grimoire/";
#else
		private static string start_path = "Packages/com.dss.grimoire/";
#endif

		private static string uxml_path = start_path + "Editor/UI/GrimoireWindow.uxml";

		[MenuItem("Window/Grimoire")]
		public static void ShowHome() {
			var window = GetWindow<GrimoireWindow>();
			var icon = EditorGUIUtility.IconContent("tree_icon_leaf").image;
			window.titleContent = new GUIContent("Grimoire", icon);
		}

		private void CreateGUI() {
			var vta = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxml_path);
			if (vta == null) return;

			var root = vta.Instantiate();
			rootVisualElement.Add(root);

			root.Q<TabView>().Add(new Tab(label: "style 3"));

			// var queryField = new TextField();
			// rootVisualElement.Add(queryField);

			// var v0 = new VisualElement();
			// v0.style.flexDirection = FlexDirection.Row;
			// rootVisualElement.Add(v0);
			//
			// var v1 = new VisualElement();
			// v0.Add(v1);
			//
			// var v2 = new VisualElement();
			// v2.style.flexDirection = FlexDirection.Row;
			// v0.Add(v2);

			// queryField.RegisterCallback<ChangeEvent<string>>(ev => {
			// 	var data = AssetDatabase.FindAssets(ev.newValue);
			// 	if (data.Length <= 0) return;
			//
			// 	loo = CreateInstance<ListOfObject>();
			// 	loo.data = data.Select(guid => AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid))).ToList();
			//
			// 	var multiColumnListView = new MultiColumnListView {
			// 		bindingPath = "data",
			// 		showBoundCollectionSize = false,
			// 		virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight
			// 	};
			// 	multiColumnListView.columns.Add(new Column { bindingPath = "name", title = "Name", stretchable = true });
			// 	multiColumnListView.columns.Add(new Column { bindingPath = "name", title = "Name", stretchable = true });
			// 	multiColumnListView.columns.Add(new Column { bindingPath = "name", title = "Name", stretchable = true });
			// 	multiColumnListView.columns.Add(new Column { bindingPath = "name", title = "Name", stretchable = true });
			// 	multiColumnListView.columns.Add(new Column { bindingPath = "name", title = "Name", stretchable = true });
			// 	multiColumnListView.columns.Add(new Column { bindingPath = "name", title = "Name", stretchable = true });
			// 	multiColumnListView.columns.Add(new Column {
			// 		bindingPath = "script",
			// 		title = "Moves",
			// 		stretchable = true,
			// 		makeCell = () => {
			// 			return new Label("asdf");
			// 			var movesMultiColumnListView = new MultiColumnListView {
			// 				showBoundCollectionSize = false,
			// 				virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
			// 			};
			// 			movesMultiColumnListView.columns.Add(new Column { bindingPath = "displayName", title = "Name", stretchable = true });
			// 			movesMultiColumnListView.columns.Add(new Column { bindingPath = "exp", title = "Power", stretchable = true });
			// 			return movesMultiColumnListView;
			// 		}
			// 	});
			//
			// 	var so = new SerializedObject(loo);
			// 	multiColumnListView.Bind(so);
			// 	rootVisualElement.Add(multiColumnListView);
			// });

			// 	var lv = rootVisualElement.Q<MultiColumnListView>();
			//
			// 	foreach (var guid in AssetDatabase.FindAssets(ev.newValue)) {
			// 		var asset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid));
			//
			// 		if (asset != null) {
			// 			var ve = new VisualElement();
			// 			var so = new SerializedObject(asset);
			// 			ve.Add(new Label(asset.name));
			// 			var inspector = AlchemyEditor.CreateEditor(so.targetObject).CreateInspectorGUI();
			// 			inspector.Bind(so);
			// 			ve.Add(inspector);
			// 			lv.Add(ve);
			// 		}
			// 	}
			// });

			// v1.Clear();
			// v2.Clear();
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

			// queryField.value = "t:Monster";

		}
	}
}
