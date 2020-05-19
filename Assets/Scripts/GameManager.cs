using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum playerTurn
{
    GREEN,
    RED
}

public class GameManager : MonoBehaviour
{

    public playerTurn currentPlayer; 

    public GameObject manPiece;
    public GameObject ministerPiece;
    public GameObject generalPiece;
    public GameObject kingPiece;
    public GameObject lordPiece;

    PiecesClass greenKing;
    PiecesClass redKing;

    bool winFlag = false;
    playerTurn winSide;

    public UIHandler ui;

    public bool playable = false;
    
    Dictionary<Position, Vector3> posTransforms = new Dictionary<Position, Vector3>();

    Dictionary<Position, GameObject> currentPos = new Dictionary<Position, GameObject>();

    GameObject[] redTaken = new GameObject[6]; //y = 6    x = - 5 +(2*i)
    GameObject[] greenTaken = new GameObject[6]; //-6




    // Start is called before the first frame update
    void Start()
    {
        posTransforms.Add(Position.GL, new Vector3(-2,-3,0));
        posTransforms.Add(Position.GC, new Vector3(0,-3,0));
        posTransforms.Add(Position.GR, new Vector3(2,-3,0));

        posTransforms.Add(Position.BL, new Vector3(-2,-1,0));
        posTransforms.Add(Position.BC, new Vector3(0,-1,0));
        posTransforms.Add(Position.BR, new Vector3(2,-1,0));

        posTransforms.Add(Position.TL, new Vector3(-2,1,0));
        posTransforms.Add(Position.TC, new Vector3(0,1,0));
        posTransforms.Add(Position.TR, new Vector3(2,1,0));

        posTransforms.Add(Position.RL, new Vector3(-2,3,0));
        posTransforms.Add(Position.RC, new Vector3(0,3,0));
        posTransforms.Add(Position.RR, new Vector3(2,3,0));


        currentPlayer = playerTurn.GREEN;
    }

    public void startGame()
    {
        currentPlayer = playerTurn.GREEN;
        clearBoard();
        //clear array AND reset variables
        winFlag = false;
        playable = true;
        currentPos = new Dictionary<Position, GameObject>();
        //Load all the pieces in the correct places
        createPiece(manPiece,Position.BC, playerTurn.GREEN);
        createPiece(ministerPiece,Position.GL, playerTurn.GREEN);
        createPiece(generalPiece,Position.GR, playerTurn.GREEN);

        GameObject tempGK = createPieceMidGame(kingPiece,Position.GC, playerTurn.GREEN);
        greenKing = tempGK.GetComponent<PiecesClass>();

        createPiece(manPiece,Position.TC, playerTurn.RED);
        createPiece(ministerPiece,Position.RR, playerTurn.RED);
        createPiece(generalPiece,Position.RL, playerTurn.RED);

        GameObject tempRK = createPieceMidGame(kingPiece,Position.RC, playerTurn.RED);
        redKing = tempRK.GetComponent<PiecesClass>();

        currentPos.Add(Position.BL, null);
        currentPos.Add(Position.BR, null);
        currentPos.Add(Position.TL, null);
        currentPos.Add(Position.TR, null);

        foreach(KeyValuePair<Position, GameObject> entry in currentPos)
        {
            if(entry.Value !=null)
            {
                //Debug.Log(entry.Key.ToString() +" "+ entry.Value.name+" "+ entry.Value.GetComponent<PiecesClass>().side.ToString());
                if(entry.Value.name.Contains("GREEN"))
                {
                    updatePieceSide(entry.Value, entry.Key, playerTurn.GREEN);
                }
                else if(entry.Value.name.Contains("RED"))
                {
                    updatePieceSide(entry.Value, entry.Key, playerTurn.RED);
                }

            }
        }
    }

    void createPiece(GameObject piece, Position pos, playerTurn side)
    {
        GameObject gMan = (GameObject)Instantiate(piece, posTransforms[pos], getSideRotate(side));
        gMan.name = side.ToString()+piece.GetComponent<PiecesClass>().type;
        currentPos.Add(pos, gMan);

        PiecesClass gManScript = piece.GetComponent<PiecesClass>();
        gManScript.gm = this;

        Debug.Log(gManScript.side.ToString() + " " + gManScript.type + " at " + gManScript.positionOB.ToString() + " " + gMan.transform.position);
    }

    GameObject createPieceMidGame(GameObject piece, Position pos, playerTurn side)
    {
        GameObject gMan = (GameObject)Instantiate(piece, posTransforms[pos], getSideRotate(side));
        gMan.name = side.ToString()+piece.GetComponent<PiecesClass>().type;
        currentPos[pos] = gMan;

        PiecesClass gManScript = piece.GetComponent<PiecesClass>();
        gManScript.gm = this;

        return gMan;
    }

    void updatePieceSide(GameObject piece, Position pos, playerTurn side)
    {
        PiecesClass gManScript = piece.GetComponent<PiecesClass>();
        gManScript.side = side;
        Position previousPos = gManScript.positionOB;
        gManScript.positionOB = pos;
    }

    void updatePieceSide(PiecesClass piece, Position pos, playerTurn side)
    {
        piece.side = side;
        Position previousPos = piece.positionOB;
        piece.positionOB = pos;
        currentPos[pos] = piece.gameObject;
        currentPos[previousPos] = null;
    }


    Quaternion getSideRotate(playerTurn side)
    {
        if(side == playerTurn.RED)
        {
            return Quaternion.Euler(0,0,180);
        }
        
        return Quaternion.identity;
    }


    public void movePiece(Transform tran, PiecesClass piece)
    {
        bool flag = true;
        float currentPosX = tran.position.x;
        float currentPosY = tran.position.y; 

        foreach(KeyValuePair<Position, Vector3> entry in posTransforms)
        {
            if(currentPosX >= entry.Value.x-1 && currentPosX < entry.Value.x+1 && currentPosY >= entry.Value.y-1 && currentPosY < entry.Value.y+1)
            {
                if(validateMove(entry.Key, piece))
                {
                    if(piece.positionOB == Position.TAKEN)
                    {
                        if(piece.side == playerTurn.GREEN)
                        {
                            removeFromTaken(piece.gameObject, greenTaken);
                        }
                        else if(piece.side == playerTurn.RED)
                        {
                            removeFromTaken(piece.gameObject, redTaken);
                        }
                    }
                    tran.position = entry.Value;
                    flag = false;
                    updatePieceSide(piece, entry.Key, piece.side);
                    piece.updateLocation(entry.Key);

                    //CHANGE MAN TO LORD AT THE END LOGIC
                    if(piece.type == "Man" && endOfBoard(piece))
                    {
                        Position tempPos = piece.positionOB;
                        GameObject lord = turnManToLord(piece.gameObject);
                        PiecesClass lordInfo = lord.GetComponent<PiecesClass>();
                        lordInfo.positionOB = tempPos;
                        lordInfo.side = currentPlayer;
                    }
     
                    if(winFlag) win(winSide); //A king has been taken
                    else if(currentPlayer == playerTurn.GREEN && endOfBoard(redKing)) win(redKing.side); // THE RED KING IS AT THE END AT THE END OF GREENS TRUN
                    else if(endOfBoard(greenKing)) win(greenKing.side); // THE GREEN KING IS AT THE END AT THE END OF REDS TRUN
                    else changeTurn();

                }
                break;
            }
        }

        if(flag)
        {
            piece.resetPos();
        }
    }

    void win(playerTurn winner)
    {
        if(winner == playerTurn.GREEN) Debug.Log("GREEN WINS");
        else Debug.Log("RED WINS");

        endGame();
    }

    void changeTurn()
    {
        if(currentPlayer == playerTurn.GREEN)
            currentPlayer = playerTurn.RED;
        else if(currentPlayer == playerTurn.RED)
            currentPlayer = playerTurn.GREEN;
    }

    void removeFromTaken(GameObject piece, GameObject[] takenArray)
    {
        for(int i = 0 ; i < takenArray.Length; i++)
        {
            if(takenArray[i] == piece)
            {
                takenArray[i] = null;
            }
        }
    }

    bool validateMove(Position pos, PiecesClass piece)
    {
        if(vaildateMoveOfPiece(pos, piece) && validateMoveToAllowedSquare(pos, piece))
            return true;

        return false;
    }

    bool vaildateMoveOfPiece(Position pos, PiecesClass piece)
    {
        if(piece.side == currentPlayer)
        {
            if(piece.validTargets.Contains(pos))
                return true;
        }
        return false;
    }

    bool validateMoveToAllowedSquare(Position pos, PiecesClass piece)
    {
        if(currentPos[pos] != null)
        {
            if(currentPos[pos].GetComponent<PiecesClass>().side == piece.side || piece.positionOB == Position.TAKEN)
                return false;
            else 
            {
                //the piece in pos is taken
                takePieceCurrentlyIn(pos);
            }
        }
        Debug.Log("Returned true");
        return true;
    }

    void takePieceCurrentlyIn(Position pos)
    {
        GameObject takenPiece = currentPos[pos];
        PiecesClass takenScript = takenPiece.GetComponent<PiecesClass>();
        
        if(takenScript.type == "King")
        {
            winSide = currentPlayer; //current player so=hould be the oppsite to the taken piece
            winFlag = true;
        }
        else if(takenScript.type == "Lord")
        {
            takenPiece = turnLordToMan(takenPiece);
            takenScript = takenPiece.GetComponent<PiecesClass>();
        }

        if(takenScript.side == playerTurn.RED)
        {
            //TAKEN BY GREEN
            takenScript.side = playerTurn.GREEN;
            for(int i = 0; i<greenTaken.Length; i++)
            {
                if(greenTaken[i] == null)
                {
                    greenTaken[i] = takenPiece;
                    takenPiece.transform.position = new Vector3(-5+(2*i), -6);
                    takenPiece.transform.rotation = getSideRotate(playerTurn.GREEN);
                    currentPos[pos] = null;
                    takenScript.updateLocation(Position.TAKEN);
                    break;
                }
            }
        }
        else if(takenScript.side == playerTurn.GREEN)
        {
            //TAKEN BY RED
            takenScript.side = playerTurn.RED;
            for(int i = 0; i<redTaken.Length; i++)
            {
                if(redTaken[i] == null)
                {
                    redTaken[i] = takenPiece;
                    takenPiece.transform.position = new Vector3(-5+(2*i), 6);
                    takenPiece.transform.rotation = getSideRotate(playerTurn.RED);
                    currentPos[pos] = null;
                    takenScript.updateLocation(Position.TAKEN);
                    break;
                }
            }
        }
    }

    GameObject turnManToLord(GameObject man)
    {
        PiecesClass manInfo = man.GetComponent<PiecesClass>();
        if(manInfo.type != "Man")
            return man;
        
        currentPos[manInfo.positionOB] = null;
        Debug.Log(currentPos[manInfo.positionOB]);
        GameObject lord = createPieceMidGame(lordPiece, manInfo.positionOB, manInfo.side);
        Debug.Log(currentPos[manInfo.positionOB]);
        Destroy(man);
        return lord;

    }

    GameObject turnLordToMan(GameObject lord)
    {
        PiecesClass lordInfo = lord.GetComponent<PiecesClass>();
        if(lordInfo.type != "Lord")
            return lord;
        
        currentPos[lordInfo.positionOB] = null;
        Debug.Log(currentPos[lordInfo.positionOB]);
        GameObject man = createPieceMidGame(manPiece, lordInfo.positionOB, lordInfo.side);
        Debug.Log(currentPos[lordInfo.positionOB]);
        Destroy(lord);
        return man;
    }

    bool endOfBoard(PiecesClass piece)
    {
        if(piece.side == playerTurn.GREEN)
        {
            if(piece.positionOB == Position.RR ||
                piece.positionOB == Position.RC ||
                piece.positionOB == Position.RL) return true;
            return false;
        }
        else if(piece.side == playerTurn.RED)
        {
            if(piece.positionOB == Position.GR ||
                piece.positionOB == Position.GC ||
                piece.positionOB == Position.GL) return true;
            return false;
        }

        return false;
    }

    void endGame()
    {
        playable = false;
        ui.updateWinnerText(winSide);

    }

    void clearBoard()
    {
        foreach(GameObject piece in GameObject.FindGameObjectsWithTag("Player"))
        {
            Destroy(piece);
        }
    }
}
