using UnityEngine;
using UnityEngine.UIElements;

namespace Grimoire.Inspector {
	[UxmlElement]
	public partial class QueryBox : VisualElement {
		public static string uxml_path = GrimoireWindow.start_path + "Editor/UI/QueryBox.uxml";

		public TextField _queryField;
		public TextField queryField => _queryField ??= this.Q<TextField>("query-field");
		public Button _queryButton;
		public Button queryButton => _queryButton ??= this.Q<Button>("query-button");
	}
}
