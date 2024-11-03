using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{

    public GameManager gameManager;
    
    //mopvement of pacstudent now handled by dedicated controller
    public PacStudentController pacStudentController;
    
    public CherryController cherryController;
    public GhostController ghostController;
    
    //speed of pacStudent's LERP movement
    private int speed = 2;
    
    //initialise the MovementManager
    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        pacStudentController.Init(this);
        cherryController.Init(this);
        ghostController.Init(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        pacStudentController = GameObject.FindGameObjectWithTag("PacStudent").GetComponent<PacStudentController>();
    }


    // Update is called once per frame
    void Update()
    {
        //Update now does nothing, as movement is handled by the dedicated controllers
    }
    
    public int GetNextTile(Vector3 position, Vector3 direction) {
        //0:left; 1:top; 2:right; 3:bottom
        int[] neighbors = gameManager.levelGenerator.GetNeighbours((int)position.x, -(int)position.y);
        if(direction.x == -1)
            return neighbors[0];
        else if(direction.x == 1)
            return neighbors[2];
        else if(direction.y == -1)
            return neighbors[3];
        else
            return neighbors[1];
    }
    
    public bool IsWalkable(int tileNumber) {
        if(tileNumber is 0 or 5 or 6 or -1)
            return true;
        return false;
    }

}