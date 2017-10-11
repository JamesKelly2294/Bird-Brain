using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShittyWizard.Model.World
{
	public enum TileType
	{
		Empty,
		Floor,
		Wall
	};

	public interface Tile
	{
		uint Id { get; }

		TileType Type { get; }

		int X { get; }

		int Y { get; }

		float MovementCost { get; }

		bool IsWalkable { get; }
	};
}
