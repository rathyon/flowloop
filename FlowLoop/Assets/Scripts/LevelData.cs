using System.Collections.Generic;
using UnityEngine;

public class LevelData : ScriptableObject
{
    public int width;
    public int height;

    public ((int, int), (int, int)) nodePair;
}
