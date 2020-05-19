using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad()]
public class WordSearchGridEditor
{
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmo(WordSearchGrid1 grid, GizmoType gizmoType)
    {
        if(grid.columns > 0 && grid.rows > 0)
        {
            int cellsCount = grid.columns * grid.rows;
            Vector3 position = grid.gameObject.transform.position;
            for(int i = 0; i < grid.rows; i++)
            {
                for(int j = 0; j < grid.columns; j++)
                {
                    Vector3 drawPos = position + new Vector3(grid.cellSizeX * j, grid.cellSizeX * i, 0);
                    //Gizmos.DrawWireCube(new Vector3(grid.cellSizeX * j, grid.cellSizeX * i, 0), new Vector3(grid.cellSizeX, grid.cellSizeX));
                    Gizmos.DrawWireCube(drawPos, Vector3.one * (grid.cellSizeX));
                }
            }
        }
    }
}
