using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
}
