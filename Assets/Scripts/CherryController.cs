using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryController : MonoBehaviour
{
    //movement manager
    private MovementManager movementManager;
    
    //cherry PreFab
    public GameObject cherryPrefab;
    
    //tweener
    private Tweener tweener;
    
    //10-second delay between each cherry spawn
    private float delay = 10.0f;
    
    //initialise the CherryController
    
    public void Init(MovementManager movementManager)
    {
        this.movementManager = movementManager;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        tweener = GetComponent<Tweener>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void StartCherrySpawn() {
        InvokeRepeating("SpawnCherryCoroutine", delay, delay);
    }

    public void StopCherrySpawn() {
        CancelInvoke();
    }
    
    void SpawnCherryCoroutine() {
        StartCoroutine(SpawnCherry());
    }
    
    IEnumerator SpawnCherry() {
        Vector3 randomLocation = new Vector3(Random.Range(-16.5f,10.5f), 7.0f, 0.0f);
        Debug.Log($"Cherry spawned at {randomLocation}");
        GameObject newCherry = Instantiate(cherryPrefab, randomLocation, Quaternion.identity);
        newCherry.tag = "cherry";
        CircleCollider2D cl = newCherry.AddComponent<CircleCollider2D>();
        cl.isTrigger = true;
        MoveCherryIn(newCherry);
        yield return new WaitUntil(() => newCherry == null || !tweener.TweenExists(newCherry.transform));
        MoveCherryOut(newCherry, randomLocation);
        yield return new WaitUntil(() => newCherry == null || !tweener.TweenExists(newCherry.transform));
        Destroy(newCherry);
    }
    
    //move the cherry in from the top to hit center of screen
    void MoveCherryIn(GameObject cherry) {
        tweener.AddTween(cherry.transform, 
            cherry.transform.position, 
            new Vector3(-3.0f, -7.0f, 0.0f),
            5.0f);
    }    
    
    //move the cherry out to the bottom from the center of the screen
    void MoveCherryOut(GameObject cherry, Vector3 spawnLocation) {
        //calculate the direction to move the cherry using the spawn location and the origin
        //add the direction to the cherry's current position
        
        tweener.AddTween(cherry.transform, 
            cherry.transform.position, 
            new Vector3(Random.Range(-16.5f,10.5f), -22.0f, 0.0f),
            5.0f);
    }
    
    public bool RemoveCherry(Transform cherry) {
        return tweener.RemoveTween(cherry);
    }
    
    
}
