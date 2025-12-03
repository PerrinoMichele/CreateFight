using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using System.Drawing;

public class mapGenerator : MonoBehaviour
{
    [SerializeField] int[] neighbors = { -1, 1 };

    public int width = 20; // max size, cap them
    public int length = 20;
    public float holePercentage = .3f;
    [Range(0f, 1f)] public float wallPercentage = .3f;//Max percentages, cap them
    [Range(0f, 1f)] public float treePercentage = .01f;

    public GameObject indestructablePrefab;
    public GameObject rockPrefab;
    //public GameObject exitPrefab;
    public GameObject treePrefab;

    List<Vector3Int> wallsCoordinates = new List<Vector3Int>();
    List<Vector3Int> treeCoordinates = new List<Vector3Int>();
    private List<Vector3Int> floorBlockCoordinates = new List<Vector3Int>();
    Vector3Int exitCoordinate;

    public int mapGenXInt;
    public int mapGenZInt;

    private void Start()
    {
        float mapGenX = transform.position.x;
        float mapGenZ = transform.position.z;
        mapGenXInt = (int)transform.position.x - 10;
        mapGenZInt = (int)transform.position.z - 10;

        GenerateMap();

        Vector3 targetPos = new Vector3(-4f, -1f, -0f);
        if (Physics.OverlapSphere(targetPos, 0.1f).Length == 0)
        {
            print("nothing under player");
            Instantiate(rockPrefab, targetPos, Quaternion.identity);
        }

        //width = Random.Range(8, 50);
        //length = Random.Range(10, 60);
        //wallPercentage = Random.Range(.1f, .3f);
        //enemyPercentage = Random.Range(.01f, .02f);
    }

    public void GenerateMap()
    {

        LayGround();
        //MakeSomeGroundBlocksRocks();
        DeleteSomeGroundBlocks();
        //AddBorders();
        //AddExit();
        
        AddWalls();
        AddTrees();
        InstantiateWallsRandomWithNeighbors();
    }

    private void LayGround()
    {
        for (int xpos = mapGenXInt; xpos <= mapGenXInt + width - 1; xpos++)
        {
            for (int zpos = mapGenZInt; zpos <= mapGenZInt + length - 1; zpos++)
            {
                floorBlockCoordinates.Add(new Vector3Int(xpos, -1, zpos));
            }
        }
    }

    private void DeleteSomeGroundBlocks()
    {
        int totalTiles = width * length;
        int numberOfRemovedTiles = (int)(totalTiles * holePercentage) / 3; // adjust holePercentage

        for (int i = 0; i < numberOfRemovedTiles; i++)
        {
            int baseX = Random.Range(mapGenXInt, mapGenXInt + width);
            int baseZ = Random.Range(mapGenZInt, mapGenZInt + length);

            Vector3Int basePos = new Vector3Int(baseX, -1, baseZ);
            Vector3Int neighbor1 = basePos + new Vector3Int(neighbors[Random.Range(0, 2)], 0, 0);
            Vector3Int neighbor2 = basePos + new Vector3Int(0, 0, neighbors[Random.Range(0, 2)]);

            // Remove these positions from your lower layer block list
            floorBlockCoordinates.Remove(basePos);
            floorBlockCoordinates.Remove(neighbor1);
            floorBlockCoordinates.Remove(neighbor2);
        }

        // Optional: remove duplicates if you're manipulating a combined list later
        floorBlockCoordinates = floorBlockCoordinates.Distinct().ToList();

        InstantiateFloor();
    }

    private void InstantiateFloor()
    {
        foreach (Vector3Int floorCoordinate in floorBlockCoordinates)
        {
            Instantiate(rockPrefab, floorCoordinate, Quaternion.identity);
        }
    }

    //private void MakeSomeGroundBlocksRocks()
    //{


    //    int totalNumberOfTiles = width * length;
    //    int numberOfWallTiles = (int)(totalNumberOfTiles * wallPercentage) / 3;

    //    for (int i = 1; i < numberOfWallTiles; i++)
    //    {
    //        int firstWallX = Random.Range(mapGenXInt, mapGenXInt + width);
    //        int firstWallZ = Random.Range(mapGenZInt, mapGenZInt + length);

    //        //Add neighboring walls to cluster them
    //        int secondWallX = firstWallX;
    //        int secondWallZ = firstWallZ + neighbors[Random.Range(0, 2)];

    //        int thirdWallX = firstWallX + neighbors[Random.Range(0, 2)];
    //        int thirdWallZ = firstWallZ;

    //        wallsCoordinates.Add(new Vector3Int(firstWallX, 0, firstWallZ));
    //        wallsCoordinates.Add(new Vector3Int(secondWallX, 0, secondWallZ));
    //        wallsCoordinates.Add(new Vector3Int(thirdWallX, 0, thirdWallZ));
    //    }
    //    //Make sure no duplicates
    //    wallsCoordinates = wallsCoordinates.Distinct().ToList();
    //    //Make sure player is not surrounded and exit is clear
    //    //wallsCoordinates.Remove(new Vector3Int(2, 1, 2));
    //    //wallsCoordinates.Remove(new Vector3Int(3, 1, 2));
    //    //wallsCoordinates.Remove(new Vector3Int(2, 1, 3));
    //    //wallsCoordinates.Remove(exitCoordinate);
    //}

    private void AddBorders()
    {
        for (int xPos = mapGenXInt - 1; xPos <= mapGenXInt + width; xPos++)
        {
            for (int zPos = mapGenZInt - 1; zPos <= mapGenZInt + length; zPos++)
            {
                if (xPos == mapGenXInt - 1 || xPos == mapGenXInt + width || zPos == mapGenZInt - 1 || zPos == mapGenZInt + length)
                {
                    Instantiate(indestructablePrefab, new Vector3(xPos, -1, zPos), Quaternion.identity);
                    Instantiate(indestructablePrefab, new Vector3(xPos, 0, zPos), Quaternion.identity);
                    Instantiate(indestructablePrefab, new Vector3(xPos, 1, zPos), Quaternion.identity);
                }
            }
        }
    }

    //private void AddExit()
    //{
    //    exitCoordinate = new Vector3Int(Random.Range(2, width), 1, length - 1);
    //    Instantiate(exitPrefab, exitCoordinate, Quaternion.identity);
    //}

    private void AddTrees()
    {
        int totalNumberOfTiles = width * length;
        int numberOfTrees = (int)(totalNumberOfTiles * treePercentage);

        HashSet<Vector3Int> newTrees = new HashSet<Vector3Int>();

        while (newTrees.Count < numberOfTrees)
        {
            int enemyX = Random.Range(mapGenXInt + 1, mapGenXInt + width - 1);
            int enemyZ = Random.Range(mapGenZInt + 1, mapGenZInt + length - 1);
            Vector3Int coord = new Vector3Int(enemyX, 0, enemyZ);

            // Skip if this tile is a wall or already used
            if (!wallsCoordinates.Contains(coord) && coord != exitCoordinate)
            {
                newTrees.Add(coord);
            }
        }

        treeCoordinates = newTrees.ToList();

        foreach (Vector3Int treeCoordinate in treeCoordinates)
        {
            Instantiate(treePrefab, treeCoordinate, Quaternion.identity);
        }
    }

    private void AddWalls()
    {


        int totalNumberOfTiles = width * length;
        int numberOfWallTiles = (int)(totalNumberOfTiles * wallPercentage) / 3;

        for (int i = 1; i < numberOfWallTiles; i++)
        {
            int firstWallX = Random.Range(mapGenXInt + 1, mapGenXInt + width - 1);
            int firstWallZ = Random.Range(mapGenZInt + 1, mapGenZInt + length - 1);

            //Add neighboring walls to cluster them
            int secondWallX = firstWallX;
            int secondWallZ = firstWallZ + neighbors[Random.Range(0, 2)];

            int thirdWallX = firstWallX + neighbors[Random.Range(0, 2)];
            int thirdWallZ = firstWallZ;

            wallsCoordinates.Add(new Vector3Int(firstWallX, 0, firstWallZ));
            wallsCoordinates.Add(new Vector3Int(secondWallX, 0, secondWallZ));
            wallsCoordinates.Add(new Vector3Int(thirdWallX, 0, thirdWallZ));
        }
        //Make sure no duplicates
        wallsCoordinates = wallsCoordinates.Distinct().ToList();
        //Make sure player is not surrounded and exit is clear
        //wallsCoordinates.Remove(new Vector3Int(2, 1, 2));
        //wallsCoordinates.Remove(new Vector3Int(3, 1, 2));
        //wallsCoordinates.Remove(new Vector3Int(2, 1, 3));
        //wallsCoordinates.Remove(exitCoordinate);
    }

    private void InstantiateWallsRandomWithNeighbors()
    {
        foreach (Vector3Int wallCoordinate in wallsCoordinates)
        {
            Instantiate(rockPrefab, wallCoordinate, Quaternion.identity);

        }
    }

    public void OnTreeDestroyed(Vector3 position, Quaternion rotation)
    {
        StartCoroutine(SpawnTreeAfterDelay(position, rotation));
    }

    private IEnumerator SpawnTreeAfterDelay(Vector3 pos, Quaternion rot)
    {
        yield return new WaitForSeconds(6f);

        // Retry until the spot is clear
        while (!IsPositionClear(pos))
        {
            yield return new WaitForSeconds(1f); // Wait and try again
        }

        Instantiate(treePrefab, pos, rot);
        
    }

    private bool IsPositionClear(Vector3 pos)
    {
        float checkRadius = 0.1f; // Adjust based on tree size
        Collider[] colliders = Physics.OverlapSphere(pos, checkRadius);
        return colliders.Length == 0;

    }
}

