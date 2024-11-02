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
        Vector3 randomLocation = new Vector3(Random.Range(0.0f,27.0f), 1.0f, 0.0f);
        GameObject newCherry = Instantiate(cherryPrefab, randomLocation, Quaternion.identity);
        newCherry.tag = "Cherry";
        CircleCollider2D cl = newCherry.AddComponent<CircleCollider2D>();
        cl.isTrigger = true;
        MoveCherry(newCherry);
        yield return new WaitUntil(() => newCherry == null || !tweener.TweenExists(newCherry.transform));
        Destroy(newCherry);
    }
    
    void MoveCherry(GameObject cherry) {
        tweener.AddTween(cherry.transform, 
            cherry.transform.position, 
            new Vector3(27.0f - cherry.transform.position.x, -30.0f, 0.0f),
            10.0f);
    }
    
    public bool RemoveCherry(Transform cherry) {
        return tweener.RemoveTween(cherry);
    }
    
    
}
