using UnityEngine;
using UnityEngine.UIElements;

namespace Grimoire.Inspector {
	[UxmlElement]
	public partial class QueryBox : VisualElement {
		public static string uxml_path = GrimoireWindow.start_path + "Editor/UI/QueryBox.uxml";

		public static string queryFieldUssClassName = "query-field";
		public static string refreshButtonUssClassName = "refresh-button";
	}
}
