using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShittyWizard.Model.World;
using ShittyWizzard.Utilities;
using System.Text.RegularExpressions;

namespace ShittyWizard.Model.World
{
	public class Map
	{

		public RoomManager RoomManager { get; protected set; }
		public TileManager TileManager { get; protected set; }

		public int Width { get { return TileManager.Width; } }

		public int Height { get { return TileManager.Height; } }

		public Map (int numberOfRooms=20)
		{
			SetupManagers (numberOfRooms);
		}

		public void Update (float delta)
		{

		}

		private void SetupManagers (int numberOfRooms=20)
		{
			RoomManager = new RoomManager (this, numberOfRooms, 15, 15);
			TileManager = new TileManager (this);
		}
	}
}