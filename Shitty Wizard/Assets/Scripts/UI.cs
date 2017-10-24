using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour {

	public Image currentHealthBar;
	public float currentHealth = 100f;
	public float maxHealth = 100f;

	public Image currentEXPbar;
	public float currentEXP = 0f;
	public float maxEXP = 100f;
	public int level = 1;
	public Text levelText;

	//private EntityPlayer player;

	// Use this for initialization
	void Start () {
		currentHealth = maxHealth;
		currentEXP = 0f;
		//player = GameObject.FindGameObjectWithTag ("Player").GetComponent<EntityPlayer> ();
	}
	
	// Update is called once per frame
	void Update () {
		float ratio = currentHealth / maxHealth;
		currentHealthBar.rectTransform.sizeDelta = new Vector2 (ratio * 250f, 20f);

		if (currentHealth <= 0f) {
			SceneManager.LoadScene ("MenuScene");
		}

		float EXPratio = currentEXP / maxEXP;
		currentEXPbar.rectTransform.sizeDelta = new Vector2 (EXPratio * 250f, 20f);
		levelText.text = level.ToString();

		if (currentEXP >= maxEXP) {
			level = level + 1;
			float tempXP = currentEXP;
			currentEXP = tempXP - maxEXP;
		}
	}

	public void TakeDamage(float damage) {
		currentHealth -= damage;
		if (currentHealth < 0f) {
			currentHealth = 0f;
		}
		if (currentHealth > 100f) {
			currentHealth = 100f;
		}
	}

	public void GiveEXP(float EXP) {
		currentEXP += EXP;
	}
}