using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourcePointController : MonoBehaviour
{
    public GameObject linePrefab;
    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;

    private GameObject[] tiles;
    private GameObject endPoint;

    // used for continuous line drawing
    //private float minDistBetweenPoints = 0.2f;
    private GameObject line;
    private List<Vector2> linePoints;

    private bool isDrawingLine;
    private bool isLevelCompleted;
    private GameObject prevTile;

    private GameObject levelManager;

    public GameObject idleParticle;
    public GameObject drawingParticle;
    public GameObject failParticle;
    public GameObject endingParticle;

    void Start()
    {
        isDrawingLine = false;
        isLevelCompleted = false;
        prevTile = null;
        linePoints = new List<Vector2>();
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        endPoint = GameObject.FindGameObjectWithTag("EndPoint");
        levelManager = GameObject.FindGameObjectWithTag("LevelManager");

        idleParticle.SetActive(true);
        drawingParticle.SetActive(false);
        failParticle.SetActive(false);
        endingParticle.SetActive(false);
        endingParticle.transform.position = endPoint.transform.position;
    }

    void Update()
    {
        if (isDrawingLine && !isLevelCompleted)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            UpdateLine(mousePos);
        }
    }

    void OnMouseDown()
    { 
        if (!isLevelCompleted)
        {
            isDrawingLine = true;
            drawingParticle.SetActive(true);
            CreateLine();

            // Doesn't really work, it's a fixed duration vibrate (~ 1 sec)
            //Handheld.Vibrate();
        }   
    }

    void OnMouseUp()
    {
        if (!isLevelCompleted && isDrawingLine)
        {
            isDrawingLine = false;
            DestroyLine();
        }
    }

    void CreateLine()
    {
        line = Instantiate(linePrefab, Vector2.zero, Quaternion.identity);
        lineRenderer = line.GetComponent<LineRenderer>();
        edgeCollider = line.GetComponent<EdgeCollider2D>();

        linePoints.Clear();
        linePoints.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        linePoints.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        lineRenderer.SetPosition(0, linePoints[0]);
        lineRenderer.SetPosition(1, linePoints[1]);

        edgeCollider.points = linePoints.ToArray();
    }

    void UpdateLine(Vector2 newPos)
    {
        drawingParticle.transform.position = newPos;

        // if colliding with endpoint
        if (endPoint.GetComponent<BoxCollider2D>().bounds.Contains(newPos))
        {
            foreach(GameObject tile in tiles)
            {
                TileController tileController = tile.GetComponent<TileController>();

                // if tried to connect to endpoint and not all tiles are occupied, reset
                if (!tileController.IsOccupied())
                {
                    OnMouseUp();
                    return;
                }
            }

            // else, level is complete
            AddNewPoint(endPoint.transform.position);

            // set flags and call "event"
            isLevelCompleted = true;
            isDrawingLine = false;
            levelManager.GetComponent<LevelManagerController>().CompleteLevel();

            // add particle flourish
            drawingParticle.SetActive(false);
            endingParticle.SetActive(true);
            //idleParticle.SetActive(false);

            return;
        }

        // if colliding with a tile and the tile doesn't have the line crossing it, add a point
        foreach(GameObject tile in tiles)
        {
            if (tile.GetComponent<BoxCollider2D>().bounds.Contains(newPos))
            {
                TileController tileController = tile.GetComponent<TileController>();
                if (!tileController.IsOccupied())
                {
                    prevTile = tile;
                    tileController.SetOccupied(true);

                    AddNewPoint(tile.transform.position);
                    return;
                }
                else if(!Object.ReferenceEquals(prevTile, tile))
                {
                    OnMouseUp();
                    return;
                }
            }
        }

        // i a continuous line is desired, enable this bit of code
        /** /
        if(Vector2.Distance(linePoints[linePoints.Count - 2], newPos) >= minDistBetweenPoints)
        {
            AddNewPoint(newPos);
        }
        /**/
        // else just update the last point's position
        linePoints[linePoints.Count - 1] = newPos;
        lineRenderer.SetPosition(linePoints.Count - 1, newPos);
        edgeCollider.points = linePoints.ToArray();
    }

    void DestroyLine()
    {
        failParticle.transform.position = linePoints[linePoints.Count - 1];
        // force reset the particle system
        failParticle.SetActive(false);
        failParticle.SetActive(true);

        Destroy(line);
        foreach (GameObject tile in tiles)
        {
            tile.GetComponent<TileController>().SetOccupied(false);
        }

        drawingParticle.transform.position = transform.position;
        drawingParticle.SetActive(false);
    }

    void AddNewPoint(Vector2 newPos)
    {
        linePoints.Add(newPos);
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newPos);
        edgeCollider.points = linePoints.ToArray();
    }
}
