using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MyPathSystem : MonoBehaviour {

    public enum SeedType { RANDOM, CUSTOM }
    [Header("Random Data")]
    public SeedType seedType = SeedType.RANDOM;

    System.Random random;
    public int seed = 0;

    //Dylan's Variables
    public GameObject roomPrefab;
    //Prefab for player
    public GameObject playerPrefab;
    //We need to know the direction the last object went before it.
    public string lastDirection = "null";
    //We need a list to store all the roomtypes
    public List<GameObject> roomTypes = new List<GameObject>();


    [Space]
    public bool animatedPath;
    public List<GridCell> gridCellList = new List<GridCell>();
    public int pathLength = 10;

    [Range(1.0f, 7.0f)]
    public float cellSize = 1.0f;

    void SpawnPlayer()
    {
        Instantiate(playerPrefab, new Vector2(-15.0f, -9.0f), Quaternion.identity);
    }


    void SetSeed() {
        if (seedType == SeedType.RANDOM)
            random = new System.Random();
        else if (seedType == SeedType.CUSTOM)
            random = new System.Random(seed);
    }

    void CreatePath() {
        gridCellList.Clear();
        Vector2 currentPosition = new Vector2(-15.0f, -9.0f);

        //we need a variable to store the last position
        Vector2 lastPosition = currentPosition;
        gridCellList.Add(new GridCell(currentPosition));

        for (int i = 0; i < pathLength; i++) {

            int n = random.Next(100);

            if (n > 0 && n < 49) {
                currentPosition = new Vector2(currentPosition.x + cellSize, currentPosition.y);
            }
            else {
                currentPosition = new Vector2(currentPosition.x, currentPosition.y + cellSize);
            }

            //We need a function to calculate the room to place in the previous position
            CalculateRoom(lastPosition, currentPosition);

            Instantiate(roomPrefab, lastPosition, Quaternion.identity); //We are creating the room behind the currentOne
            gridCellList.Add(new GridCell(currentPosition));

            lastPosition = currentPosition;
        }

        //We need to make the last room in the sequence
        if (lastDirection == "right") //Last Move was right
        {
            roomPrefab = roomTypes[2]; //Room_R
        }
        else if (lastDirection == "up") //Last move was up
        {
            roomPrefab = roomTypes[0]; //Room_R
        }

        Instantiate(roomPrefab, lastPosition, Quaternion.identity); //We are creating the room behind the currentOne
    }

    void CalculateRoom(Vector2 roomPosition, Vector2 currentPosition) //The roomPos is where we are creating the room, while the next is the current Position
    {
        //We need to see what direction the room has moved
        //Index for rooms
        //0  Room_B     //4 Room_R
        //1  Room_BR    //5 Room_T
        //2  Room_L     //6 Room_TB
        //3  Room_LR    //7 Room_TL
                //8 Template Room
        Vector2 pathShift = new Vector2(currentPosition.x - roomPosition.x, currentPosition.y - roomPosition.y);

        //Lets detect where we've moved
        if (lastDirection == "null") //We are still at start
        {
            if (pathShift.x > 0) //Moved Right
            {
                roomPrefab = roomTypes[4]; //Room_R
                lastDirection = "right";
            }
            else if (pathShift.y > 0) //Moved Up
            {
                roomPrefab = roomTypes[5]; //Room_T
                lastDirection = "up";
            }
        }
        else if(lastDirection == "right") //We last moved right
        {
            if (pathShift.x > 0) //Next Moves Right
            {
                roomPrefab = roomTypes[3]; //Room_LR
                lastDirection = "right";
            }
            else if (pathShift.y > 0) //Next Moves Up
            {
                roomPrefab = roomTypes[7]; //Room_TL
                lastDirection = "up";
            }
        }
        else if(lastDirection == "up")
        {
            if (pathShift.x > 0) //Next Moves Right
            {
                roomPrefab = roomTypes[1]; //Room_BR
                lastDirection = "right";
            }
            else if (pathShift.y > 0) //Next Moves Up
            {
                roomPrefab = roomTypes[6]; //Room_TB
                lastDirection = "up";
            }
        }
        else
        {
            if (pathShift.x > 0)
            {
                Debug.Log("MOVED RIGHT");
            }
            else if (pathShift.y > 0)
            {
                Debug.Log("MOVED UP");
            }
            else
            {
                Debug.Log("WHAT");
            }
        }

    }

    IEnumerator CreatePathRoutine() {
        gridCellList.Clear();
        Vector2 currentPosition = new Vector2(-15.0f, -9.0f);

        gridCellList.Add(new GridCell(currentPosition));

        for (int i = 0; i < pathLength; i++) {

            int n = random.Next(100);

            if (n > 0 && n < 49) {
                currentPosition = new Vector2(currentPosition.x + cellSize, currentPosition.y);
            }
            else {
                currentPosition = new Vector2(currentPosition.x, currentPosition.y + cellSize);
            }

         
            gridCellList.Add(new GridCell(currentPosition));
            yield return null;
        }
    }


    private void OnDrawGizmos() {
        for (int i = 0; i < gridCellList.Count; i++) {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(gridCellList[i].location, Vector2.one * cellSize);
            Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            Gizmos.DrawCube(gridCellList[i].location, Vector2.one * cellSize);
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            //Instantiate(roomPrefab, gameObject.transform);
            SetSeed();
            if (animatedPath) {
                StartCoroutine(CreatePathRoutine());
            }
            else
                CreatePath();
        }
    }

}
