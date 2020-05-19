using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum Position
{
    RL,RC,RR,
    TL,TC,TR,
    BL,BC,BR,
    GL,GC,GR,
    TAKEN
}

public class PiecesClass : MonoBehaviour
{
    public string type;
    public string[] directions;
    public playerTurn side;
    public Position positionOB;
    Camera cam;
    public GameManager gm;

    Vector3 previousPos;


    char[] upDownVal = {'G','B','T','R'};
    char[] leftRightVal = {'L','C','R'};
    public ArrayList validTargets = new ArrayList();

    //drag functions
    void OnMouseDown()
    {
        previousPos = transform.position;
        Debug.Log(side.ToString() + type);
        Debug.Log(transform.position);
    }

    void OnMouseDrag()
    {
        if(gm.currentPlayer == side)
        {
            Vector3 mousePos = Input.mousePosition;
            transform.position = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
        }
    }

    void OnMouseUp()
    {
        if(gm.currentPlayer == side)
        {
            // Debug.Log( "DROPPED "+side.ToString() + type);
            gm.movePiece(transform, this);
        }
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        validTargets = new ArrayList();

        gm = cam.GetComponent<GameManager>();

        updateValidTargets();
    }

    public void updateLocation(Position pos)
    {
        positionOB = pos;
        updateValidTargets();

    }

    void updateValidTargets()
    {
        validTargets.Clear();

        int sideMultiplier = 0;

        if(side == playerTurn.GREEN)
            sideMultiplier =1;
        else if(side == playerTurn.RED)
            sideMultiplier =-1;

        // if(positionOB == Position.TAKEN)
        //     return;


        string posString = positionOB.ToString();

        if(positionOB == Position.TAKEN)
        {
            validTargets.Add(Position.BC);
            validTargets.Add(Position.BL);
            validTargets.Add(Position.BR);
            validTargets.Add(Position.TC);
            validTargets.Add(Position.TL);
            validTargets.Add(Position.TR);

            if(side == playerTurn.RED)
            {
                validTargets.Add(Position.RC);
                validTargets.Add(Position.RL);
                validTargets.Add(Position.RR);
            }
            else if(side == playerTurn.GREEN)
            {
                validTargets.Add(Position.GC);
                validTargets.Add(Position.GL);
                validTargets.Add(Position.GR);
            }
        }
        else
        {    
            foreach (string direction in directions)
            {
                switch(direction)
                {
                    case "F":
                        try{
                            char upDownPos = upDownVal[Array.IndexOf(upDownVal, posString[0]) + sideMultiplier];
                            string temp = upDownPos.ToString() + posString[1];
                            Position vaildSpot = (Position)Enum.Parse(typeof(Position), temp);
                            validTargets.Add(vaildSpot);
                            // Debug.Log("Added " + temp + " as a vaild spot for " + name);
                        }
                        catch(IndexOutOfRangeException e){Debug.Log("Failed");}
                        break;
                    case "B":
                        try{
                            char upDownPos = upDownVal[Array.IndexOf(upDownVal, posString[0]) - sideMultiplier];
                            string temp = upDownPos.ToString() + posString[1];
                            Position vaildSpot = (Position)Enum.Parse(typeof(Position), temp);
                            validTargets.Add(vaildSpot);
                            // Debug.Log("Added " + temp + " as a vaild spot for " + name);
                        }
                        catch(IndexOutOfRangeException e){Debug.Log("Failed");}
                        break;

                    case "R":
                        try{
                            char leftRightPos = leftRightVal[Array.IndexOf(leftRightVal, posString[1]) + 1];
                            string temp =  posString[0] + leftRightPos.ToString();
                            Position vaildSpot = (Position)Enum.Parse(typeof(Position), temp);
                            validTargets.Add(vaildSpot);
                            // Debug.Log("Added " + temp + " as a vaild spot for " + name);
                        }
                        catch(IndexOutOfRangeException e){Debug.Log("Failed");}
                        break;
                    case "L":
                        try{
                            char leftRightPos = leftRightVal[Array.IndexOf(leftRightVal, posString[1]) - 1];
                            string temp = posString[0] + leftRightPos.ToString();
                            Position vaildSpot = (Position)Enum.Parse(typeof(Position), temp);
                            validTargets.Add(vaildSpot);
                            // Debug.Log("Added " + temp + " as a vaild spot for " + name);
                        }
                        catch(IndexOutOfRangeException e){Debug.Log("Failed");}
                        break;

                    
                    case "FR":
                        try{
                            char upDownPos = upDownVal[Array.IndexOf(upDownVal, posString[0]) + sideMultiplier];
                            char leftRightPos = leftRightVal[Array.IndexOf(leftRightVal, posString[1]) + 1];
                            string temp = upDownPos.ToString() + leftRightPos.ToString();
                            Position vaildSpot = (Position)Enum.Parse(typeof(Position), temp);
                            validTargets.Add(vaildSpot);
                            // Debug.Log("Added " + temp + " as a vaild spot for " + name);
                        }
                        catch(IndexOutOfRangeException e){Debug.Log("Failed");}
                        break;
                    case "FL":
                        try{
                            char upDownPos = upDownVal[Array.IndexOf(upDownVal, posString[0]) + sideMultiplier];
                            char leftRightPos = leftRightVal[Array.IndexOf(leftRightVal, posString[1]) - 1];
                            string temp = upDownPos.ToString() + leftRightPos.ToString();
                            Position vaildSpot = (Position)Enum.Parse(typeof(Position), temp);
                            validTargets.Add(vaildSpot);
                            // Debug.Log("Added " + temp + " as a vaild spot for " + name);
                        }
                        catch(IndexOutOfRangeException e){Debug.Log("Failed");}
                        break;
                    case "BR":
                        try{
                            char upDownPos = upDownVal[Array.IndexOf(upDownVal, posString[0]) - sideMultiplier];
                            char leftRightPos = leftRightVal[Array.IndexOf(leftRightVal, posString[1]) + 1];
                            string temp = upDownPos.ToString() + leftRightPos.ToString();
                            Position vaildSpot = (Position)Enum.Parse(typeof(Position), temp);
                            validTargets.Add(vaildSpot);
                            // Debug.Log("Added " + temp + " as a vaild spot for " + name);
                        }
                        catch(IndexOutOfRangeException e){Debug.Log("Failed");}
                        break;
                    case "BL":
                        try{
                            char upDownPos = upDownVal[Array.IndexOf(upDownVal, posString[0]) - sideMultiplier];
                            char leftRightPos = leftRightVal[Array.IndexOf(leftRightVal, posString[1]) - 1];
                            string temp = upDownPos.ToString() + leftRightPos.ToString();
                            Position vaildSpot = (Position)Enum.Parse(typeof(Position), temp);
                            validTargets.Add(vaildSpot);
                            // Debug.Log("Added " + temp + " as a vaild spot for " + name);
                        }
                        catch(IndexOutOfRangeException e){Debug.Log("Failed");}
                        break;
                }
            }
        }
        

        // Debug.Log(name + " has " + validTargets.Count + " vaild moves");
    }

    public void resetPos()
    {
        Debug.Log("Reset Pos");
        transform.position = previousPos;
    }


}
