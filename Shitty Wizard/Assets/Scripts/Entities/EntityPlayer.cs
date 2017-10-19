using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityPlayer : Entity {

    public int exp;
    public int expToNextLevel;
	public int currentHealth = 6;

	private UI _ui;

    protected override void OnStart() {
        base.OnStart();
		_ui = GameObject.Find ("HUD").GetComponent<UI> ();
    }

    protected override void OnUpdate() {
        base.OnUpdate();
    }

    protected override void OnDeath() {
        base.OnDeath();
    }

    protected override void OnDamage() {
        base.OnDamage();
		_ui.currentHealth--;
        MakeInvulnerable(2);
    }


}
