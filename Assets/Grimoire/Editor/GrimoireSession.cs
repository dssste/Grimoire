using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if (!USE_DEV_PATH)
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif

namespace Grimoire.Inspector {
	[Serializable]
	public class TabData {
		public string name;
		public string query;
		public ISheet.Type sheetType;
	}

	public class GrimoireSession : ScriptableObject {
		public List<TabData> tabs;
		public int selectedTabIndex;

		public int Remove(TabData data) {
			var index = tabs.IndexOf(data);
			if (index >= 0) {
				if (selectedTabIndex == index && selectedTabIndex > 0) {
					selectedTabIndex -= 1;
				}
				tabs.Remove(data);
				EditorUtility.SetDirty(this);
				AssetDatabase.SaveAssets();
			}
			return selectedTabIndex;
		}

		public int UpdateOrAdd(TabData data) {
			if (!tabs.Contains(data)) {
				tabs.Add(data);
				selectedTabIndex = tabs.Count - 1;
			}
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
			return selectedTabIndex;
		}
	}

#if (!USE_DEV_PATH)
	[CustomEditor(typeof(GrimoireSession))]
	public class GrimoireDataInspector : Editor {
		public override VisualElement CreateInspectorGUI() {
			var root = new VisualElement();
			var scriptField = new PropertyField(new SerializedObject(target).FindProperty("m_Script"));
			scriptField.SetEnabled(false);
			root.Add(scriptField);
			root.Add(new HelpBox(
					"This is a Grimoire data asset.\n" +
					"It stores your open tabs and queries.\n" +
					"You can select the active data in Project Settings → Grimoire.",
					HelpBoxMessageType.Info
			));
			return root;
		}
	}
#endif
}
