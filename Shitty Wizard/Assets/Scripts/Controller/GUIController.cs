using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ShittyWizard.Controller.Game
{
	public class GUIController : MonoBehaviour
	{
		[Header ("Player")]
		public Image currentHealthBar;
		public Image currentEXPbar;

		public float currentEXP = 0f;
		public float maxEXP = 100f;
		public int level = 1;
		public Text levelText;

		[Header ("Boss")]
		public GameObject currentBossBarGO;
		public Image currentBossBar;
		public Image currentBossBarBase;

		[Header ("Minimap")]
		public Text floorText;
		public RawImage minimap;
		public RectTransform minimapRectTransform;
		public RectTransform minimapPlayerTracker;
		public RectTransform minimapStaircaseTracker;

		[Header ("References")]
		public GameObject playerGO;
		public GameObject staircaseGO;
		public GameObject owlGO;
		public GameObject minimapGO;
		public EntityPlayer player;

		public WorldController worldController;

		private bool showMinimap = true;

		// Use this for initialization
		void Start ()
		{
			currentEXP = 0f;
			player = playerGO.GetComponent<EntityPlayer> ();
			UpdateHealthBar ();
		}

		public void UpdateForNewLevel (string floor)
		{
			minimap.texture = worldController.ActiveLevel.GenerateMinimap ();

			floorText.text = "Floor " + floor;
		}

		public void UpdateMinimap ()
		{
			if (!showMinimap) {
				return;
			}
			float x = (playerGO.transform.position.x / minimap.texture.width) * minimapRectTransform.rect.width;
			float z = (playerGO.transform.position.z / minimap.texture.height) * minimapRectTransform.rect.height;
			float offsetX = (float)minimapRectTransform.localScale.x / 2 + 1.0f;
			float offsetY = (float)minimapRectTransform.localScale.y / 2 - 1.0f;

			if (staircaseGO != null) {
				minimapPlayerTracker.localPosition = new Vector3 (x + offsetX, z + offsetY, 0.0f);

				x = (staircaseGO.transform.position.x / minimap.texture.width) * minimapRectTransform.rect.width;
				z = (staircaseGO.transform.position.z / minimap.texture.height) * minimapRectTransform.rect.height;
				minimapStaircaseTracker.localPosition = new Vector3 (x + offsetX, z + offsetY, 0.0f);
			}
		}

		// Update is called once per frame
		void Update ()
		{
			UpdateHealthBar ();

			float EXPratio = currentEXP / maxEXP;
			currentEXPbar.rectTransform.sizeDelta = new Vector2 (EXPratio * 250f, 20f);


			if (currentEXP >= maxEXP) {
				level = level + 1;
				levelText.text = level.ToString ();
				float tempXP = currentEXP;
				currentEXP = tempXP - maxEXP;
			}

			UpdateMinimap ();
		}

		public void UpdateHealthBar ()
		{
			float ratio = player.health / player.maxHealth;
			if (player.maxHealth <= 0) {
				ratio = 1;
			}
			currentHealthBar.rectTransform.sizeDelta = new Vector2 (ratio * 250.0f, 20f);

			if (owlGO != null) {
				Owlman owlman = owlGO.GetComponent<Owlman> ();
				float bossratio = owlman.health / owlman.maxHealth;
				if (owlman.maxHealth <= 0) {
					currentBossBarGO.SetActive (false);
					ratio = 1;
				}
				currentBossBar.rectTransform.localPosition = new Vector3 ((bossratio * 250.0f) - 250.0f, currentBossBar.rectTransform.localPosition.y, currentBossBar.rectTransform.localPosition.z);
				currentBossBar.rectTransform.sizeDelta = new Vector2 (bossratio * 500.0f, 20f);
			}
		}

		public void SetBoss (GameObject bossGO)
		{
			owlGO = bossGO;
			showMinimap = false;
			minimapGO.SetActive (false);
			ShowBossBar ();
		}

		public void ShowBossBar ()
		{
			currentBossBarGO.SetActive (true);
		}

		public void HideBossBar ()
		{
			currentBossBarGO.SetActive (false);
			showMinimap = true;
			minimapGO.SetActive (true);
			owlGO = null;
		}

		public void GiveEXP (float EXP)
		{
			currentEXP += EXP;
		}
	}
}