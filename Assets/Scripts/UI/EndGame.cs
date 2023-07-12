using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EndGame : NetworkBehaviour
{
    public static EndGame Instance;
    private string _idMatch;
    private int _countPlayer;


    private void Start()
    {
        Player[] players = FindObjectsOfType<Player>();
    }

    private void Update()
    {
        
    }

    private void CheckMatch(Player[] players)
    {
        for (int i = 0; i < players.Length; i++)
        {
            _countPlayer++;
        }
    }


    public void SetMatch(string idMatch)
    {
        _idMatch = idMatch; 
    }




}
