using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

    public GameObject BottomRight;
    public int RowCount;
    public int ColumnCount;

    public Vector3 GetAnchor(int column, int row)
    {
        float xPos = Map(column, 0, ColumnCount, transform.position.x, BottomRight.transform.position.x);
        float yPos = Map(row, 0, RowCount, transform.position.y, BottomRight.transform.position.y);
        return new Vector3(xPos, yPos);
    }

    private static float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
}
