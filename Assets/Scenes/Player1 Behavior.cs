using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class Player1Behavior : NetworkBehaviour {
    // Initialize variables for the character
    public GameObject arrowPrefab;
    
    private Vector3 destinationPos;
    private Vector3 arrowTarget;
    public float moveSpeed = 5f;
    public Vector3 resetPos;

    public float arrowTimer = 5f;
    public float arrowSpeed = 5f;
    public float shootCooldown = 2f;
    private bool canShoot = true;



    // Start is called before the first frame update
    void Start() {
        destinationPos = this.transform.position;
    }



    // Update is called once per frame
    void Update() {
        if(!IsOwner) return;
        // Get the position of the mouse when right click button is pressed
        // Move the character to that position
        if (Input.GetMouseButton(1)) {
            destinationPos = Input.mousePosition;
            destinationPos.z = -Camera.main.transform.position.z;
            destinationPos = Camera.main.ScreenToWorldPoint(destinationPos); 
        }

        // Shoot an arrow in the direction that the mouse is clicked
        if (Input.GetMouseButtonDown(0) && canShoot) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = transform.position.z;

            // Call ServerRpc to request server to spawn arrow
            SpawnArrowServerRpc(mousePos);

            shootCooldown = 0f;
            canShoot = false;
        } 
        else if (!canShoot) {
            shootCooldown += Time.deltaTime;
            if (shootCooldown >= 2f) {
                canShoot = true;
            }
        }
    }

    void FixedUpdate() {
        // Move the Character towards where right click was pressed
        if (Vector3.Distance(this.transform.position, destinationPos) > 0.1f) {
            this.transform.position = Vector3.MoveTowards(this.transform.position, destinationPos, moveSpeed * Time.deltaTime);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnArrowServerRpc(Vector3 targetPos, ServerRpcParams rpcParams = default) {
        if (!IsServer) return;

        // Calculate shoot direction
        Vector3 shootDirection = (targetPos - transform.position).normalized;

        // Offset spawn position slightly ahead of the player to prevent self-hit
        Vector3 spawnPosition = transform.position + (shootDirection * 0.8f);  // Move forward by 1.5 units

        // Instantiate arrow
        GameObject arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity);
        NetworkObject arrowNetworkObject = arrow.GetComponent<NetworkObject>();

        if (arrowNetworkObject != null) {
            arrowNetworkObject.Spawn();
        }

        // Rotate arrow to face the shooting direction
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Apply velocity
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.velocity = shootDirection * arrowSpeed;
        }

        // Destroy after set time
        Destroy(arrow, arrowTimer);
    }
    
    // Function for resetting the position of the player
    [ServerRpc(RequireOwnership = false)]
    public void ResetPositionServerRpc() {
        ResetPositionClientRpc();
    }

    [ClientRpc(RequireOwnership = false)]
    private void ResetPositionClientRpc() {
        if (IsOwner) {  // Only reset the affected player's position
            transform.position = resetPos;
            destinationPos = resetPos;
        }
    }
    
}
