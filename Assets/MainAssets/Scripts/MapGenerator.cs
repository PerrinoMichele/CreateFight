using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

public class mapGenerator : MonoBehaviour
{
    [SerializeField] int[] neighbors = { -1, 1 };

    public int width = 20; // max size, cap them
    public int length = 20;
    public float holePercentage = .3f;
    [Range(0f, 1f)] public float wallPercentage = .3f;//Max percentages, cap them
    [Range(0f, 1f)] public float enemyPercentage = .01f;

    public GameObject indestructablePrefab;
    public GameObject rockPrefab;
    //public GameObject exitPrefab;
    public GameObject enemyPrefab;

    List<Vector3Int> wallsCoordinates = new List<Vector3Int>();
    List<Vector3Int> enemiesCoordinates = new List<Vector3Int>();
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
        AddBorders();
        //AddExit();
        //AddEnemies();
        AddWalls();
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
            Instantiate(rockPrefab, floorCoordinate, Quaternion.identity, transform);
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
                    Instantiate(indestructablePrefab, new Vector3(xPos, -1, zPos), Quaternion.identity, transform);
                    Instantiate(indestructablePrefab, new Vector3(xPos, 0, zPos), Quaternion.identity, transform);
                    Instantiate(indestructablePrefab, new Vector3(xPos, 1, zPos), Quaternion.identity, transform);
                }
            }
        }
    }

    //private void AddExit()
    //{
    //    exitCoordinate = new Vector3Int(Random.Range(2, width), 1, length - 1);
    //    Instantiate(exitPrefab, exitCoordinate, Quaternion.identity);
    //}

    //private void AddEnemies()
    //{
    //    int totalNumberOfTiles = width * length;
    //    int numberOfEnemies = (int)(totalNumberOfTiles * enemyPercentage);

    //    for (int i = 1; i < numberOfEnemies; i++)
    //    {
    //        int EnemyX = Random.Range(1, width);
    //        int EnemyZ = Random.Range(1, length);
    //        enemiesCoordinates.Add(new Vector3Int(EnemyX, 1, EnemyZ));
    //    }
    //    //Make sure no duplicates
    //    enemiesCoordinates = enemiesCoordinates.Distinct().ToList();

    //    foreach (Vector3Int enemyCoordinate in enemiesCoordinates)
    //    {
    //        if (enemyCoordinate != exitCoordinate)
    //        {
    //            Instantiate(enemyPrefab, enemyCoordinate, Quaternion.identity);
    //        }
    //    }
    //}

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
            Instantiate(rockPrefab, wallCoordinate, Quaternion.identity, transform);

        }
    }
}

