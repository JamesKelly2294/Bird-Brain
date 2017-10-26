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

		public World ()
		{
			m_levels = new List<Map> ();
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
				newLevel = new Map (m_roomsPerLevel);
			}

			m_levels.Add (newLevel);

			return newLevel;
		}

		public Map AdvanceToBossLevel() {
			Map m = Map.NewBossMap ();

			return m;
		}

		public int CurrentLevelNumber {
			get {
				return m_currentLevel;
			}
		}
	}
}

