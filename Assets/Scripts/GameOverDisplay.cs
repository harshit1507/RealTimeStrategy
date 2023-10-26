using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winnerNameText;
    [SerializeField] private GameObject gameOverDisplayParent;
    void Start()
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void ClientHandleGameOver(string winner)
    {
        winnerNameText.text = $"{winner} has Won!";
        gameOverDisplayParent.SetActive(true);
    }

    public void LeaveGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            //stop hosting
            NetworkManager.singleton.StopHost();
        }
        else
        {
            //stop client
            NetworkManager.singleton.StopClient();
        }
    }
}
