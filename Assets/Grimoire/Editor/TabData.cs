using System;
using UnityEditor;
using UnityEngine;

namespace Grimoire.Inspector {
	[Serializable]
	public class TabData {
		public string name;
		public string query;
		public ISheet.Type sheetType;
	}
}
