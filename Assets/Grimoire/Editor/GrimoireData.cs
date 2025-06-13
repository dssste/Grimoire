using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Grimoire.Inspector {
	[Serializable]
	public class TabData {
		public string name;
		public string query;
		public ISheet.Type sheetType;
	}

	public class GrimoireData : ScriptableObject {
		public List<TabData> tabs;
		public int selectedTabIndex;

		public void Remove(TabData data) {
			if (tabs.Remove(data)) {
				EditorUtility.SetDirty(this);
				AssetDatabase.SaveAssets();
			}
		}

		public void UpdateOrAdd(TabData data) {
			if (!tabs.Contains(data)) {
				tabs.Add(data);
			}
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
		}
	}

	[CustomEditor(typeof(GrimoireData))]
	public class GrimoireDataInspector : Editor {
		public override VisualElement CreateInspectorGUI() {
			var root = new VisualElement();
			root.Add(new HelpBox(
					"This is a Grimoire data asset.\n" +
					"It stores your open tabs and queries.\n" +
					"You can select the active data in Project Settings → Grimoire.",
					HelpBoxMessageType.Info
			));
			return root;
		}
	}
}
