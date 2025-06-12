using UnityEngine;
using UnityEngine.Localization;

namespace Grimoire.Dummy {
	[CreateAssetMenu(menuName = "Grimoire Dummy Data/Item")]
	public class Item : ScriptableObject {
		public LocalizedString displayName;
	}
}
