using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {

	public int currentHealth;

	public Image healthUI;
	public Sprite[] healthSprites;

	//private EntityPlayer player;

	// Use this for initialization
	void Start () {
		currentHealth = 6;
		//player = GameObject.FindGameObjectWithTag ("Player").GetComponent<EntityPlayer> ();
	}
	
	// Update is called once per frame
	void Update () {
		healthUI.sprite = healthSprites [6 - currentHealth];
	}
}