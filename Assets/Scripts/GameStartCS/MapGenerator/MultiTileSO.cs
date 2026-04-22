using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "MulitiTileSO", menuName = "Creat/SO/MultiTileSO")]
public class MultiTileSO : ScriptableObject
{
    public int id;
    public int width;
    public int height;
    public float probability;
    public TileBase[] multiTiles;
    public TileHierarchy[] tileHierarchies;
    public int[] terrainType;
    public int minDistance;
}