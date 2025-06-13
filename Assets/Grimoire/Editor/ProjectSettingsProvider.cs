using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Grimoire.Inspector {
	public static class GrimoireSettings {
		[SettingsProvider]
		public static SettingsProvider CreateGrimoireSettingsProvider() {
			return new SettingsProvider("Project/Grimoire", SettingsScope.Project) {
				guiHandler = (searchContext) => {
					var selectedData = LoadData();

					EditorGUILayout.BeginHorizontal();

					EditorGUI.BeginChangeCheck();
					selectedData = (GrimoireData)EditorGUILayout.ObjectField("Grimoire Data", selectedData, typeof(GrimoireData), false, GUILayout.ExpandWidth(true));
					if (EditorGUI.EndChangeCheck()) {
						SaveData(selectedData);
					}

					if (GUILayout.Button("+", GUILayout.Width(20f))) {
						var path = EditorUtility.SaveFilePanelInProject("Create GrimoireData", "GrimoireData", "asset", "Choose a location for the new GrimoireData asset");
						if (!string.IsNullOrEmpty(path)) {
							var asset = ScriptableObject.CreateInstance<GrimoireData>();
							AssetDatabase.CreateAsset(asset, path);
							AssetDatabase.SaveAssets();
							SaveData(asset);
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

		private static string user_settings_key => "Grimoire_Data_Guid";

		public static void SaveData(GrimoireData data) {
			if (data == null) {
				EditorUserSettings.SetConfigValue(user_settings_key, "");
			} else {
				EditorUserSettings.SetConfigValue(user_settings_key, AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(data)));
			}
		}

		public static GrimoireData LoadData() {
			var guid = EditorUserSettings.GetConfigValue(user_settings_key);
			if (string.IsNullOrEmpty(guid)) return null;

			return AssetDatabase.LoadAssetAtPath<GrimoireData>(AssetDatabase.GUIDToAssetPath(guid));
		}
	}
}
