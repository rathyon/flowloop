using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
 * This class generates a LevelData ScriptableObject that is then read at runtime by the game.
 * Unfortunately the generator is unfinished so at this moment it is essentially useless, but
 * it is capable of creating and saving a ScriptableObject and can be called in the Editor in
 * its own menu item: "Level Generation".
 */

public class LevelDataGenerator
{
    [MenuItem("Level Generation/Generate Level Data")]
    static void Init()
    {
        // Generates a 4x4 level
        LevelData data = ScriptableObject.CreateInstance<LevelData>();
        Level testLevel = GenerateLevel(4, 4, 1);
        data.width = testLevel.width;
        data.height = testLevel.height;
        data.nodePair = testLevel.nodePair;

        AssetDatabase.CreateAsset(data, "Assets/Levels/Test4x4LevelData.asset");
    }

    struct Node
    {
        // if the next or prev nodes are the node itself, then it means it is an endpoint (head or tail)
        public (int,int) prev;
        public (int,int) next;
    }

    struct Level
    {
        public int width;
        public int height;
        public ((int, int), (int, int)) nodePair;
    }

    static Level GenerateLevel(int rows, int columns, int iterations)
    {
        Level level = new Level();
        level.width = columns;
        level.height = rows;

        // a level must have at least 2 rows or 2 columns
        if(rows <= 1 || columns <= 1)
        {
            //TODO: throw exception
            return level;
        }

        // Initialize the grid with a zig zag pattern path, starting from left to right

        /*
         * 1->2->3->4
         *          |
         * 8<-7<-6<-5
         * |
         * 9->...
         * 
         */

        Node[,] grid = new Node[rows, columns];
        for(int r=0; r < rows; r++)
        {
            Node newNode = new Node();

            // if on an even row (0, 2, 4, etc) we go from left to right
            if (r % 2 == 0)
            {
                for (int c = 0; c < columns; c++)
                {
                    // if start point
                    if (r == 0 && c == 0)
                    {
                        newNode.prev = (0, 0);
                        newNode.next = (0, 1);
                    }
                    // if end point
                    else if (r == rows-1 && c == columns-1)
                    {
                        newNode.prev = (r, c-1);
                        newNode.next = (r, c);
                    }
                    else
                    {
                        if(c == 0)
                        {
                            newNode.prev = (r-1, c);
                        }
                        else
                        {
                            newNode.prev = (r, c-1);
                        }
                        
                        if(c == columns-1)
                        {
                            newNode.next = (r+1, c);
                        }
                        else
                        {
                            newNode.next = (r, c+1);
                        }
                        
                    }
                    grid[r, c] = newNode;
                }
            }
            // else we go from right to left
            else
            {
                for(int c = columns-1; c > -1; c--)
                {
                    // if end point
                    if(r == rows-1 && c == 0)
                    {
                        newNode.prev = (r, c+1);
                        newNode.next = (r, c);
                    }
                    else
                    {
                        if(c == columns - 1)
                        {
                            newNode.prev = (r-1, c);
                        }
                        else
                        {
                            newNode.prev = (r, c+1);
                        }

                        if(c == 0)
                        {
                            newNode.next = (r+1, c);
                        }
                        else
                        {
                            newNode.next = (r, c-1);
                        }
                        
                    }
                    grid[r, c] = newNode;
                }
            }
        }

        // grid has now been filled with a baseline Hamiltonian Path
        // now we jumble it up a bit to create a different Hamiltonian Path
        // Algorithm source: https://datagenetics.com/blog/december22018/index.html

        (int, int) head = (0,0);
        (int, int) tail = (rows - 1, columns - 1);

        for(int iter=0; iter < iterations; iter++)
        {
            (int, int) endpoint;
            // select one of the two endpoints at random
            if (Random.Range(0, 2) == 0)
            {
                endpoint = head;
            }
            else
            {
                endpoint = tail;
            }

            Node node = grid[endpoint.Item1, endpoint.Item2];

            // create a list of all possible neighbors
            List<(int, int)> possibleNeighbors = new List<(int, int)>();

            // left
            if (endpoint.Item2 > 0)
            {
                (int, int) neighbor = endpoint;
                neighbor.Item2 -= 1;
                possibleNeighbors.Add(neighbor);
            }
            // right
            if (endpoint.Item2 < columns - 1)
            {
                (int, int) neighbor = endpoint;
                neighbor.Item2 += 1;
                possibleNeighbors.Add(neighbor);
            }
            // up
            if (endpoint.Item1 > 0)
            {
                (int, int) neighbor = endpoint;
                neighbor.Item1 -= 1;
                possibleNeighbors.Add(neighbor);
            }
            // down
            if (endpoint.Item1 < rows - 1)
            {
                (int, int) neighbor = endpoint;
                neighbor.Item1 += 1;
                possibleNeighbors.Add(neighbor);
            }

            // create list with neighbors that aren't in path
            List<(int, int)> neighbors = new List<(int, int)>();
            foreach ((int, int) neighbor in possibleNeighbors)
            {
                if (neighbor != node.prev || neighbor != node.next)
                {
                    neighbors.Add(neighbor);
                }
            }

            // randomly choose one of the neighbors
            int idx = Random.Range(0, neighbors.Count);
            (int, int) loopPoint = neighbors[idx];

            // if its the end of the path
            if (node.next == endpoint)
            {
                grid[endpoint.Item1, endpoint.Item2].next = loopPoint;
            }
            // else its the start of the path
            else
            {
                grid[endpoint.Item1, endpoint.Item2].prev = loopPoint;
            }

            // check both newly created paths to see if which one loops back around to loopPoint
            bool endOfPath = false;
            bool isLoop = false;
            (int, int) auxNode = grid[loopPoint.Item1, loopPoint.Item2].next;
            while (!endOfPath)
            {
                //Debug.Log("AuxNode: " + auxNode.Item1 + "," + auxNode.Item2);
                // (looped back around)
                if (auxNode == loopPoint)
                {
                    isLoop = true;
                    endOfPath = true;
                }
                // dead end (the other endpoint of the path)
                else if (auxNode == grid[auxNode.Item1, auxNode.Item2].next)
                {
                    isLoop = false;
                    endOfPath = true;
                }
                else
                {
                    auxNode = grid[auxNode.Item1, auxNode.Item2].next;
                }
            }

            // if isLoop, then we "cut" the "next" of loopPoint
            if (isLoop)
            {
                tail = grid[loopPoint.Item1, loopPoint.Item2].next;
                grid[loopPoint.Item1, loopPoint.Item2].next = endpoint;

                // we now have a segment of the path that runs in the opposite direction, so we have to reverse each node's direction one by one
                (int, int) nodeToFix = endpoint;
                bool pathFixed = false;
                while (!pathFixed)
                {
                    (int, int) oldNext = grid[nodeToFix.Item1, nodeToFix.Item2].next;
                    grid[nodeToFix.Item1, nodeToFix.Item2].next = grid[nodeToFix.Item1, nodeToFix.Item2].prev;
                    grid[nodeToFix.Item1, nodeToFix.Item2].prev = oldNext;

                    if (grid[nodeToFix.Item1, nodeToFix.Item2].next == loopPoint && nodeToFix != endpoint)
                    {
                        grid[nodeToFix.Item1, nodeToFix.Item2].next = nodeToFix;
                        pathFixed = true;
                    }
                    else
                    {
                        nodeToFix = grid[nodeToFix.Item1, nodeToFix.Item2].next;
                    }
                }

            }
            // else we "cut" the "prev" of loopPoint
            else
            {
                head = tail;
                tail = grid[loopPoint.Item1, loopPoint.Item2].prev;
                grid[tail.Item1, tail.Item2].next = tail;

                (int, int) nodeToFix = grid[loopPoint.Item1, loopPoint.Item2].next;
                bool pathFixed = false;
                while (!pathFixed)
                {
                    (int, int) oldNext = grid[nodeToFix.Item1, nodeToFix.Item2].next;
                    grid[nodeToFix.Item1, nodeToFix.Item2].next = grid[nodeToFix.Item1, nodeToFix.Item2].prev;
                    grid[nodeToFix.Item1, nodeToFix.Item2].prev = oldNext;

                    nodeToFix = grid[nodeToFix.Item1, nodeToFix.Item2].prev;
                    if (nodeToFix == head)
                    {
                        grid[nodeToFix.Item1, nodeToFix.Item2].next = grid[nodeToFix.Item1, nodeToFix.Item2].prev;
                        grid[nodeToFix.Item1, nodeToFix.Item2].prev = nodeToFix;
                        pathFixed = true;
                    }
                }
                grid[loopPoint.Item1, loopPoint.Item2].prev = grid[loopPoint.Item1, loopPoint.Item2].next;
                grid[loopPoint.Item1, loopPoint.Item2].next = endpoint;
            }

            // Grid now contains a different Hamiltonian path!
        }

        /** /
        string debugString = "Grid: ";
        for (int r = 0; r < rows; r++)
        {
            debugString += "\n";
            for (int c = 0; c < columns; c++)
            {
                debugString += "[" + r + "," + c + "]" + "Prev: " + grid[r, c].prev + " Next: " + grid[r, c].next + "\n";
            }
        }
        Debug.Log(debugString);
        /**/

        return level;
    }
}
