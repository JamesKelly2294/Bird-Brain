using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShittyWizard.Model.World
{
	public class World
	{
		private List<Map> m_levels;

		private int m_currentLevel = 0;
		private int m_maxLevels = 1;

		private int m_roomsPerLevel = 15;
		private float m_roomsPerFloorSpread = 0.2f;

		public World (int numberOfFloors, int roomsPerFloor, float roomsPerFloorSpread)
		{
			m_levels = new List<Map> ();
			m_maxLevels = numberOfFloors;
			m_roomsPerLevel = roomsPerFloor;
			m_roomsPerFloorSpread = roomsPerFloorSpread;
		}

		public void Update(float delta)
		{
			ActiveLevel.Update(delta);
		}

		public Map ActiveLevel {
			get {
				return m_levels [m_currentLevel - 1];
			}
		}

		public bool IsBossLevel {
			get {
				return m_currentLevel > m_maxLevels;
			}
		}

		public Map AdvanceLevel() {
			m_currentLevel += 1;
			Map newLevel;
			if (IsBossLevel) {
				newLevel = AdvanceToBossLevel();
			} else {
				int roomsForThisFloor = (int)(m_roomsPerLevel * (1.0f + UnityEngine.Random.Range (-m_roomsPerFloorSpread, m_roomsPerFloorSpread)));
				newLevel = new Map (roomsForThisFloor);
			}

			m_levels.Add (newLevel);

			return newLevel;
		}

		public Map AdvanceToBossLevel() {
			Map m = Map.NewBossMap ();

			return m;
		}

		public int CurrentFloorNumber {
			get {
				return m_currentLevel;
			}
		}

		public int MaximumFloors {
			get {
				return m_maxLevels;
			}
		}
	}
}

