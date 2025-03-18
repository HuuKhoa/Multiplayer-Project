using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player2Behavior : MonoBehaviour {
    // Initialize variables for the character
    public GameObject arrowPrefab;
    
    private Vector3 destinationPos;
    private Vector3 arrowTarget;
    public float moveSpeed = 5f;
    public Vector3 resetPos;

    public float arrowTimer = 5f;
    public float arrowSpeed = 5f;
    private float shootCooldown;
    private bool canShoot = true;


    // Awake is called before the scene is loaded
    private void Awake() {

    }

    // Start is called before the first frame update
    void Start() {
        // soundManager.PlaySFX(soundManager.dungeonAmbiance); was too loud so I cut it from the game
        // Set initial destiniation position
        destinationPos = this.transform.position;
    }

    /*
    IEnumerator SpawnArrows(){
        while(true) {
            yield return new WaitForSeconds(arrowTimer);
            SpawnArrow();
        }
    }
    */

    // Update is called once per frame
    void Update() {
        // Get the position of the mouse when right click button is pressed
        
        if (Input.GetMouseButton(3)) {
            destinationPos = Input.mousePosition;
            destinationPos.z = -Camera.main.transform.position.z;
            destinationPos = Camera.main.ScreenToWorldPoint(destinationPos); 
        }

        if (Input.GetMouseButtonDown(4) && canShoot) {
            
            SpawnArrow();
            shootCooldown = 0f;
            canShoot = false;
        } else if (!canShoot) {
            shootCooldown += Time.deltaTime;
            if(shootCooldown >= 5f) {
                canShoot = true;
            } 
        }
        
    }

    void SpawnArrow () {
        // Convert mouse position to world space
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z; // Keep arrow in the same Z-plane

        // Calculate direction from player to mouse click
        Vector3 shootDirection = (mousePos - transform.position).normalized;

        // Instantiate arrow
        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);

        // Rotate arrow to face the direction
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Apply velocity in the direction
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.velocity = shootDirection * arrowSpeed;
        }
        // Destroy arrow after some time
        Destroy(arrow, arrowTimer);
    }

    void FixedUpdate() {
        // Move the Character towards where right click was pressed
        if (Vector3.Distance(this.transform.position, destinationPos) > 0.1f) {
            this.transform.position = Vector3.MoveTowards(this.transform.position, destinationPos, moveSpeed * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision coll) {

        // If the character touches anything that does damage calls the takes damage method
        if (coll.gameObject.CompareTag("DamageSource")) {
            TakesDamage();
        }

    }

    // Method to damage the character
    void TakesDamage() {

    }
    
}
