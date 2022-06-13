using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChasingEnemy : Enemy {

    public NavMeshAgent agent;
    public Animator enemyAnimator;
    public float attackRadius = 3f;
    public float attackUpdateDuration = 1f;
    public float movementRadius = 15f;
    public float movementUpdateDuration = 3f;


    private Vector3 initialPosition;
    private float attackUpdateTimer;
    private float movementUpdateTimer;
    private Vector3 originalEnemyAnimatorPosition;
    private Vector3 previousPosition;

    private bool isAttacking = false;

    void Awake () {
      previousPosition = transform.position;
      originalEnemyAnimatorPosition = enemyAnimator.transform.localPosition;
    }
    // Start is called before the first frame update
    void Start() {
      initialPosition = transform.position;
      movementUpdateTimer = movementUpdateDuration;
      attackUpdateTimer = attackUpdateDuration;
      MoveAroundStart();
    }

    // Update is called once per frame
    void Update() {

      // Walk around if didnt see the player
      if(isAttacking == false) {
      movementUpdateTimer -= Time.deltaTime;
      if(movementUpdateTimer <= 0) {
        movementUpdateTimer = movementUpdateDuration;
        MoveAroundStart();
      }
    }

      // Search for the player
      attackUpdateTimer -= Time.deltaTime;
      if (attackUpdateTimer <= 0) {
        attackUpdateTimer = attackUpdateDuration;
        // A bit bugged, need to improve the chase logic
        //SearchPlayer();
      }

      // Walk or Idle animations
      if(Vector3.Distance(transform.position, previousPosition) < 0.003f) {
        enemyAnimator.SetFloat("Forward", 0.0f);
      } else if(agent.velocity.magnitude > 0) {
        enemyAnimator.SetFloat("Forward", 0.5f);
        }


        previousPosition = transform.position;
      }

    void SearchPlayer() {
      isAttacking = false;
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, attackRadius, transform.forward);
          foreach(RaycastHit hit in hits) {
            if(hit.transform.GetComponent<Player>() != null) {
              agent.SetDestination(hit.transform.position);
              isAttacking = true;
              break;
            }
          }
        }

    void MoveAroundStart() {
      agent.SetDestination(initialPosition + new Vector3(
      Random.Range(-movementRadius, movementRadius),
      0,
      Random.Range(-movementRadius, movementRadius)
      ));
    }

    // Update is called once per frame
    void LateUpdate() {
      enemyAnimator.transform.localPosition = originalEnemyAnimatorPosition;
    }
}
