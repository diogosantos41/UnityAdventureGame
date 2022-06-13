using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSceneController : MonoBehaviour {

    [Header("Game")]
    public Player player;
    public GameCamera gameCamera;

    [Header("UI")]
    public GameObject[] hearts;
    public Text bombsText;
    public Text arrowsText;
    public GameObject dungeonPanel;
    public Text dungeonInfoText;
    public Text orbText;

    private float resetTimer = 3f;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
      if(player != null) {

        // Check for player information
        for(int i = 0 ; i < hearts.Length ; i++) {
          hearts[i].SetActive(i < player.health);
        }
        bombsText.text = "Bombs: " + player.bombAmount;
        arrowsText.text = "Arrows: " + player.arrowAmount;
        orbText.text = "Orbs: " + player.orbAmount;

        // Check for dungeon information
        Dungeon currentDungeon = player.CurrentDungeon;
        dungeonPanel.SetActive(currentDungeon != null);
        if(currentDungeon != null) {
          float clearPercentage =
          (float)(currentDungeon.EnemyCount - currentDungeon.CurrentEnemyCount) / currentDungeon.EnemyCount;
          dungeonInfoText.text = "Progress: " + Mathf.FloorToInt(clearPercentage * 100) + "%";

          if(currentDungeon.JustCleared) {
            gameCamera.FocusOn(currentDungeon.DungeonTreasure.gameObject);
          }
        }
      } else {
        for(int i = 0 ; i < hearts.Length ; i++) {
          hearts[i].SetActive(false);
        }
        resetTimer -= Time.deltaTime;
        if(resetTimer <= 0) {
            SceneManager.LoadScene("MainMenu");
        }
      }
    }
}
