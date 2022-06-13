using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongEnemy : Enemy {

  public Animator enemyAnimator;
  private Vector3 originalEnemyAnimatorPosition;
    // Start is called before the first frame update
    void Start() {
      originalEnemyAnimatorPosition = enemyAnimator.transform.localPosition;
      enemyAnimator.SetFloat("Forward", 0.3f);
    }

    // Update is called once per frame
    void LateUpdate() {
      enemyAnimator.transform.localPosition = originalEnemyAnimatorPosition;
    }
}
