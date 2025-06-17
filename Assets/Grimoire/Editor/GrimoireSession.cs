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
		public string sheetKey;
	}

	public class GrimoireSession : ScriptableObject {
		public List<TabData> tabs;
		[SerializeField] private int _selectedTabIndex;
		public int selectedTabIndex {
			get => _selectedTabIndex;
			private set => _selectedTabIndex = value;
		}

		public void Remove(TabData data) {
			var index = tabs.IndexOf(data);
			if (index >= 0) {
				tabs.Remove(data);
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

		public void SetSelected(int index) {
			selectedTabIndex = index;
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
		}

		public void Reorder(int fromIndex, int toIndex, int selectedTabIndex) {
			var temp = tabs[toIndex];
			tabs[toIndex] = tabs[fromIndex];
			tabs[fromIndex] = temp;
			this.selectedTabIndex = selectedTabIndex;
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
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
