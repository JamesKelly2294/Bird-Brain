using System;
using UnityEngine;

namespace ShittyWizard.Model.World
{
	public interface Room
	{
		int MinX { get; }

		int MaxX { get; }

		int MinY { get; }

		int MaxY { get; }

		int Width { get; }

		int Height { get; }

		TileType[,] Tiles { get; }

		Vector2 Center { get; }

		Vector2 Position { get; }
	}
}

