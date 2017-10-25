using System;
using System.Collections.Generic;

namespace ShittyWizard.Model.World
{
	public class World
	{
		private List<Map> m_levels;

		private int m_currentLevel = 0;
		private int m_maxLevels = 5;

		private int m_roomsPerLevel = 10;

		public World ()
		{
			m_levels = new List<Map> ();
			m_levels.Add (new Map (m_roomsPerLevel));
		}

		public void Update(float delta)
		{
			ActiveLevel.Update(delta);
		}

		public Map ActiveLevel {
			get {
				return m_levels [m_currentLevel];
			}
		}

		public Map AdvanceLevel() {
			m_currentLevel += 1;
			if (m_currentLevel > m_maxLevels) {
				return AdvanceToBossLevel();
			}

			Map newLevel = new Map (m_roomsPerLevel);
			m_levels.Add (newLevel);

			return newLevel;
		}

		public Map AdvanceToBossLevel() {
			return new Map (m_roomsPerLevel);
		}
	}
}

