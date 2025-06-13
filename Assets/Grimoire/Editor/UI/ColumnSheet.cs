using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Grimoire.Inspector {
	public interface ISheet {
		public static string ussClass = "sheet";

		public enum Type {
			column_sheet,
		}

		string[] assetIds { set; }
		void Rebuild();
	}

	[UxmlElement]
	public partial class ColumnSheet : VisualElement, ISheet {
		public static string uxml_path = GrimoireWindow.start_path + "Editor/UI/ColumnSheet.uxml";

		public string[] assetIds { get; set; }

		public ColumnSheet() {
			AddToClassList(ISheet.ussClass);
		}

		public void Rebuild() {
			var resultContainer = this.Q<ScrollView>("result-container");
			resultContainer.Clear();
			foreach (var guid in assetIds) {
				var asset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid));
				if (asset != null) {
					var ve = new VisualElement();
					ve.AddToClassList("result-column");
					var so = new SerializedObject(asset);
					ve.Add(new Label(asset.name));
					var inspector = Editor.CreateEditor(so.targetObject).CreateInspectorGUI();
					inspector.Bind(so);
					ve.Add(inspector);
					resultContainer.Add(ve);
				}
			}
		}
	}
}
