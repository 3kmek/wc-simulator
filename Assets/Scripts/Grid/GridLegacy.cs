using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.UI;

public class GridLegacy<TGridObject>
{
   private int width;
   private int height;
   private float cellSize;
   private Vector3 originPosition;
   private TGridObject[,] gridArray;
   private TextMesh[,] debugTextArray;
   
   
   public GridLegacy(int width, int height, float cellSize, Vector3 originPosition)
   {
      this.width = width;
      this.height = height;
      this.cellSize = cellSize;
      this.originPosition = originPosition;
      
      gridArray = new TGridObject[width, height];
      debugTextArray = new TextMesh[width, height];

      bool showDebug = true;
      if(showDebug)
      {
         for (int x = 0; x < gridArray.GetLength(0); x++)
         {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {

               Debug.Log(gridArray);

               debugTextArray[x, z] = UtilsClass.CreateWorldText(gridArray[x, z].ToString(), null,
                  GetWorldPosition(x, z) + new Vector3(cellSize, cellSize, 0) * .5f,
                  20, Color.white, TextAnchor.MiddleCenter);
               Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.cyan, 100f);
               Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.cyan, 100f);
            }
         }

         Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.cyan, 100f);
         Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.cyan, 100f);
      }
      
      
   }

   private Vector3 GetWorldPosition(int x, int z)
   {
      return new Vector3(x, 0, z) * cellSize + originPosition;
   }

   private void GetXZ(Vector3 worldPosition, out int x, out int z)
   {
      x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
      z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
   }

   public void SetValue(int x, int z, TGridObject value)
   {
      if (x < 0 || x <= width || z < 0 || z <= height)
      {
         gridArray[x, z] = value;
         debugTextArray[x, z].text = gridArray[x, z].ToString();
      }
      
   }

   public void SetValue(Vector3 worldPosition, TGridObject value)
   {
      int x, z;
      GetXZ(worldPosition, out x, out z);
      SetValue(x, z, value);
   }

   public TGridObject GetValue(int x, int z)
   {
      if (x < 0 || x <= width || z < 0 || z <= height)
      {
         return gridArray[x, z];
      }

      else
      {
         return default(TGridObject);
      }
   }

   public TGridObject GetValue(Vector3 worldPosition)
   {
      int x, z;
      GetXZ(worldPosition, out x, out z);
      return GetValue(x, z);
   }
   
    
    
}
