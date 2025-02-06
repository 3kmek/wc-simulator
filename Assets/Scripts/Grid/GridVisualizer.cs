using UnityEngine;

[ExecuteAlways] // Scene'de çalışırken de çizgi görebilmek için
public class GridVisualizer : MonoBehaviour
{
    public float cellSize = 5f;
    public int gridWidth = 10;
    public int gridHeight = 10;
    public Color gridColor = Color.white;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = gridColor;

        // X ekseninde dikey çizgiler
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 startPos = transform.position + new Vector3(x * cellSize, 0f, 0f);
            Vector3 endPos = startPos + new Vector3(0f, 0f, gridHeight * cellSize);
            Gizmos.DrawLine(startPos, endPos);
        }

        // Z ekseninde yatay çizgiler
        for (int z = 0; z <= gridHeight; z++)
        {
            Vector3 startPos = transform.position + new Vector3(0f, 0f, z * cellSize);
            Vector3 endPos = startPos + new Vector3(gridWidth * cellSize, 0f, 0f);
            Gizmos.DrawLine(startPos, endPos);
        }
    }
}