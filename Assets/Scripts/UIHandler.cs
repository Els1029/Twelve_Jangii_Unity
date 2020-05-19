using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public Text topText;
    public Text botText;

    public GameManager gm;

    // Update is called once per frame
    void Update()
    {
        if(!gm.playable && Input.GetMouseButtonDown(0)) startGame();
        
    }

    void startGame()
    {
        topText.text = "";
        botText.text = "";
        gm.startGame();
    }

    public void updateWinnerText(playerTurn winner)
    {
        if(gm.currentPlayer == playerTurn.GREEN)
            updateVicText("GREEN");
        else if(gm.currentPlayer == playerTurn.RED)
            updateVicText("RED");
    }

    void updateVicText(string winner)
    {
        topText.text = winner + " WON!!!";
        botText.text = "Click to play again";
    }
}
