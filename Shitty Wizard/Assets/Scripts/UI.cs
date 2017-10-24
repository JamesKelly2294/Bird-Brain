using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour {

	public Image currentHealthBar;

	public Image currentEXPbar;
	public float currentEXP = 0f;
	public float maxEXP = 100f;
	public int level = 1;
	public Text levelText;

	private EntityPlayer player;

	// Use this for initialization
	void Start () {
		currentEXP = 0f;
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<EntityPlayer>();
        UpdateHealthBar();
	}
	
	// Update is called once per frame
	void Update () {

        UpdateHealthBar();

		float EXPratio = currentEXP / maxEXP;
		currentEXPbar.rectTransform.sizeDelta = new Vector2 (EXPratio * 250f, 20f);
		levelText.text = level.ToString();

		if (currentEXP >= maxEXP) {
			level = level + 1;
			float tempXP = currentEXP;
			currentEXP = tempXP - maxEXP;
		}
	}

    public void UpdateHealthBar() {

        float ratio = player.health / player.maxHealth;
        if (player.maxHealth <= 0) {
            ratio = 1;
        }
        currentHealthBar.rectTransform.sizeDelta = new Vector2(ratio * 250f, 20f);

    }

	public void GiveEXP(float EXP) {
		currentEXP += EXP;
	}
}