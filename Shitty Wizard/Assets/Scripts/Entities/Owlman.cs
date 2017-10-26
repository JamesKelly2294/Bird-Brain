using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Owlman : EntityEnemy {

	enum BossState{
		Circling,
		MoveToPoint,
		Attacking,
		Dying
	};

	private BossState bossState = BossState.Circling;

	public Spell testSpell;


	private Vector3 circlePoint = Vector3.zero;
	private float radius = 6.0f;
	private float radiansTravelled = 0.0f;
	private float circlingSpeed = 1.0f;
	private float attackTimer = 0.0f;
	private float attackRate = 2.0f;

	private float pointSpeed = 1.0f;

	public Animator attackAnimator;
	public GameObject idleObject;
	public GameObject attackObject;

	public GameObject target;

	public Vector3 offset = Vector3.zero;

	protected override void OnUpdate ()
	{
		// States
		if (bossState == BossState.Circling) {

			// Circle around a point
			radiansTravelled += circlingSpeed * Time.deltaTime;
			transform.position = offset + new Vector3 (radius * Mathf.Cos (radiansTravelled), transform.position.y, radius * Mathf.Sin (radiansTravelled));

			// Increment attack timer
			attackTimer += Time.deltaTime;

			if (attackTimer >= attackRate)
			{
				StartCoroutine (Attack());
				attackTimer = 0.0f;
			}

		} else if (bossState == BossState.MoveToPoint) {

		} else if (bossState == BossState.Attacking) {



		} else if(bossState == BossState.Dying){
		
		}
	}

	private IEnumerator Attack(){
		idleObject.SetActive (false);
		attackObject.SetActive (true);

		// Start animation

		// Wait one second then shoot then wait again before moving
		yield return new WaitForSeconds (1.0f);
		testSpell.RequestCast ((target.transform.position - this.transform.position).normalized); // shoot towards (0,0,0)
		yield return new WaitForSeconds (1.0f);

		idleObject.SetActive (true);
		attackObject.SetActive (false);
		bossState = BossState.Circling;
	}
}
