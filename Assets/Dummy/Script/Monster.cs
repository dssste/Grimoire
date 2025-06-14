using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace Grimoire.Dummy {
	public enum MonsterType {
		Undead,
		Beast,
		Elemental,
		Humanoid,
		Dragon,
		Demon
	}

	[CreateAssetMenu(menuName = "Grimoire Dummy Data/Monster")]
	public class Monster : ScriptableObject {
		public LocalizedString displayName;
		public int exp;
		public MonsterType type;
		public Sprite icon;
		public GameObject prefab;

		public int health;
		public float speed;
		public Vector3 spawnPosition;
		public Color mainColor;

		public bool isBoss;
		public bool canFly;

		// public string[] abilities;
		// public int[] lootTableIds;
		// public AnimationClip[] attackAnimations;

		[System.Serializable]
		public class Drop {
			public Item item;
			public int minAmount;
			public int maxAmount;
			public float dropChance;
		}
		public List<Drop> drops;

		public Monster evolution;
	}
}
