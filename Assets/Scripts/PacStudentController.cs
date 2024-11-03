using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    MovementManager movementManager;
    
    private Tweener tweener;
    
    //particle systems
    private ParticleSystem trailParticles;
    private ParticleSystem bumpParticles;
    private bool bumped = false;
    
    private float speed = 2;

    private int lastInput = -1;

    private int currentInput;

    public Animator pacStudentAnimator;
    
    private Vector3 spawnPosition = new Vector3(-15.5f, 6.0f, 0.0f);
    
    //list of each animation state of pacStudent
    private string[] pacStudentAnimationStates = new string[]
    {
        "Up",
        "Right",
        "Down",
        "PacStudentLeft"
        
    };
    //list of each vector3 direction in which pacStudent can be moving in
    private Vector3[] directions = new Vector3[]
    {
        Vector3.up,
        Vector3.left,
        Vector3.down,
        Vector3.right
    };
    //list of each euler angle rotation of pacStudent, corresponding to each direction
    private Quaternion[] particleRotations = new Quaternion[]
    {
        Quaternion.Euler(0.0f, 0.0f, 260.0f),
        Quaternion.Euler(0.0f, 0.0f, 350.0f),
        Quaternion.Euler(0.0f, 0.0f, 80.0f),
        Quaternion.Euler(0.0f, 0.0f, 170.0f)
    };
    
    //initialise the Controller
    public void Init(MovementManager movementManager)
    {
        this.movementManager = movementManager;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        tweener = GetComponent<Tweener>();
        pacStudentAnimator = GetComponent<Animator>();
        trailParticles = GameObject.Find("TrailParticles").GetComponent<ParticleSystem>();
        bumpParticles = GameObject.Find("BumpParticles").GetComponent<ParticleSystem>();
        //by default stop the particle systems
        trailParticles.Stop();
        bumpParticles.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if(movementManager.gameManager.isGameStarted) {
            if(Input.GetKeyDown(KeyCode.W)) {
                lastInput = 0;
                if(movementManager.gameManager.isPaused)
                    movementManager.gameManager.Play();
            }
            if(Input.GetKeyDown(KeyCode.A)) {
                lastInput = 1;
                if(movementManager.gameManager.isPaused)
                    movementManager.gameManager.Play();
            }
            if(Input.GetKeyDown(KeyCode.S)) {
                lastInput = 2;
                if(movementManager.gameManager.isPaused)
                    movementManager.gameManager.Play();
            }
            if(Input.GetKeyDown(KeyCode.D)) {
                lastInput = 3;
                if(movementManager.gameManager.isPaused)
                    movementManager.gameManager.Play();
            }
            
            if(!movementManager.gameManager.isPaused)
                Move();
        }
    }
    
    void Move() {
        if(!tweener.TweenExists(transform) && lastInput != -1) {            
            Vector3 direction = directions[lastInput];
            //rotate the pacStudent to face the direction it is moving in
            //transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f * lastInput);
            //delta of (16.5, -7) to be added to the current position of pacStudent to get its current tile position on the grid
            Vector3 currentTile = new Vector3(transform.position.x + 16.5f, transform.position.y - 7.0f, 0.0f);
            //Debug.Log($"pacstudent position: {transform.position}, current tile: {currentTile} and direction: {direction}");
            int nextTile = movementManager.GetNextTile(currentTile, direction);

            if(nextTile == -1)
                Teleport(direction);
            else {   
                if(movementManager.IsWalkable(nextTile)) {
                    currentInput = lastInput;
                    tweener.AddTween(transform, transform.position, transform.position + direction, 1/speed);
                    pacStudentAnimator.enabled = true;
                    pacStudentAnimator.Play(pacStudentAnimationStates[currentInput]);
                    trailParticles.transform.rotation = particleRotations[currentInput];
                    trailParticles.Play();
                    bumped = false;
                    movementManager.gameManager.audioManager.PlayMoveSound();
                } else {
                    direction = directions[currentInput];
                    nextTile = movementManager.GetNextTile(currentTile, direction);
                    if(movementManager.IsWalkable(nextTile)) {
                        tweener.AddTween(transform, transform.position, transform.position + direction, 1/speed);
                        pacStudentAnimator.enabled = true;
                        pacStudentAnimator.Play(pacStudentAnimationStates[currentInput]);
                        trailParticles.transform.rotation = particleRotations[currentInput];
                        trailParticles.Play();
                        bumped = false;
                        movementManager.gameManager.audioManager.PlayMoveSound();
                    } else {
                        pacStudentAnimator.enabled = false;
                        trailParticles.Stop();
                        if(!bumped) {
                            bumpParticles.Play();
                            bumped = true;
                            movementManager.gameManager.audioManager.PlayCollisionSound();
                        }
                    }
                }
            }
        }
    }
    
    void Teleport(Vector3 direction) {
        //if direction is left, teleport to the right edge of the grid, else teleport to the left edge of the grid
        if(direction == directions[1]) {
            float newX = Mathf.Abs(transform.position.x + movementManager.gameManager.levelGenerator.width - 1);
            transform.position = new Vector3(newX, transform.position.y, 0.0f);
        }
        else {
            float newX = Mathf.Abs(transform.position.x - movementManager.gameManager.levelGenerator.width + 1);
            transform.position = new Vector3(-newX, transform.position.y, 0.0f);
        }
        
    }
    
    void Eat(GameObject pellet) {
        Destroy(pellet);
    }
    
    void OnTriggerEnter2D(Collider2D other) {
        string tag = other.gameObject.tag;
        if(tag == "pellet") {
            movementManager.gameManager.audioManager.PlayEatSound();
            Eat(other.gameObject);
            AddScore(10);
        } else if(tag == "cherry") {
            movementManager.cherryController.RemoveCherry(other.transform);
            Eat(other.gameObject);
            AddScore(100);
        } else if(tag == "pellet-power") {
            Eat(other.gameObject);
            movementManager.ghostController.Scared();
        } else if(tag == "ghost") {
            if(movementManager.ghostController.CanDie(other.gameObject)) {
                movementManager.ghostController.Die(other.gameObject);
                AddScore(300);
            } else if(movementManager.ghostController.GetState(other.gameObject) != GhostController.GhostState.Dead) {
                Die();
            }
        }
    }
    
    void AddScore(int x) {
        movementManager.gameManager.AddScore(x);
    }
    
    void Die() {
        movementManager.gameManager.Pause();
        tweener.RemoveTween(transform);
        pacStudentAnimator.Play("Death");
        movementManager.gameManager.audioManager.PlayDieSound();
        movementManager.gameManager.uiManager.LoseLife();
        if(movementManager.gameManager.lives > 0) {
            Invoke(nameof(MoveToStart), 1.0f);
        } else {
            movementManager.gameManager.GameOver();
        }
    }

    public void MoveToStart() {
        pacStudentAnimator.Play("Right");
        trailParticles.Stop();
        lastInput = -1;
        transform.position = spawnPosition;
    }
}
