﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*** NOTE ***
// Everything that talks about circles/test circles in this project is simply just for the visual representation of the positions for the grid
//aka, will be deleted further on, and nothing to worry about


public class GridCreate : MonoBehaviour
{
    // Start is called before the first frame update

    public int rows = 6, columns = 5; //can be changed according to design of grid sprite
    public float gridSpread;
    [HideInInspector] public float width, height;
    [HideInInspector] public float boxWidth, boxHeight, midCol;
    //private GameObject redCircle;
    private GameObject grid;
    private Vector3 position;
    private Bounds size;
    private List<Vector3> positions = new List<Vector3>(); //creates a list of positions


    void Start()
    {
        grid = gameObject;
        //redCircle = GameObject.Find("RedCircle");
        position = grid.transform.position;
        size = grid.GetComponent<Renderer>().bounds;

        //This chunk is only for displaying numbers, not important for actual code
        /*
        Debug.Log(position);
        Debug.Log(size);
        Debug.Log(size.extents.x); //distance to right side from center
        Debug.Log(-size.extents.x);//distance to left side from center
        Debug.Log(size.extents.y);//distance to top from center
        Debug.Log(-size.extents.y);//distance to bottom side from center
        */
        //Back to important stuff: calculating width and height of the grid, plus the width and height of each individual square

        width = 2 * (size.extents.x);
        height = 2 * (size.extents.y);
        midCol = ((float)columns) / 2f;
        boxWidth = width / columns;
        boxHeight = height / rows;


        //Once again, this chunk is only for displaying numbers, not important for actual code

        /*Debug.Log("width:" + width);
        Debug.Log("height:" + height);
        Debug.Log("boxWidth:" + boxWidth);
        Debug.Log("boxHeight:" + boxHeight);
        */

        //Simple for loop to iterate through columns and rows. Kinda inefficient, but I can't think of any other implementation

        for (int i = 0; i < rows; i++){
            Vector3 adjustedPos = position - new Vector3((width / 2) - (boxWidth / 2), (-height / 2) + (boxHeight / 2), 0); //basically starts the positions at top left corner, rather than the center
            //Debug.Log("row" + i);
            int j;
            for(j = 0; j < columns; j++)
            {
                Vector3 gridPos;
                float centerDif = j - midCol;
                float spreadAdjust = gridSpread * i * centerDif;
                gridPos = adjustedPos + new Vector3(j * boxWidth + (spreadAdjust*boxWidth), -i * boxHeight, 0); //determines the actual position  where the unit would be placed
                //adds the center position (gridPos) into a list. Top Left is first, bottom right is last
                //Instantiate(redCircle, gridPos, redCircle.transform.rotation);
                positions.Add(gridPos);
            }
         j = 0;
        }

    }
    public List<Vector3> getPositions()
    {
        return positions;
    }
}
