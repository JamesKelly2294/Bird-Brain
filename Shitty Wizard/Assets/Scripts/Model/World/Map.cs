using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShittyWizard.Model.World;
using ShittyWizzard.Utilities;

public class Map {

	public TileManager TileManager { get; protected set; }

	public int Width { get; protected set; }

	public int Height { get; protected set; }

	public Map (int width = 20, int height = 20)
	{
		this.Width = width;
		this.Height = height;

		SetupManagers ();

		//Debug.Log ("World created with " + (width * height) + " tiles.");
	}

	public void Update(float delta) {

	}

	private void SetupManagers() {
		TileManager = new TileManager (this);
	}
}
