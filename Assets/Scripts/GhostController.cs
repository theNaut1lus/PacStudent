using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    //movement manager
    private MovementManager movementManager;
    
    //import tweener because the ghost will be moving now
    private Tweener tweener;
    
    //enum for the ghost's state
    public enum GhostState
    {
        Normal,
        Scared,
        Recovering,
        Dead
    };
    
    string[] ghostAnimStates = {
        "Ghost-Left",
        "Ghost-Up",
        "Ghost-Right",
        "Ghost-Down",
    };
    
    //maintain the current state of each ghost
    private GhostState[] ghostStates =
    {
        GhostState.Normal,
        GhostState.Normal,
        GhostState.Normal,
        GhostState.Normal
    };
    
    public GhostState globalState = GhostState.Normal;
    
    //specific to ghost 4, the target tiles for the ghost to move in a clockwise loop across the map
    Vector3[] ghost4Targets = {
        new Vector3(-15.5f,6.0f,0),
        new Vector3(-1.5f,6.0f,0),
        new Vector3(9.5f,6.0f,0),
        new Vector3(9.5f,1.0f,0),
        new Vector3(4.5f,-14f,0),
        new Vector3(9.5f,-14f,0),
        new Vector3(9.5f,-21f,0),
        new Vector3(-4.5f,-21f,0),
        new Vector3(-15.5f,-21f,0),
        new Vector3(-15.5f,-14f,0),
        new Vector3(-10.5f,-1f,0),
        new Vector3(-15.5f,-1f,0),
    };
    int ghost4TargetIndex = 0;
    
    


    private int[] ghostOrigin = { -1, -1, -1, -1 };
    
    //Spawn position is center of the 4 tilemaps
    private Vector3 spawnPosition = new Vector3(-3f, -7f, 0.0f);
    
    //ghost prefabs string array for each ghost with their respective colors, contains all sprites and animators
    [SerializeField]
    private List<GameObject> ghostPrefabs;
    
    
    //bool for if ghosts are scared/edible
    public bool scared = false;
    
    //bool for if ghosts are recovering
    private bool recovering = false;
    
    //bool for if ghosts are dead
    private bool dead = false;
    
    //timer for scared and dead ghosts to return to normal
    float scaredTimer;
    //timer for each dead ghost to respawn
    float[] deadTimer = { 0, 0, 0, 0 };
    
    //ghost speed
    private float speed = 2;
    
    
    //initialise the GhostController
    public void Init(MovementManager movementManager)
    {
        this.movementManager = movementManager;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        tweener = GetComponent<Tweener>();
    }
    
    public void Normal() {
        globalState = GhostState.Normal;
        ChangeAllStates(GhostState.Normal);
        foreach(GameObject ghost in ghostPrefabs) {
            Animator animator = ghost.GetComponent<Animator>();
            animator.Play("Ghost-Right");
        }
        movementManager.gameManager.uiManager.DisableGhostTimer();
    }
    public void Scared() {
        globalState = GhostState.Scared;
        ChangeAllStates(GhostState.Scared);
        scaredTimer = 10.0f;
        foreach(GameObject ghost in ghostPrefabs) {
            Animator animator = ghost.GetComponent<Animator>();
            animator.Play("Ghost-Scared");
        }
    }

    public void Recover()
    {
        globalState = GhostState.Recovering;
        ChangeAllStates(GhostState.Recovering);
        foreach(GameObject ghost in ghostPrefabs) {
            Animator animator = ghost.GetComponent<Animator>();
            animator.Play("Ghost-Recovering");
        }
    }

    public void Die(GameObject ghost) {
        int index = ghostPrefabs.IndexOf(ghost);
        ghostStates[index] = GhostState.Dead;
        deadTimer[index] = 5.0f;
        Animator animator = ghost.GetComponent<Animator>();
        animator.Play("Ghost-Dead");
    }
    
    void Resurrect(int index) {
        deadTimer[index] = 0;
        ghostStates[index] = globalState;
        Animator animator = ghostPrefabs[index].GetComponent<Animator>();
        if(ghostStates[index] == GhostState.Recovering)
            animator.Play("Ghost-Recovering");
        else if(ghostStates[index] == GhostState.Scared)
            animator.Play("Ghost-Scared");
        else
            animator.Play("Ghost-Right");
    }

    // Update is called once per frame
    void Update()
    {
        if(movementManager.gameManager.isGameStarted) {
            if(globalState == GhostState.Scared || globalState == GhostState.Recovering) {
                movementManager.gameManager.uiManager.SetGhostTimer(scaredTimer);
                if(scaredTimer > 0) {
                    scaredTimer -= Time.deltaTime;
                    if(scaredTimer <= 3.0f && globalState == GhostState.Scared) {
                        Recover();
                    }
                } else {
                    Normal();
                }
            }

            for(int i = 0; i < ghostPrefabs.Count; i++) {
                if(ghostStates[i] == GhostState.Dead) {
                    if(InStartArea(ghostPrefabs[i].transform.position) || deadTimer[i] <= 0)
                        Resurrect(i);
                    else {
                        deadTimer[i] -= Time.deltaTime;
                        DeadMovement(i);
                    }
                } else if(InStartArea(ghostPrefabs[i].transform.position)) {
                    LeaveStartArea(i);
                } else if(ghostStates[i] == GhostState.Scared
                          || ghostStates[i] == GhostState.Recovering) {
                    GhostOneMovement(i);
                } else {
                    if(i == 0)
                        GhostOneMovement(i);
                    else if(i == 1)
                        GhostTwoMovement(i);
                    else if(i == 2)
                        GhostThreeMovement(i);
                    else if(i == 3)
                        GhostFourMovement(i);
                }
            }
        }
    }
    
    

    public bool GhostIsDead() {
        foreach(GhostState state in ghostStates) {
            if(state == GhostState.Dead)
                return true;
        }
        return false;
    }

    public GhostState GetState(GameObject ghost) {
        int index = ghostPrefabs.IndexOf(ghost);
        return ghostStates[index];
    }

    bool InStartArea(Vector3 pos) {
        return (pos.x > -7.5f
            && pos.x < 1.5f
            && pos.y > -11
            && pos.y < -4);
    }

    public bool CanDie(GameObject ghost) {
        int index = ghostPrefabs.IndexOf(ghost);
        return (ghostStates[index] == GhostState.Scared 
            || ghostStates[index] == GhostState.Recovering);
    }

    void ChangeAllStates(GhostState newState) {
        for(int i = 0; i < ghostStates.Length; i++) {
            if(!(ghostStates[i] == GhostState.Dead && deadTimer[i] > 0))
                ghostStates[i] = newState;
        }
    }

    Vector3 GetNextPosition(Vector3 pos, int neighborIndex) {
        if(neighborIndex == 0) {
            return pos + Vector3.left;
        } else if(neighborIndex == 1) {
            return pos + Vector3.up;
        } else if(neighborIndex == 2) {
            return pos + Vector3.right;
        } else {
            return pos + Vector3.down;
        }
    }

    bool IsWalkable(int tileNumber, Vector3 nextPos) {
        return tileNumber != -1 
            && movementManager.IsWalkable(tileNumber)
            && !InStartArea(nextPos);
    }

    void MoveGhost(int index, Vector3 target, int origin) {
        ghostOrigin[index] = origin;
        int direction = (origin+2)%4;
        if(ghostStates[index] == GhostState.Normal)
            ghostPrefabs[index].GetComponent<Animator>().Play(ghostAnimStates[direction]);
        tweener.AddTween(ghostPrefabs[index].transform, ghostPrefabs[index].transform.position, target, 1/speed);
    }

    void LeaveStartArea(int index) {
        // First go to center and then either up or down
        // depending on which ghost
        Vector3 currentPos = ghostPrefabs[index].transform.position;
        if(currentPos.x < -3.5)
            MoveGhost(index, ghostPrefabs[index].transform.position + Vector3.right, 0);
        else if(currentPos.x > -2.5)
            MoveGhost(index, ghostPrefabs[index].transform.position + Vector3.left, 2);
        else {
            // ghost 1 move down
            if(index == 0)
                MoveGhost(index, ghostPrefabs[index].transform.position + Vector3.down, 1);
            // ghost 2 move up
            else if(index == 1)
                MoveGhost(index, ghostPrefabs[index].transform.position + Vector3.up, 3);
            // ghost 3+4 move randomly up or down
            else {
                if(ghostOrigin[index] == 1)
                    MoveGhost(index, ghostPrefabs[index].transform.position + Vector3.down, 1);
                else if(ghostOrigin[index] == 3)
                    MoveGhost(index, ghostPrefabs[index].transform.position + Vector3.up, 3);
                else {
                    float random = Random.Range(0,1);
                    if(random > 0.5f)
                        MoveGhost(index, ghostPrefabs[index].transform.position + Vector3.up, 3);
                    else
                        MoveGhost(index, ghostPrefabs[index].transform.position + Vector3.down, 1);
                }
            }
        }

    }
    
    //ghost 1 movement: move to a tile that is at a distance >= from pacstudent
    void GhostOneMovement(int index) {
        if(!tweener.TweenExists(ghostPrefabs[index].transform)) {
            //0:left; 1:top; 2:right; 3:bottom
            int[] neighbours = movementManager.gameManager.levelGenerator.GetNeighbours((int)(ghostPrefabs[index].transform.position.x + 16.5f), -(int)(ghostPrefabs[index].transform.position.y - 7.0f));
            int previousTile = ghostOrigin[index];
            Vector3 nextPosition;
            
            // Check if one of neighboring Tiles fulfills 
            // condition of greater distance and walk there if so
            // Start at random index and loop through all neigbouring tiles
            int start = Random.Range(0,4);
            for(int i = 0; i < 4; i++) {
                int neighborIndex = (i+start)%4;
                if(neighborIndex != previousTile) {
                    int neighborTile = neighbours[neighborIndex];
                    nextPosition = GetNextPosition(ghostPrefabs[index].transform.position, neighborIndex);
                    if(IsWalkable(neighborTile, nextPosition)) {
                        Vector3 pacPosition = movementManager.pacStudentController.transform.position;
                        float distance = Vector3.Distance(ghostPrefabs[index].transform.position, pacPosition);
                        if(distance <= Vector3.Distance(nextPosition, pacPosition)) {
                            MoveGhost(index, nextPosition, (neighborIndex+2)%4);
                            return;
                        }
                    }
                }
            }

            // Check if ghost can walk anywhere else without walking back
            // else walk back
            for(int i = 0; i < 4; i++) {
                int neighborIndex = (i+previousTile+1)%4;
                int neighborTile = neighbours[neighborIndex];
                nextPosition = GetNextPosition(ghostPrefabs[index].transform.position, neighborIndex);
                if(IsWalkable(neighborTile, nextPosition)) {
                    MoveGhost(index, nextPosition, (neighborIndex+2)%4);
                    return;
                }
            }

        }
    }
    
    //ghost 2 movement: move to a tile that is at a distance <= from pacstudent
    void GhostTwoMovement(int index) {
        if(!tweener.TweenExists(ghostPrefabs[index].transform)) {
            //0:left; 1:top; 2:right; 3:bottom
            int[] neighbours = movementManager.gameManager.levelGenerator.GetNeighbours((int)(ghostPrefabs[index].transform.position.x + 16.5f), -(int)(ghostPrefabs[index].transform.position.y - 7.0f));
            int previousTile = ghostOrigin[index];
            Vector3 nextPosition;
            
            // Check if one of neighboring Tiles fulfills 
            // condition of greater distance and walk there if so
            // Start at random index and loop through all neigboring tiles
            int start = Random.Range(0,4);
            for(int i = 0; i < 4; i++) {
                int neighborIndex = (i+start)%4;
                if(neighborIndex != previousTile) {
                    int neighborTile = neighbours[neighborIndex];
                    nextPosition = GetNextPosition(ghostPrefabs[index].transform.position, neighborIndex);
                    if(IsWalkable(neighborTile, nextPosition)) {
                        Vector3 pacPosition = movementManager.pacStudentController.transform.position;
                        float distance = Vector3.Distance(ghostPrefabs[index].transform.position, pacPosition);
                        if(distance >= Vector3.Distance(nextPosition, pacPosition)) {
                            MoveGhost(index, nextPosition, (neighborIndex+2)%4);
                            return;
                        }
                    }
                }
            }

            // Check if ghost can walk anywhere else without walking back
            // else walk back
            for(int i = 0; i < 4; i++) {
                int neighborIndex = (i+previousTile+1)%4;
                int neighborTile = neighbours[neighborIndex];
                nextPosition = GetNextPosition(ghostPrefabs[index].transform.position, neighborIndex);
                if(IsWalkable(neighborTile, nextPosition)) {
                    MoveGhost(index, nextPosition, (neighborIndex+2)%4);
                    return;
                }
            }

        }
    }
    
    //ghost 3 movement: move to a random tile that is walkable
    void GhostThreeMovement(int index) {
        if(!tweener.TweenExists(ghostPrefabs[index].transform)) {
            //0:left; 1:top; 2:right; 3:bottom
            int[] neighbours = movementManager.gameManager.levelGenerator.GetNeighbours((int)(ghostPrefabs[index].transform.position.x + 16.5f), -(int)(ghostPrefabs[index].transform.position.y - 7.0f));
            int previousTile = ghostOrigin[index];
            Vector3 nextPosition;
            
            // Start at random index and loop through all neigboring tiles
            // Check if ghost can walk anywhere without walking back
            int start = Random.Range(0,4);
            for(int i = 0; i < 4; i++) {
                int neighborIndex = (i+start)%4;
                if(neighborIndex != previousTile) {
                    int neighborTile = neighbours[neighborIndex];
                    nextPosition = GetNextPosition(ghostPrefabs[index].transform.position, neighborIndex);
                    if(IsWalkable(neighborTile, nextPosition)) {
                        MoveGhost(index, nextPosition, (neighborIndex+2)%4);
                        return;
                    }
                }
            }

            // walk back
            nextPosition = GetNextPosition(ghostPrefabs[index].transform.position, previousTile);
            MoveGhost(index, nextPosition, (previousTile+2)%4);
        }
    }

    bool IsAtOuterWall(Vector3 pos) {
        int[] neighbours = movementManager.gameManager.levelGenerator.GetNeighbours((int)pos.x, -(int)pos.y);
        
        // Either an outer wall is adjacent
        foreach(int tileNumber in neighbours) {
            if(tileNumber == 1 || tileNumber == 2 || tileNumber == 7)
                return true;
        }

        // or ghost is at these coordinates
        return (pos.x > 11 && pos.x < 16 && (pos.y > -6 || pos.y < -22));
    }

    //ghost 4 movement: Move clockwise around the map, following the outside the wall

    void GhostFourMovement(int index) {
        if(!tweener.TweenExists(ghostPrefabs[index].transform)) {
            //0:left; 1:top; 2:right; 3:bottom
            int[] neighbours = movementManager.gameManager.levelGenerator.GetNeighbours((int)(ghostPrefabs[index].transform.position.x + 16.5f), -(int)(ghostPrefabs[index].transform.position.y - 7.0f));
            int previousTile = ghostOrigin[index];
            Vector3 nextPosition;
            
            // Check if one of neighboring Tiles is closer at 
            // target position and walk there if so
            // Start at random index and loop through all neigboring tiles
            int start = Random.Range(0,4);
            for(int i = 0; i < 4; i++) {
                int neighborIndex = (i+start)%4;
                if(neighborIndex != previousTile) {
                    int neighborTile = neighbours[neighborIndex];
                    nextPosition = GetNextPosition(ghostPrefabs[index].transform.position, neighborIndex);
                    if(IsWalkable(neighborTile, nextPosition)) {
                        Vector3 targetPosition = ghost4Targets[ghost4TargetIndex];
                        float distance = Vector3.Distance(ghostPrefabs[index].transform.position, targetPosition);
                        if(distance >= Vector3.Distance(nextPosition, targetPosition)) {
                            if(nextPosition == targetPosition)
                                ghost4TargetIndex = (ghost4TargetIndex+1)%ghost4Targets.Length;
                            MoveGhost(index, nextPosition, (neighborIndex+2)%4);
                            return;
                        }
                    }
                }
            }

            // Check if ghost can walk anywhere else without walking back
            // else walk back
            for(int i = 0; i < 4; i++) {
                int neighbourIndex = (i+previousTile+1)%4;
                int neighbourTile = neighbours[neighbourIndex];
                nextPosition = GetNextPosition(ghostPrefabs[index].transform.position, neighbourIndex);
                if(IsWalkable(neighbourTile, nextPosition)) {
                    MoveGhost(index, nextPosition, (neighbourIndex+2)%4);
                    return;
                }
            }
        }
    }

    void DeadMovement(int index) {
        Transform gTransform = ghostPrefabs[index].transform;
        if(InStartArea(gTransform.position)) {
            Resurrect(index);
        } else {
            if(!tweener.TweenExists(gTransform)) {
                tweener.AddTween(gTransform, gTransform.position, spawnPosition, 3);
            }
        }
    }
}
