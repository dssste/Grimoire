using UnityEngine.UIElements;

namespace Grimoire.Inspector {
	public interface ISheet {
		public enum Type {
			inspector_columns,
			table,
		}

		string[] assetIds { set; }
		void Rebuild();
	}

	public static class SheetExtensions {
		public static ISheet GetSheet(this Tab tab) {
			foreach (var ve in tab.Query<VisualElement>().Build()) {
				if (ve is ISheet sheet && sheet != null) {
					return sheet;
				}
			}
			return null;
		}

		public static VisualElement GetVisaulElement(this ISheet.Type type) {
			return type switch {
				ISheet.Type.table => new TableSheet(),
				_ => new ColumnSheet(),
			};
		}

		public static ISheet.Type GetSheetType(this ISheet sheet) {
			return sheet switch {
				TableSheet => ISheet.Type.table,
				_ => ISheet.Type.inspector_columns,
			};
		}
	}
}
