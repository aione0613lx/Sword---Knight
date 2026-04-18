using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public Vector2Int startPos;
    public Vector2Int endPos;

    public Dictionary<Vector2Int,MultiTileSO> chunkHasMultiTile;

    public Chunk(Vector2Int startPos,Vector2Int endPos)
    {
        this.startPos = startPos;
        this.endPos = endPos;
        chunkHasMultiTile = new Dictionary<Vector2Int, MultiTileSO>();
    }
}
