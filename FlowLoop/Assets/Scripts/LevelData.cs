using System.Collections.Generic;
using UnityEngine;

public class LevelData : ScriptableObject
{
    // levels are regular grids
    // total number of tiles = width x height
    public int width;
    public int height;

    // source node / end node
    public ((int, int), (int, int)) nodePair;
}
