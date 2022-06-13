using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

[Header("Visuals")]
public GameObject model;
public Animator playerAnimator;
public float rotatingSpeed = 5f;
public float characterModelHeightOffset = 0.1f;


[Header("Movement")]
public float movingVelocity;
public float sprintingVelocity;
public float jumpingVelocity;
public float knockbackForce;
public float playerRotatingSpeed = 1000f;

[Header("Equipment")]
public int health = 5;
public Sword sword;
public Bow bow;
public GameObject quiver;
public GameObject bombPrefab;
public int bombAmount = 5;
public int arrowAmount = 15;
public int orbAmount = 0;
public float throwingSpeed = 200f;


private float currentVelocity;
private bool canJump = true;
private Rigidbody playerRigidBody;
private Quaternion targetModelRotation;
private float knockbackTimer;
private bool justTeleported;
private Dungeon currentDungeon;


    public bool JustTeleported {
        get {
          bool returnValue = justTeleported;
          justTeleported = false;
          return returnValue;
        }
    }

    public Dungeon CurrentDungeon {
      get {
        return currentDungeon;
      }
    }

    // Start is called before the first frame update
    void Start() {
      bow.gameObject.SetActive(false);
      quiver.gameObject.SetActive(false);
      playerRigidBody = GetComponent<Rigidbody>();
      targetModelRotation = Quaternion.Euler(0, 0, 0);
      currentVelocity = movingVelocity;
    }

    // Update is called once per frame
    void Update() {

      model.transform.rotation = Quaternion.Lerp(
      model.transform.rotation,
      targetModelRotation,
      Time.deltaTime * rotatingSpeed);

      if(knockbackTimer > 0) {
        knockbackTimer -= Time.deltaTime;
      } else {
        ProcessInput();
      }

      playerAnimator.SetBool("OnGround", canJump);

      Debug.Log(currentDungeon);
    }

    void FixedUpdate () {
      // Keep the character animator gameobject in place
      playerAnimator.transform.position = transform.position + Vector3.up * characterModelHeightOffset;
    }

    void ProcessInput() {

      // Check Movement in the XZ
      playerRigidBody.velocity = new Vector3 (0, playerRigidBody.velocity.y, 0);

      bool isPlayerMoving = false;
      bool isPlayerSprinting = false;

      // Check for sprint
      if(Input.GetKey(KeyCode.LeftShift)) {
          currentVelocity = sprintingVelocity;
          isPlayerSprinting = true;
      } else {
        currentVelocity = movingVelocity;
      }

      if(Input.GetKey("right") || Input.GetKey("d")) {
        targetModelRotation = Quaternion.Euler(
        0,
        model.transform.localEulerAngles.y + playerRotatingSpeed * Time.deltaTime,
        0
        );
      }
      if(Input.GetKey("left") || Input.GetKey("a")) {
        targetModelRotation = Quaternion.Euler(
        0,
        model.transform.localEulerAngles.y - playerRotatingSpeed * Time.deltaTime,
        0
        );
      }
      if(Input.GetKey("up") || Input.GetKey("w")) {
        playerRigidBody.velocity = new Vector3(
        model.transform.forward.x * currentVelocity,
        playerRigidBody.velocity.y,
        model.transform.forward.z * currentVelocity
        );

        isPlayerMoving = true;

      }
      if(Input.GetKey("down") || Input.GetKey("s")) {
        playerRigidBody.velocity = new Vector3(
          -model.transform.forward.x * currentVelocity,
          playerRigidBody.velocity.y,
          -model.transform.forward.z * currentVelocity
        );

        isPlayerMoving = true;

      }


      if(isPlayerMoving) {
        playerAnimator.SetFloat("Forward", isPlayerSprinting ? 1f : 0.5f);
      } else {
        playerAnimator.SetFloat("Forward", 0.0f);
      }

      // Check for Jumps
      if(Input.GetKeyDown("space") && canJump) {
        canJump = false;
        playerRigidBody.velocity = new Vector3(
          playerRigidBody.velocity.x,
          jumpingVelocity,
          playerRigidBody.velocity.z
        );
      }


      // Check for Equipment interactions
      if(Input.GetKeyDown("u")) {
        sword.gameObject.SetActive(true);
        bow.gameObject.SetActive(false);
        quiver.gameObject.SetActive(false);
        sword.Attack();
      }
      if(Input.GetKeyDown("i")) {
        ThrowBomb();
      }
      if(Input.GetKeyDown("o")) {
        if(arrowAmount > 0) {
        sword.gameObject.SetActive(false);
        bow.gameObject.SetActive(true);
        quiver.gameObject.SetActive(true);
        bow.Attack();
        arrowAmount--;
         }
      }
    }

    private void ThrowBomb() {
      if(bombAmount <= 0) {
        return;
      }
      GameObject bombObject = Instantiate (bombPrefab);
      bombObject.transform.position = transform.position + model.transform.forward * 1.2f;

      Vector3 throwingDirection = (model.transform.forward + Vector3.up).normalized;

      bombObject.GetComponent<Rigidbody>().AddForce(throwingDirection * throwingSpeed);

      bombAmount--;
    }

    void OnTriggerEnter (Collider otherCollider) {
      if(otherCollider.GetComponent<EnemyBullet>() != null) {
        Hit((transform.position - otherCollider.transform.position).normalized);
        Destroy(otherCollider.gameObject);
      } else if(otherCollider.GetComponent<Treasure>() != null) {
          orbAmount++;
          Destroy(otherCollider.gameObject);
      }
    }

    void OnTriggerStay(Collider otherCollider) {
      if(otherCollider.GetComponent<Dungeon>() != null) {
        currentDungeon = otherCollider.GetComponent<Dungeon>();
      }
    }

    void OnTriggerExit(Collider otherCollider) {
      if(otherCollider.GetComponent<Dungeon>() != null) {
        Dungeon exitDungeon = otherCollider.GetComponent<Dungeon>();
        if(exitDungeon == currentDungeon) {
          currentDungeon = null;
        }
      }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.transform.tag == "FloorTrigger") {
            canJump = true;
        }

        if(collision.gameObject.GetComponent<Enemy>()) {
          Hit((transform.position - collision.transform.position).normalized);
        }
    }
    private void Hit(Vector3 direction) {
      Vector3 knockbackDirection = (direction + Vector3.up).normalized;
      playerRigidBody.AddForce(knockbackDirection * knockbackForce);
      knockbackTimer = 1f;

      health--;
      if(health <= 0) {
        Destroy(gameObject);
      }
    }

    public void Teleport (Vector3 target) {
      transform.position = target;
      justTeleported = true;
    }
}
