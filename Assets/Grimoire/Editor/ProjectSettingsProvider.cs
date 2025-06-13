using UnityEditor;
using UnityEngine;

namespace Grimoire.Inspector {
	public static class ProjectSettings {
		private static string user_settings_key => "Grimoire_Data_Guid";

		private static GrimoireData _grimoireData;
		public static GrimoireData grimoireData {
			get {
				if (_grimoireData == null) {
					var guid = EditorUserSettings.GetConfigValue(user_settings_key);
					if (string.IsNullOrEmpty(guid)) return null;

					_grimoireData = AssetDatabase.LoadAssetAtPath<GrimoireData>(AssetDatabase.GUIDToAssetPath(guid));
				}
				return _grimoireData;
			}
			private set {
				_grimoireData = value;
				if (value == null) {
					EditorUserSettings.SetConfigValue(user_settings_key, "");
				} else {
					EditorUserSettings.SetConfigValue(user_settings_key, AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value)));
				}
			}
		}

		[SettingsProvider]
		public static SettingsProvider CreateGrimoireSettingsProvider() {
			return new SettingsProvider("Project/Grimoire", SettingsScope.Project) {
				guiHandler = (searchContext) => {
					var selectedData = grimoireData;

					EditorGUILayout.BeginHorizontal();

					EditorGUI.BeginChangeCheck();
					selectedData = (GrimoireData)EditorGUILayout.ObjectField("Grimoire Data", selectedData, typeof(GrimoireData), false, GUILayout.ExpandWidth(true));
					if (EditorGUI.EndChangeCheck()) {
						grimoireData = selectedData;
					}

					if (GUILayout.Button("+", GUILayout.Width(20f))) {
						var path = EditorUtility.SaveFilePanelInProject("Create GrimoireData", "GrimoireData", "asset", "Choose a location for the new GrimoireData asset");
						if (!string.IsNullOrEmpty(path)) {
							var asset = ScriptableObject.CreateInstance<GrimoireData>();
							AssetDatabase.CreateAsset(asset, path);
							AssetDatabase.SaveAssets();
							grimoireData = asset;
						}
					}

					EditorGUILayout.EndHorizontal();

					if (selectedData != null) {
						EditorGUILayout.Space();
						Editor.CreateEditor(selectedData).OnInspectorGUI();
					}
				}
			};
		}
	}
}
