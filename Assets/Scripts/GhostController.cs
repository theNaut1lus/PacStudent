using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    //movement manager
    private MovementManager movementManager;
    
    //ghost prefabs dictionary for each ghost with their respective colors
    public Dictionary<string, GameObject> ghostPrefabs = new Dictionary<string, GameObject>();
    
    //bool for if ghosts are scared/edible
    public bool scared = false;
    
    //bool for if ghosts are recovering
    private bool recovering = false;
    
    //bool for if ghosts are dead
    private bool dead = false;
    
    //timer for scared and dead ghosts to return to normal
    float scaredTimer;
    float deadTimer;
    
    //[TODO]: Implement the ghost movement
    
    
    //initialise the GhostController
    public void Init(MovementManager movementManager)
    {
        this.movementManager = movementManager;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public void Normal() {
        scared = false;
        recovering = false;
        dead = false;
        foreach(KeyValuePair<string, GameObject> kvp in ghostPrefabs) {
            Animator animator = kvp.Value.GetComponent<Animator>();
            animator.Play($"Ghost-{kvp.Key}");
        }
        movementManager.gameManager.audioManager.PlayNormalMusic();
        movementManager.gameManager.uiManager.DisableGhostTimer();
    }
    public void Scared() {
        scared = true;
        scaredTimer = 10.0f;
        foreach(KeyValuePair<string, GameObject> kvp in ghostPrefabs) {
            Animator animator = kvp.Value.GetComponent<Animator>();
            animator.Play("Ghost-Scared");
        }
        movementManager.gameManager.audioManager.PlayScaredMusic();
    }

    public void Recover() {
        recovering = true;
        foreach(KeyValuePair<string, GameObject> kvp in ghostPrefabs) {
            Animator animator = kvp.Value.GetComponent<Animator>();
            animator.Play("Ghost-Recovering");
        }
    }

    public void Die(GameObject ghost) {
        dead = true;
        deadTimer = 5.0f;
        Animator animator = ghost.GetComponent<Animator>();
        animator.Play("Ghost-Dead");
        movementManager.gameManager.audioManager.PlayDeadMusic();
    }

    // Update is called once per frame
    void Update()
    {
        if(scared) {
            movementManager.gameManager.uiManager.SetGhostTimer(scaredTimer);
            if(scaredTimer > 0) {
                scaredTimer -= Time.deltaTime;
                if(scaredTimer <= 3.0f && !recovering) {
                    Recover();
                }
            } else {
                Normal();
            }
        }
        if(dead) {
            if(deadTimer > 0) {
                deadTimer -= Time.deltaTime;
            } else {
                Normal();
            }
        }
    }
}
