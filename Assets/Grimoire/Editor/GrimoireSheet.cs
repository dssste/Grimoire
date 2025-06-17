using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

namespace Grimoire.Inspector {
	public interface IGrimoireSheet {
		string[] assetIds { set; }
		void Rebuild();
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class RegisterGrimoireSheetAttribute : Attribute {
		public string key;
		private string _displayName;
		public string displayName => string.IsNullOrEmpty(_displayName) ? key : _displayName;
	}

	public static class SheetExtensions {
		public static class GrimoireSheetRegistry {
			public class Entry {
				public Type type;
				public RegisterGrimoireSheetAttribute attr;
				public string key;
			}

			public static readonly Dictionary<string, Entry> entries;

			static GrimoireSheetRegistry() {
				entries = new();
				foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
						.SelectMany(a => a.GetTypes())) {
					if (!typeof(VisualElement).IsAssignableFrom(type)) continue;

					var attrs = type.GetCustomAttributes<RegisterGrimoireSheetAttribute>(false);
					if (attrs.Count() <= 0) continue;
					if (!typeof(IGrimoireSheet).IsAssignableFrom(type)) {
						Debug.LogError($"[RegisterGrimoireSheet] '{type.FullName}' must interface IGrimoireSheet.");
						continue;
					}

					foreach (var attr in attrs) {
						if (string.IsNullOrEmpty(attr.key)) {
							attr.key = type.FullName;
						}
						var key = attr.key;
						if (entries.ContainsKey(key)) {
							Debug.LogWarning($"[RegisterGrimoireSheet] Duplicate sheet '{key}' found on type '{type.FullName}'.");
						}
						entries[key] = new() {
							type = type,
							attr = attr,
							key = key
						}
						;
					}
				}
			}
		}

		private static ConditionalWeakTable<IGrimoireSheet, string> _sheetInstances;

		public static IGrimoireSheet GetSheet(this Tab tab) {
			foreach (var ve in tab.Query<VisualElement>().Build()) {
				if (ve is IGrimoireSheet sheet && sheet != null) {
					return sheet;
				}
			}
			return null;
		}

		public static VisualElement GetVisaulElement(this string key) {
			if (GrimoireSheetRegistry.entries.TryGetValue(key, out var entry)) {
				var ve = Activator.CreateInstance(entry.type) as VisualElement;
				SetKey(ve as IGrimoireSheet, entry.key);
				return ve;
			} else {
				return new ColumnSheet();
			}
		}

		public static List<string> GetSheets() {
			return GrimoireSheetRegistry.entries.Keys.ToList();
		}

		private static void SetKey(IGrimoireSheet ve, string key) {
			if (_sheetInstances == null) {
				_sheetInstances = new();
			}
			_sheetInstances.AddOrUpdate(ve, key);
		}

		public static string GetKey(this IGrimoireSheet ve) {
			if (_sheetInstances == null) return null;

			_sheetInstances.TryGetValue(ve, out var key);
			return key;
		}
	}
}
