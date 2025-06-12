using UnityEngine;
using UnityEngine.Localization;

namespace Grimoire.Dummy {
	[CreateAssetMenu(menuName = "Grimoire Dummy Data/Monster")]
	public class Monster : ScriptableObject {
		public LocalizedString displayName;
		public int exp;
	}
}
