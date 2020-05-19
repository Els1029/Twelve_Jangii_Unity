using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareCode : MonoBehaviour
{
    public string squareName;
    public GameManager gm;

    public GameObject currentPiece;

    public bool isOccupied()
    {
        if(currentPiece == null)
            return false;
        return true;
    }

    // void OnMouseDown()
    // {

    //     if(!isOccupied())
    //         Debug.Log("Clicked " + squareName);
    // }
}
