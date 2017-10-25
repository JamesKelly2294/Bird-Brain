using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ShittyWizard.Controller.Game
{
	public class GUIController : MonoBehaviour
	{

		public Image currentHealthBar;

		public Image currentEXPbar;
		public float currentEXP = 0f;
		public float maxEXP = 100f;
		public int level = 1;
		public Text levelText;
		public RawImage minimap;
		public RectTransform minimapPlayerTracker;

		private GameObject playerGO;
		private EntityPlayer player;

		public WorldController worldController;

		// Use this for initialization
		void Start ()
		{
			currentEXP = 0f;
			playerGO = GameObject.FindGameObjectWithTag ("Player");
			player = playerGO.GetComponent<EntityPlayer> ();
			UpdateHealthBar ();

			minimap.texture = worldController.ActiveLevel.GenerateMinimap ();
		}

		// Update is called once per frame
		void Update ()
		{

			UpdateHealthBar ();

			float EXPratio = currentEXP / maxEXP;
			currentEXPbar.rectTransform.sizeDelta = new Vector2 (EXPratio * 250f, 20f);
			levelText.text = "Level " + level;

			if (currentEXP >= maxEXP) {
				level = level + 1;
				float tempXP = currentEXP;
				currentEXP = tempXP - maxEXP;
			}

			minimapPlayerTracker.localPosition = new Vector3 ((int)(playerGO.transform.position.x), 
				(int)playerGO.transform.position.z, 0.0f);
		}

		public void UpdateHealthBar ()
		{

			float ratio = player.health / player.maxHealth;
			if (player.maxHealth <= 0) {
				ratio = 1;
			}
			currentHealthBar.rectTransform.sizeDelta = new Vector2 (ratio * 250f, 20f);

		}

		public void GiveEXP (float EXP)
		{
			currentEXP += EXP;
		}
	}
}