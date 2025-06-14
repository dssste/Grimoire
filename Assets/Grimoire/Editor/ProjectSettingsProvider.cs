using UnityEditor;
using UnityEngine;

namespace Grimoire.Inspector {
	public static class ProjectSettings {
		private static string user_settings_key => "Grimoire_Data_Guid";

		private static GrimoireSession _grimoireSession;
		public static GrimoireSession grimoireSession {
			get {
				if (_grimoireSession == null) {
					var guid = EditorUserSettings.GetConfigValue(user_settings_key);
					if (string.IsNullOrEmpty(guid)) return null;

					_grimoireSession = AssetDatabase.LoadAssetAtPath<GrimoireSession>(AssetDatabase.GUIDToAssetPath(guid));
				}
				return _grimoireSession;
			}
			private set {
				_grimoireSession = value;
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
					var selectedData = grimoireSession;

					EditorGUILayout.BeginHorizontal();

					EditorGUI.BeginChangeCheck();
					selectedData = (GrimoireSession)EditorGUILayout.ObjectField("Grimoire Data", selectedData, typeof(GrimoireSession), false, GUILayout.ExpandWidth(true));
					if (EditorGUI.EndChangeCheck()) {
						grimoireSession = selectedData;
					}

					if (GUILayout.Button("+", GUILayout.Width(20f))) {
						var path = EditorUtility.SaveFilePanelInProject("Create GrimoireData", "GrimoireData", "asset", "Choose a location for the new GrimoireData asset");
						if (!string.IsNullOrEmpty(path)) {
							var asset = ScriptableObject.CreateInstance<GrimoireSession>();
							AssetDatabase.CreateAsset(asset, path);
							AssetDatabase.SaveAssets();
							grimoireSession = asset;
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
