using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This class handles each tile's state.
 */
public class TileController : MonoBehaviour
{
    public GameObject occupiedSquare;

    private bool isOccupied;
    void Start()
    {
        occupiedSquare.SetActive(false);
        isOccupied = false;
    }

    public bool IsOccupied()
    {
        return isOccupied;
    }

    public void SetOccupied(bool val)
    {
        isOccupied = val;

        if (isOccupied)
        {
            occupiedSquare.SetActive(true);
        }
        else
        {
            occupiedSquare.SetActive(false);
        }
    }
}
