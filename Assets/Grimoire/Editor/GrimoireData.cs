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
	}
}
