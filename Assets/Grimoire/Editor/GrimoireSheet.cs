using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

namespace Grimoire.Inspector {
	public interface IGrimoireSheet {
		IEnumerable<UnityEngine.Object> assets { set; }
		void Rebuild();
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class RegisterGrimoireSheetAttribute : Attribute {
		public string key;
		public string initHook;
	}

	public static class SheetExtensions {
		public static class GrimoireSheetRegistry {
			public class Entry {
				public Type type;
				public RegisterGrimoireSheetAttribute attr;
			}

			public static readonly List<Entry> entries;

			static GrimoireSheetRegistry() {
				entries = new();
				foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())) {
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
						if (entries.Exists(e => e.attr.key == key)) {
							Debug.LogWarning($"[RegisterGrimoireSheet] Duplicate sheet '{key}' on type '{type.FullName}'.");
						}
						entries.Add(new() {
							type = type,
							attr = attr,
						});
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

		public static VisualElement CreateInstance(this string key) {
			var entry = GrimoireSheetRegistry.entries.FirstOrDefault(e => e.attr.key == key);
			if (entry == null) {
				return new ColumnSheet();
			} else {
				var ve = Activator.CreateInstance(entry.type) as VisualElement;
				SetKey(ve as IGrimoireSheet, entry.attr.key);
				if (!string.IsNullOrEmpty(entry.attr.initHook)) {
					var method = entry.type.GetMethod(entry.attr.initHook, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (method != null && method.GetParameters().Length == 0) {
						method.Invoke(ve, null);
					} else {
						Debug.LogError($"Parameterless '{entry.attr.initHook}' not found on type '{entry.type.FullName}'.");
					}
				}
				return ve;
			}
		}

		public static List<string> GetSheets() {
			return GrimoireSheetRegistry.entries.Select(e => e.attr.key).ToList();
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
