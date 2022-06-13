using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemy : Enemy {

    public GameObject model;
    public float timeToRotate = 2f;
    public float rotationSpeed = 6f;
    public bool rotateClockwise = true;
    public int startingAngle = 0;

    public GameObject bulletPrefab;
    public GameObject bulletSpawnPoint;
    public float bulletHeightOffset = 0.2f;
    public float timeToShoot = 1f;

    private float shootingTimer;
    private int targetAngle;
    private float rotationTimer;


    // Start is called before the first frame update
    void Start() {
        rotationTimer = timeToRotate;
        shootingTimer = timeToShoot;

        targetAngle = startingAngle;
        transform.localRotation = Quaternion.Euler(0, targetAngle, 0);
    }

    // Update is called once per frame
    void Update() {
        // Update the enemy angle
        rotationTimer -= Time.deltaTime;
        if(rotationTimer <= 0f) {
          rotationTimer = timeToRotate;
          targetAngle += rotateClockwise ? 90 : -90;
        }
        // perform enemy rotation
        transform.localRotation = Quaternion.Lerp(
          transform.localRotation,
           Quaternion.Euler(0, targetAngle, 0),
           Time.deltaTime * rotationSpeed
          );

          // shoot bullets
          shootingTimer -= Time.deltaTime;
          if(shootingTimer <= 0f) {
            shootingTimer = timeToShoot;

            GameObject bulletObject = Instantiate(bulletPrefab);
            bulletObject.transform.SetParent(transform.parent);
            bulletObject.transform.position = bulletSpawnPoint.transform.position;
            bulletObject.transform.forward = model.transform.forward;
          }
    }
}
