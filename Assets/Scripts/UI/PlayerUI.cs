using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUI : MonoBehaviour
{
    public TMP_Text PlayerText;
   
    private Player player;

    public void SetPlayer(Player player)
    {
        this.player = player;
        int num = Random.RandomRange(1, 30);
        PlayerText.text = "Name" + num;
    }
}
