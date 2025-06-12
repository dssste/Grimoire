using UnityEditor;
using UnityEngine;

namespace Grimoire.Editor {
	public static class GrimoireSettingsProvider {
		[SettingsProvider]
		public static SettingsProvider CreateGrimoireSettingsProvider() {
			var provider = new SettingsProvider("Project/Grimoire", SettingsScope.Project) {
				label = "Grimoire",
				guiHandler = (searchContext) => {
					EditorGUILayout.LabelField("Grimoire Settings", EditorStyles.boldLabel);

					bool exampleToggle = EditorPrefs.GetBool("Grimoire_ExampleToggle", false);
					bool newToggle = EditorGUILayout.Toggle("Enable Magic", exampleToggle);
					if (newToggle != exampleToggle) {
						EditorPrefs.SetBool("Grimoire_ExampleToggle", newToggle);
					}

					// Add more settings fields as needed
				},

				keywords = new System.Collections.Generic.HashSet<string>(new[] { "Grimoire", "Table", "Inspector" })
			};

			return provider;
		}
	}
}
