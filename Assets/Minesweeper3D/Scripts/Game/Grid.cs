using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public GameObject tilePrefab;
    public int width = 10, height = 10, depth = 10;
    public float spacing = 1f;
    public bool hasLost;
    public Tile[,,] tiles;
    // Use this for initialization
    void Start()
    {
        hasLost = false;
        GenerateTiles();
    }
    Tile SpawnTile(Vector3 pos)
    {
        GameObject clone = Instantiate(tilePrefab, transform);



        clone.transform.position = pos;

        Tile currentTile = clone.GetComponent<Tile>();

        return currentTile;
    }
    private void Update()
    {
        MouseOver();

    }
    void GenerateTiles()
    {
        tiles = new Tile[width, height, depth];
        //store half size for later use
        Vector3 halfSize = new Vector3(width * 0.5f, height * 0.5f, depth * 0.5f);

        //offset
        Vector3 offset = new Vector3(1f, 1f, 1f);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {

                    //pivot tiles around grid
                    Vector3 pos = new Vector3(x - halfSize.x, y - halfSize.y, z - halfSize.z);

                    //apply offset
                    pos += offset;

                    //apply spacing
                    pos *= spacing;
                    //spawn the tile
                    Tile newTile = SpawnTile(pos);
                    //set transform of the tiles to the grid

                    //store its array coords
                    newTile.x = x;
                    newTile.y = y;
                    newTile.z = z;
                    //stores tile in array at those coordss
                    tiles[x, y, z] = newTile;
                }

            }
        }
    }
    public int GetAdjacentMineCount(Tile tile)
    {
        //set count to 0
        int count = 0;
        // Loop through all the adjacent tiles on the x
        for (int x = -1; x <= 1; x++)
        {
            //loop through all the adjacent tiles on the y
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    //calculate which adjacent tile to look at
                    int desiredX = tile.x + x;
                    int desiredY = tile.y + y;
                    int desiredZ = tile.z + z;
                    //check if the desired x & y is outside bounds
                    if (IsOutOfBounds(desiredX, desiredY, desiredZ))
                    {
                        //Continue to next element in loop
                        continue;
                    }
                    //select current tile
                    Tile currentTile = tiles[desiredX, desiredY, desiredZ];
                    //if the selected tile is a mine
                    if (currentTile.isMine)
                    {
                        //increase increment by 1
                        count++;
                    }
                }

            }
        }
        //remember to return the count
        return count;
    }
    void MouseOver()
    {
        if (!hasLost)
        {
            //if left mouse button is clicked
            if (Input.GetMouseButtonDown(0))
            {

                Tile hitTile = GetHitTile(Input.mousePosition);
                if (hitTile)
                {
                    SelectTile(hitTile);
                }

            }
            if (Input.GetMouseButtonDown(1))
            {
                Tile hitTile = GetHitTile(Input.mousePosition);

                if (hitTile)
                {
                    hitTile.Flag();
                }

            }

        }

    }
    //FF = Flood Fill Algorithm
    void FFunCover(int x, int y, int z, bool[,,] visited)
    {
        // is x  y and z out of bounds of the grid
        if (IsOutOfBounds(x, y, z))
        {
            //exit
            return;
        }
        //have these coordinates been visited
        if (visited[x, y, z])
        {
            return;
        }


        Tile tile = tiles[x, y, z];
        //reveal tile in that x and y coord
        int adjacentMines = GetAdjacentMineCount(tile);
        //Reveal tile in that x and y coord
        tile.Reveal(adjacentMines);
        //if there were no more adjacent tiles around that tile
        if (adjacentMines == 0)
        {
            //this tile has been visited
            visited[x, y, z] = true;
            //visit all the other tiles around this tile
            FFunCover(x - 1, y, z, visited);
            FFunCover(x + 1, y, z, visited);
            FFunCover(x, y - 1, z, visited);
            FFunCover(x, y + 1, z, visited);
            FFunCover(x, y, z - 1, visited);
            FFunCover(x, y, z + 1, visited);
        }

    }
    void UncoverAllMines()
    {
        //loop through 2d array
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    Tile tile = tiles[x, y, z];
                    //Check if tile is a mine
                    if (tile.isMine)
                    {
                        //reveal mine tile

                        tile.Reveal();
                    }
                }

            }
        }
    }
    //scans the grid to check if there are no more empty tiles
    bool NoMoreEmptyTiles()
    {
        //set empty tile count to xero
        int emptyTileCount = 0;
        //loop through 2d array
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    Tile tile = tiles[x, y, z];
                    if (tile.isRevealed || tile.isMine)
                    {
                        //skip next loop iteration
                        continue;
                    }
                    //its a empty tile
                    emptyTileCount++;
                }
            }
        }
        return emptyTileCount == 0;
    }
    //uncovers a selected tile
    void SelectTile(Tile selected)
    {

        int selectedTile = GetAdjacentMineCount(selected);
        selected.Reveal(selectedTile);

        //if the tile selected is a mine
        if (selected.isMine)
        {
            //function to uncover the mine
            UncoverAllMines();
            Lose();
            //lose
        }
        //otherwise, are there no mines around this tile
        else if (selectedTile == 0)
        {

            int x = selected.x;
            int y = selected.y;
            int z = selected.z;
            //use flood fill algorith to uncover all adjacent mines
            FFunCover(x, y, z, new bool[width, height, depth]);
        }
        //if there are no more empty tiles
        if (NoMoreEmptyTiles())
        {
            //uncover the mines, the 1 being win state
            UncoverAllMines();
        }
    }
    bool IsOutOfBounds(int x, int y, int z)
    {
        return x < 0 || x >= width || y < 0 || y >= height || z < 0 || z >= depth;

    }
    Tile GetHitTile(Vector2 mousePos)
    {
        //shoots a ray from the main camera to the input positon
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        //hit data from raycast
        RaycastHit hit;
        if (Physics.Raycast(mouseRay, out hit))
        {

            //get Tile component from the collider
            return hit.collider.GetComponent<Tile>();
        }
        return null;
    }
    void UpdateGrid()
    {
        // Store half the size of the grid
        Vector3 halfSize = new Vector3(width * .5f, height * .5f, depth * .5f);

        // Offset
        Vector3 offset = new Vector3(.5f, .5f, .5f);

        // Loop through the entire list of tiles
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    // Generate position for current tile
                    Vector3 position = new Vector3(x - halfSize.x,
                                                   y - halfSize.y,
                                                   z - halfSize.z);
                    // Offset position to center
                    position += offset;
                    // Apply spacing
                    position *= spacing;
                    // Spawn a new tile
                    Tile tile = tiles[x, y, z];


                    tile.transform.position = position;
                }
            }
        }
    }
    #region Ending
    void Lose()
    {
        hasLost = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }
    #endregion
}
