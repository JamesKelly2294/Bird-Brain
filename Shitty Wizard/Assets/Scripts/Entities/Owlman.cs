using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ShittyWizard.Controller.Game;

public class Owlman : EntityEnemy {

	enum BossState{
		Circling,
		MoveToPoint,
		Attacking,
		Dying,
        Waiting
	};

	private BossState bossState = BossState.Waiting;

    [Header("Owl Settigns")]
    public GameObject spellHolder;
    private Dictionary<string, Spell> spells;
    
	private float radius = 6.0f;
	private float radiansTravelled = 0.0f;
	private float circlingSpeed = 1.0f;
	private float attackTimer = 0.0f;
	private float attackRate = 2.0f;

	private float pointSpeed = 1.0f;

	public Animator attackAnimator;
	public GameObject idleObject;
	public GameObject attackObject;
	public Transform eyeTransform;
	public GameObject target;

	public Vector3 offset = Vector3.zero;

    protected override void OnStart() {

        base.OnStart();

        spells = new Dictionary<string, Spell>();
        foreach (Spell spell in spellHolder.GetComponentsInChildren<Spell>()) {
            spells.Add(spell.name, spell);
        }

        this.transform.position = offset + new Vector3(0, 0, radius);

        bossState = BossState.Waiting;
        ProcessCurrentState();

    }

    private void ProcessCurrentState () {

		// States
		if (bossState == BossState.Circling) {

			// Circle around a point
			radiansTravelled += circlingSpeed * Time.deltaTime;
			transform.position = offset + new Vector3 (radius * Mathf.Cos (radiansTravelled), transform.position.y, radius * Mathf.Sin (radiansTravelled));

			// Increment attack timer
			attackTimer += Time.deltaTime;

			if (attackTimer >= attackRate) {
                StartCoroutine(Attack("Spiral", BossState.Circling));
				attackTimer = 0.0f;
			}

		} else if (bossState == BossState.MoveToPoint) {

            if (Vector3.Distance(this.transform.position, offset) < 0.2f) {
                int command = Random.Range(0, 3);
                if (command == 0) {
                    StartCoroutine(MoveTo(new Vector2(offset.x + radius, offset.z)));
                } else if (command == 1) {
                    StartCoroutine(MoveTo(new Vector2(offset.x - radius, offset.z)));
                } else if (command == 2) {
                    StartCoroutine(MoveTo(new Vector2(offset.x, offset.z + radius)));
                }
            } else {
                StartCoroutine(MoveTo(new Vector2(offset.x, offset.z)));
            }

        } else if (bossState == BossState.Attacking) {

            if (Vector3.Distance(this.transform.position, offset) < 0.2f) {
                int command = Random.Range(0, 2);
                if (command == 0) {
                    StartCoroutine(Attack("Spiral", BossState.MoveToPoint));
                } else if (command == 1) {
                    StartCoroutine(Attack("Blast", BossState.MoveToPoint));
                }
            } else {
                int command = Random.Range(0, 2);
                if (command == 0) {
                    StartCoroutine(Attack("Flurry", BossState.MoveToPoint));
                } else if (command == 1) {
                    StartCoroutine(Attack("FlurryCircle", BossState.MoveToPoint));
                }
            }

		} else if (bossState == BossState.Dying){
		
		} else if (bossState == BossState.Waiting) {

            StartCoroutine(Wait(3, BossState.Attacking));

        }

	}

	protected override void OnDamage() {
		base.OnDamage();
		MakeInvisibleFlash(0.5f);
	}

	protected override void OnDeath() {
		GameObject.Find ("WorldController").GetComponent<WorldController> ().LoadWinScene(3.0f);

		base.OnDeath ();
	}

    private IEnumerator Wait(float _length, BossState nextState) {
        yield return new WaitForSeconds(_length);
        bossState = nextState;
        ProcessCurrentState();
    }

	private IEnumerator Attack(string _type, BossState _nextState) {

		bossState = BossState.Attacking;
		idleObject.SetActive (false);
		attackObject.SetActive (true);

		// Wait one second then shoot then wait again before moving
		yield return new WaitForSeconds (2.0f);

        Vector3 targetDir = offset - this.transform.position; //target.transform.position - (this.transform.position + eyeTransform.position);
        targetDir.y = 0;
        targetDir.Normalize();

        if ("Blast" == _type) {

            spells["Blast"].RequestCast(targetDir);
            yield return new WaitForSeconds(1);
            spells["Blast"].RequestCast(Quaternion.Euler(0, 90, 0) * targetDir);
            yield return new WaitForSeconds(1);
            spells["Blast"].RequestCast(targetDir);
            yield return new WaitForSeconds(2);

        } else if ("Spiral" == _type) {

            spells["Spiral"].RequestCast(targetDir);
            yield return new WaitForSeconds(7);

        } else if ("Flurry" == _type) {

            spells["Flurry"].RequestCast(targetDir);
            yield return new WaitForSeconds(6);

        } else if ("FlurryCircle" == _type) {

            float circleRot = 0;
            if (Vector3.Distance(this.transform.position, offset + new Vector3(0, 0, radius)) < 0.2f) {
                circleRot = Mathf.PI / 2f;
            } else if (Vector3.Distance(this.transform.position, offset + new Vector3(radius, 0, 0)) < 0.2f) {
                circleRot = 0;
            } else if (Vector3.Distance(this.transform.position, offset + new Vector3(-radius, 0, 0)) < 0.2f) {
                circleRot = Mathf.PI;
            }

            int circleDir = Random.Range(0, 1);
            if (circleDir == 0) {
                circleDir = -1;
            }

            spells["FlurryCircle"].RequestCast(targetDir);

            float amountRotated = 0;
            while (amountRotated < Mathf.PI * 4) {

                amountRotated += Time.deltaTime;
                if (amountRotated > Mathf.PI * 4) amountRotated = Mathf.PI * 4;

                float currRot = amountRotated + circleRot;

                transform.position = offset + new Vector3(radius * Mathf.Cos(currRot), transform.position.y, radius * Mathf.Sin(currRot));
                yield return null;

            }

            yield return new WaitForSeconds(1);

        }

		idleObject.SetActive (true);
		attackObject.SetActive (false);

        bossState = _nextState;
        ProcessCurrentState();

    }

    private IEnumerator MoveTo(Vector2 _pos) {

        bossState = BossState.MoveToPoint;

        Vector3 target = new Vector3(_pos.x, this.transform.position.y, _pos.y);
        Vector3 velocity = Vector3.zero;
        float smoothTime = 0.3f;
        
        while (Vector3.Distance(this.transform.position, target) > 0.1f) {
            this.transform.position = Vector3.SmoothDamp(this.transform.position, target, ref velocity, smoothTime);
            yield return null;
        }

        bossState = BossState.Attacking;
        ProcessCurrentState();

    }

}
