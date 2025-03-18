using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.UI;

public class NetworkUI : NetworkBehaviour
{

    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private TextMeshProUGUI playerCountText;

    private NetworkVariable<int> playerNum = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);

    private void Awake()
    {
        hostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
        } );

        clientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        } );
    }

    private void Update() {
        playerCountText.text = "Players: " + playerNum.Value.ToString();
        if(!IsServer) return;
        playerNum.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }

}
