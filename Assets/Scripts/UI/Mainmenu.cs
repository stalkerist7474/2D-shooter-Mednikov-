using Mirror;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Match : NetworkBehaviour
{

    public string ID;
    public readonly List<GameObject> players = new List<GameObject>();

    public Match(string ID, GameObject player)
    {
        this.ID = ID;
        players.Add(player);
    }
}


public class Mainmenu : NetworkBehaviour
{
    private string _inputID;
    private NetworkManager networkManager;

    public static Mainmenu Instance;
    public readonly SyncList<Match> matches = new SyncList<Match>();
    public readonly SyncList<string> matchIDs = new SyncList<string>();
    [SerializeField] public TMP_InputField InputIDForCreate;
    public TMP_InputField InputIDForJoin;
    public Button HostButton;
    public Button JoinButton;
    public Canvas LobbyCanvas;


    public Transform UIPlayerParent;
    public GameObject UIPlayerPrefab;
    public TMP_Text IDText;
    public Button BeginGameButton;
    public GameObject TurnManager;
    public bool InGame;


    private void Start()
    {
        Instance = this;
        networkManager = FindObjectOfType<NetworkManager>();


    }

    private void Update()
    {

        if (!InGame)
        {
            Player[] players = FindObjectsOfType<Player>();

            for (int i = 0; i < players.Length; i++)
            {
                players[i].gameObject.transform.localScale = Vector3.zero;
            }
        }



    }


    public void Host()
    {
        if (Player.LocalPlayer != null)
        {

            InputIDForCreate.interactable = false;
            InputIDForJoin.interactable = false;
            HostButton.interactable = false;
            JoinButton.interactable = false;


            Player.LocalPlayer.HostGame();


        }


    }

    public void HostSuccess(bool success, string matchID)
    {
        if (success)
        {
            LobbyCanvas.enabled = true;

            SpawnPlayerUIPrefab(Player.LocalPlayer);
            IDText.text = matchID;
            BeginGameButton.interactable = true;

        }
        else
        {
            InputIDForCreate.interactable = true;
            InputIDForJoin.interactable = true;
            HostButton.interactable = true;
            JoinButton.interactable = true;
        }
    }


    public void Join()
    {
        InputIDForCreate.interactable = false;
        InputIDForJoin.interactable = false;
        HostButton.interactable = false;
        JoinButton.interactable = false;

        Player.LocalPlayer.JoinGame(InputIDForJoin.text.ToUpper());
    }

    public void JoinSuccess(bool success, string matchID)
    {
        if (success)
        {
            LobbyCanvas.enabled = true;

            SpawnPlayerUIPrefab(Player.LocalPlayer);
            IDText.text = matchID;
            BeginGameButton.interactable = true;

        }
        else
        {
            InputIDForCreate.interactable = true;
            InputIDForJoin.interactable = true;
            HostButton.interactable = true;
            JoinButton.interactable = true;
        }
    }

    public bool HostGame(string matchID, GameObject player)
    {
        if (!matchIDs.Contains(matchID))
        {
            matchIDs.Add(matchID);
            matches.Add(new Match(matchID, player));
            return true;
        }
        else
        {
            return false;
        }
    }


    public bool JoinGame(string matchID, GameObject player)
    {
        if (matchIDs.Contains(matchID))
        {
            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].ID == matchID)
                {
                    matches[i].players.Add(player);
                    break;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public static string GetID()
    {

        return Mainmenu.Instance.InputIDForCreate.text;
    }

    public void SpawnPlayerUIPrefab(Player player)
    {
        GameObject newUIPlayer = Instantiate(UIPlayerPrefab, UIPlayerParent);
        newUIPlayer.GetComponent<PlayerUI>().SetPlayer(player);
    }


    public void StartGame()
    {
        Player.LocalPlayer.BeginGame();
    }

    public void BeginGame(string matchID)
    {
        GameObject newTurnManager = Instantiate(TurnManager);
        NetworkServer.Spawn(newTurnManager);
        newTurnManager.GetComponent<NetworkMatch>().matchId = matchID.ToGuid();
        TurnManager turnManager = newTurnManager.GetComponent<TurnManager>();


        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].ID == matchID)
            {
                foreach (var player in matches[i].players)
                {
                    Player player1 = player.GetComponent<Player>();
                    turnManager.AddPlayer(player1);
                    player1.StartGame();
                }
                break;
            }
        }
    }
    public void SetBeginButtonActive(bool active)
    {
        BeginGameButton.interactable = active;
    }

}

public static class MatchExtension
{
    public static Guid ToGuid(this string id)
    {
        MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
        byte[] inputBytes = Encoding.Default.GetBytes(id);
        byte[] hasBytes = provider.ComputeHash(inputBytes);

        return new Guid(hasBytes);
    }
}
