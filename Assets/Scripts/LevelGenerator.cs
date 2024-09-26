using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    //place the 2d level map for the initial build
    int[,] levelMap =
    {
        {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
        {2,6,4,0,0,4,5,4,0,0,0,4,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
        {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
        {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
        {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
        {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
        {0,0,0,0,0,2,5,4,3,4,4,3,0,3},
        {0,0,0,0,0,2,5,4,4,0,0,0,0,0},
        {0,0,0,0,0,2,5,4,4,0,3,4,4,0},
        {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
        {0,0,0,0,0,0,5,0,0,0,4,0,0,0},
    };
    
    //4 quadrants of the level, each built using a tilemap, so storing their respective gameobjects
    [SerializeField]
    private Tilemap[] quadrants = new Tilemap[4];
    
    //Store all sprites for the level design needed in a list of Sprites
    [SerializeField]
    private List<Sprite> sprites = new List<Sprite>();
    
    // Start is called before the first frame update
    //Delete the currently designed manual level in the Start function
    void Start()
    {
        DeleteCurrentLevel();
    }

    //Best to have a separate function to delete a level, so that it can be re-used if needed.
    void DeleteCurrentLevel()
    {
        Debug.Log("deleting current level");
        //loop through each quadrant and destroy all child gameobjects.
        for (int i = 0; i < quadrants.Length; i++) {
            Debug.Log($"Destructing quadrant {quadrants[i].name}");
            quadrants[i].ClearAllTiles();
            
            //BoundsInt bounds = Quadrants[i].cellBounds;
            //TileBase[] allTiles = Quadrants[i].GetTilesBlock(bounds);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
