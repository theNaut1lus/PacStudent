using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    //game manager
    private GameManager gameManager;
    
    //place the 2d level map for the initial build
    int[,] levelMap =
    {
        {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        {2,0,5,5,5,5,5,5,5,5,5,5,5,4},
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
    
    //level map but with all 4 quadrants
    public int[,] completeLvlMap;
    
    //width and height of the complete level map
    public int width;
    public int height;
    
    //count number of pellets left in the level
    public int pelletCounter;
    
    //4 quadrants of the level, each built using a tilemap, so storing their respective gameobjects
    [SerializeField]
    private Tilemap[] quadrants = new Tilemap[4];
    
    //Store all sprites for the level design needed in a list of Sprites
    //[SerializeField]
    //private List<GameObject> prefabs = new List<GameObject>();
    
    //instead of sprites, we need to store the prefabs for the tiles, this will be used to instantiate the gameobject and get the sprite from it.
    public List<Tile> prefabTiles = new List<Tile>();
    
    
    private bool tOpen = true;
    
    //initialise the LevelGenerator
    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }
    
    // Start is called before the first frame update
    //Delete the currently designed manual level in the Start function
    void Start()
    {
        CompletelvlMap();
        width = completeLvlMap.GetLength(1);
        height = completeLvlMap.GetLength(0);
        DeleteCurrentLevel();
        GenerateLevel();
    }

    void CompletelvlMap()
    {
        completeLvlMap = new int[levelMap.GetLength(0)*2-1,levelMap.GetLength(1)*2];
        for (int i = 0; i < completeLvlMap.GetLength(0); i++) {
            for (int j = 0; j < completeLvlMap.GetLength(1); j++) {
                int x = i;
                int y = j;
                if(i >= levelMap.GetLength(0))
                    x = 2*levelMap.GetLength(0)-i-2;
                if(j >= levelMap.GetLength(1))
                    y = 2*levelMap.GetLength(1)-j-1;
                completeLvlMap[i,j] = levelMap[x,y];
            }
        }
    }
    
    //Function to generate the level using the levelMap array
    public void GenerateLevel()
    {
        //loop through each quadrant and generate the level using the levelMap array
        for (int i = 0; i < quadrants.Length; i++) {
            BoundsInt bounds = quadrants[i].cellBounds;
            TileBase[] allTiles = quadrants[i].GetTilesBlock(bounds);
            //loop through each row of the levelMap array
            for (int y = 0; y < levelMap.GetLength(0); y++) {
                //loop through each column of the levelMap array
                //if the current quadrant is the bottom right or bottom left, we need to skip the last row of the levelMap array.
                if ((i == 2 || i == 3) && y == levelMap.GetLength(0) - 1) {
                    continue;
                }
                for (int x = 0; x < levelMap.GetLength(1); x++) {
                    //check if the current value in the levelMap array is not 0
                    if (levelMap[y, x] != 0) {
                        //create a new Vector3Int with the x and y values as the position, if the current quadrant is the bottom right or bottom left, we need to bump down the y value by 1 as we are skipping the last row.
                        Vector3Int position = new Vector3Int((-15)+x, 1-y, 0);
                        if (i ==2 || i == 3) {
                            position = new Vector3Int((-15)+x, 1-y-1, 0);
                        }
                        //Tile tile = ScriptableObject.CreateInstance<Tile>();
                        //tile.sprite = prefabs[levelMap[y, x] - 1].GetComponent<SpriteRenderer>().sprite;
                        //get tile based on the value in the levelMap array from the CreateTile function
                        Tile tile = CreateTile(levelMap[y, x]-1);
                        
                        //get the rotation of the tile based on the neighbours
                        Vector3 tileRotation = TileRotation(levelMap[y, x], x, y);
                        //Debug.Log($"Tile {tile.sprite} rotation: {tileRotation}");
                        tile.transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(tileRotation), new Vector3(1, 1, 1));
                        //set the tile at the position in the current quadrant
                        quadrants[i].SetTile(position, tile);
                    }
                    else {
                        //if the current value in the levelMap array is 0, set the tile at the position in the current quadrant to null
                        quadrants[i].SetTile(new Vector3Int(x, y, 0), null);
                    }
                }
            }
        }
    }
    
    //instead of just sprite, we need to now instantiate the gameobject and get the sprite from it.
    Tile CreateTile(int prefabIndex)
    {
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        
        //set the tile (with corresponding gameobject and sprite) based on the prefabIndex
        tile = prefabTiles[prefabIndex];
        
        //increment the pelletCounter if the tile is a pellet
        if (prefabIndex == 5) {
            pelletCounter++;
        }
        
        return tile;
    }
    
    //Function to rotate the tile based on the neighbours, all we need is the tile's number on the map, and it's x,y position in the input map
    Vector3 TileRotation(int tileNumber, int x, int y)
    {
        //if the tile is a start tile, that is outer corner at 0,0, no rotation needed.
        if (x == 0 && y == 0) {
            return new Vector3(0, 0, 0);
        }
        
        //We will need tileNumbers fpr all 4 neighbours of the tile to determine the rotation, top, bottom, left and right.
        //0:left; 1:top; 2:right; 3:bottom
        int[] neighbours = GetNeighbours(x, y);
        
        switch (tileNumber) {
            //outside corner
            case 1:
                //must have outside corner, outside wall or t junction adjacent
                if (   neighbours[0] == 1
                    || neighbours[0] == 2
                    || neighbours[0] == 7) 
                {
                    if (   neighbours[3] == 1
                        || neighbours[3] == 2
                        || neighbours[3] == 7)
                    {
                        return new Vector3(0.0f, 0.0f, 270.0f);
                    } else {
                        return new Vector3(0.0f, 0.0f, 180.0f);
                    }
                } else {
                    if (   neighbours[3] == 1
                        || neighbours[3] == 2
                        || neighbours[3] == 7)
                    {
                        return new Vector3(0.0f, 0.0f, 0.0f);
                    } else {
                        return new Vector3(0.0f, 0.0f, 90.0f);
                    }
                }
            //outside wall
            case 2:
                //must have outside corner, outside wall or t junction adjacent
                if ((  neighbours[0] == 1
                    || neighbours[0] == 2
                    || neighbours[0] == 7)
                    &&
                    (  neighbours[2] == 1
                    || neighbours[2] == 2
                    || neighbours[2] == 7))
                {
                    return new Vector3(0.0f, 0.0f, 90.0f);
                } else if ((   neighbours[1] == 1
                            || neighbours[1] == 2
                            || neighbours[1] == 7
                            || neighbours[1] == -1)
                        &&
                        (  neighbours[3] == 1
                        || neighbours[3] == 2
                        || neighbours[3] == 7
                        || neighbours[3] == -1))
                {
                    return new Vector3(0.0f, 0.0f, 0.0f);
                } else {
                    return new Vector3(0.0f, 0.0f, 90.0f);
                }
            //inside corner
            case 3:
                //must have inside corner, inside wall or t junction adjacent
                if (   neighbours[0] == 3
                    || neighbours[0] == 4
                    || neighbours[0] == 7) 
                {
                    if (   neighbours[3] == 3
                        || neighbours[3] == 4
                        || neighbours[3] == 7) 
                    {
                        if (neighbours[1] == 5 || neighbours[1] == 0)
                        {
                            return new Vector3(0.0f, 0.0f, 270.0f);
                        }
                        else if (neighbours[3] == 3)
                        {
                            return new Vector3(0.0f, 0.0f, 90.0f);
                        }
                        if (neighbours[1] == 3)
                        {
                            return new Vector3(0.0f, 0.0f, 0.0f);
                        }
                        else {
                            return new Vector3(0.0f, 0.0f, 270.0f);
                        }
                    } else {
                        return new Vector3(0.0f, 0.0f, 180.0f);
                    }
                } else {
                    if (   neighbours[3] == 3
                        || neighbours[3] == 4
                        || neighbours[3] == 7) 
                    {
                        return new Vector3(0.0f, 0.0f, 0.0f);
                    } else {
                        return new Vector3(0.0f, 0.0f, 90.0f);
                    }
                }
            //inside wall
            case 4:
                //must have inside corner, inside wall or t junction adjacent
                if ((  neighbours[0] == 3
                    || neighbours[0] == 4
                    || neighbours[0] == 7)
                    && 
                    (  neighbours[2] == 3
                    || neighbours[2] == 4
                    || neighbours[2] == 7))
                {
                    if (neighbours[1] == 5 || neighbours[1] == 5)
                    {
                        return new Vector3(0.0f, 0.0f, 270.0f);
                    }
                    else if (neighbours[3] == 5 || neighbours[3] == 5)
                    {
                        return new Vector3(0.0f, 0.0f, 90.0f);
                    }
                    {
                        return new Vector3(0.0f, 0.0f, 90.0f);
                    }
                    return new Vector3(0.0f, 0.0f, 270.0f);
                } else if (((  neighbours[1] == 3
                            || neighbours[1] == 4
                            || neighbours[1] == 7)
                            &&
                        (  neighbours[3] == 3
                        || neighbours[3] == 4
                        || neighbours[3] == 7
                        || neighbours[3] == 0
                        || neighbours[3] == -1))
                        ||
                        (( neighbours[1] == 3
                        || neighbours[1] == 4
                        || neighbours[1] == 7
                        || neighbours[3] == 0
                        || neighbours[3] == -1)
                        &&
                        (  neighbours[3] == 3
                        || neighbours[3] == 4
                        || neighbours[3] == 7)))
                {
                    if (neighbours[0] == 5 || neighbours[0] == 0)
                    {
                        return new Vector3(0.0f, 0.0f, 0.0f);
                    }
                    else if (neighbours[2] == 5 || neighbours[2] == 0)
                    {
                        return new Vector3(0.0f, 0.0f, 180.0f);
                    }

                    return new Vector3(0.0f, 0.0f, 0.0f);
                } else 
                {
                    if (neighbours[1] == 5 || neighbours[1] == 0)
                    {
                        return new Vector3(0.0f, 0.0f, 270.0f);
                    }
                    else if (neighbours[3] == 5 || neighbours[3] == 0)
                    {
                        return new Vector3(0.0f, 0.0f, 90.0f);
                    }
                    {
                        return new Vector3(0.0f, 0.0f, 90.0f);
                    }
                    return new Vector3(0.0f, 0.0f, 90.0f);
                }
            //t junction
            case 7:
                if ((  neighbours[0] == 1
                    || neighbours[0] == 2
                    || neighbours[0] == 7)
                    &&
                    (  neighbours[2] == 1
                    || neighbours[2] == 2
                    || neighbours[2] == 7))
                {
                    if (   neighbours[1] == 3
                        || neighbours[1] == 4
                        || neighbours[1] == 7)
                    {
                        if (tOpen) {
                            tOpen = false;
                            return new Vector3(0.0f, 0.0f, 180.0f);
                            //return new Vector3(180.0f, 0.0f, 0.0f);
                        } else {
                            tOpen = true;
                            return new Vector3(180.0f, 0.0f, 0.0f);
                            //return new Vector3(0.0f, 0.0f, 180.0f);
                        }
                    } else {
                        if (tOpen) {
                            tOpen = false;
                            return new Vector3(0.0f, 0.0f, 0.0f);
                            //return new Vector3(0.0f, 0.0f, 0.0f);
                        } else {
                            tOpen = true;
                            return new Vector3(0.0f, 0.0f, 0.0f);
                            //return new Vector3(0.0f, 180.0f, 0.0f);
                        }
                    }
                } else if ((   neighbours[1] == 1
                            || neighbours[1] == 2
                            || neighbours[1] == 7)
                            &&
                            (  neighbours[3] == 1
                            || neighbours[3] == 2
                            || neighbours[3] == 7))
                {
                    if (   neighbours[0] == 3
                        || neighbours[0] == 4
                        || neighbours[0] == 7)
                    {
                        if (tOpen) {
                            tOpen = false;
                            return new Vector3(180.0f, 0.0f, 270.0f);
                            //return new Vector3(0.0f, 0.0f, 270.0f);
                        } else {
                            tOpen = true;
                            return new Vector3(0.0f, 0.0f, 270.0f);
                            //return new Vector3(180.0f, 0.0f, 270.0f);
                        }
                    } else {
                        if (tOpen) {
                            tOpen = false;
                            return new Vector3(0.0f, 0.0f, 90.0f);
                            //return new Vector3(180.0f, 0.0f, 90.0f);
                        } else {
                            tOpen = true;
                            return new Vector3(180.0f, 0.0f, 90.0f);
                            //return new Vector3(0.0f, 0.0f, 90.0f);
                        }
                    }
                } else {
                    return new Vector3(0.0f, 0.0f, 0.0f);
                }
            //no rotation needed for pellets and empty tiles
            default:
                return new Vector3(0.0f, 0.0f, 0.0f);
        }
    }
    
    public int[] GetNeighbours(int x, int y)
    {
        //0:left; 1:top; 2:right; 3:bottom
        //initialize the neighbours array with -1, which means no tile at that position.
        int[] neighbours = {-1, -1, -1, -1};
        //check if the current position is not at the top edge of the map
        if (y > 0) {
            //if there is a tile at the top, set the top neighbour to the value of the tile
            neighbours[1] = completeLvlMap[y - 1, x];
        }
        //check if the current position is not at the bottom edge of the map
        if (y < completeLvlMap.GetLength(0) - 1) {
            //if there is a tile at the bottom, set the bottom neighbour to the value of the tile
            neighbours[3] = completeLvlMap[y + 1, x];
        }
        //check if the current position is not at the left edge of the map
        if (x > 0) {
            //if there is a tile at the left, set the left neighbour to the value of the tile
            neighbours[0] = completeLvlMap[y, x - 1];
        }
        //check if the current position is not at the right edge of the map
        if (x < completeLvlMap.GetLength(1) - 1) {
            //if there is a tile at the right, set the right neighbour to the value of the tile
            neighbours[2] = completeLvlMap[y, x + 1];
        }
        //return the neighbours array
        return neighbours;
    }

    //Best to have a separate function to delete a level, so that it can be re-used if needed.
    void DeleteCurrentLevel()
    {
        //loop through each quadrant and destroy all child gameobjects.
        for (int i = 0; i < quadrants.Length; i++)
        {
            //Debug.Log($"Destructing quadrant {quadrants[i].name}");
            //helps in shrinking the bounds of the tilemap to the minimum required size.
            quadrants[i].CompressBounds();
            //clear all tiles in the tilemap
            quadrants[i].ClearAllTiles();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
