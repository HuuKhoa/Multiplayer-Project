using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ArrowBehavior : NetworkBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (!IsServer) return;  // Only the server should process collisions

        // Destroy the arrow if it hits a wall
        if (other.CompareTag("Wall")) {
            GetComponent<NetworkObject>().Despawn(); 
        }

        // Reset the position of the player if it gets hit by an arrow
        if (other.CompareTag("Player")) {
            Player1Behavior player = other.GetComponent<Player1Behavior>();
            if (player != null) {
                player.ResetPositionServerRpc();  // Server tells client to reset
            }

            GetComponent<NetworkObject>().Despawn(); 
        }
    }
}