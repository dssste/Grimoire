using System.Collections.Generic;
using UnityEngine;

namespace Grimoire.Dummy {
	[CreateAssetMenu(menuName = "Grimoire Dummy Data/Weapon")]
	public class Weapon : Item {
		public int attackValue;
		public float baseAttackInterval;
		public List<string> affixes;
	}
}
