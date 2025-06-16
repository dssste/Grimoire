using UnityEditor;
using UnityEngine.UIElements;

namespace Grimoire.Inspector {
	[UxmlElement]
	public partial class QueryBox : VisualElement {
		private static string uxml_path = "Editor/UI/QueryBox.uxml";

		public QueryBox() {
			Add(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(GrimoireWindow.start_path + uxml_path).Instantiate());
		}

		public static string nameFieldUssClassName = "name-field";
		public static string queryFieldUssClassName = "query-field";
		public static string sheetTypeDropdownUssClassName = "sheet-type-dropdown";
		public static string refreshButtonUssClassName = "refresh-button";
		public static string closeButtonUssClassName = "close-button";
	}
}
