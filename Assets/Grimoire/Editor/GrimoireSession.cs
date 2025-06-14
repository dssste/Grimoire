using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

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
			if (tabs.Remove(data)) {
				EditorUtility.SetDirty(this);
				AssetDatabase.SaveAssets();
			}
			return 0;
		}

		public int UpdateOrAdd(TabData data) {
			if (!tabs.Contains(data)) {
				tabs.Add(data);
			}
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
			return 0;
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
