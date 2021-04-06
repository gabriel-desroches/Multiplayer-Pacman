using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Gabriel Desroches 40099529 hi
//Assignment 2 COMP 476 Advanced Game Development

//POV uses the Node GameObjects while Tile Graph uses the Node class. In hingsight, I should
//have made them use the same thing, but you would never have both of these implementations at the same time anyway.

//I should have made two scenes.

//The code was made to accomodate for many requests for pathfinding at the same time, this isn't necessary here but I 
//included it because I was going to need it for a project.

//Tile graph was completed with help from Sabastian Lague's tutorials on Youtube https://www.youtube.com/watch?v=-L-WgKMFuhE

public class PathFinding : MonoBehaviour
{
    Grid grid;

    PathRequestManager pathRequestManager;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        pathRequestManager = GetComponent<PathRequestManager>();
    }

    public void startFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(findPath(startPos, targetPos));   
    }


    private void Update()
    {
        //if (tilePathFinding) findPath(seeker.position, target.position);
        
    }

    IEnumerator findPath(Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.getNodeFromWorldPoint(startPos);
        Node targetNode = grid.getNodeFromWorldPoint(targetPos);

        if (startNode.walkable && targetNode.walkable)
        {
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            //loop
            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost ||
                       openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    grid.openSet = openSet;
                    grid.closedSet = closedSet;
                    break;
                }

                foreach (Node neighbour in grid.getNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue; //skip if unavaible or already seen

                    int newMovementCostToNeighbour = currentNode.gCost + getDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour; //cost of movement here
                        neighbour.hCost = getDistance(neighbour, targetNode); //distance from target
                        neighbour.Parent = currentNode;
                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
        }
        yield return null;
        if(pathSuccess)
        { 
            waypoints = retracePath(startNode, targetNode);
        }
        pathRequestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    //works backwards from target to build path
    Vector3[] retracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse(); //put it the right way
        grid.path = path; //send to display

        List<Vector3> wayPoints = new List<Vector3>();

        for (int i = 0; i < path.Count; i++) wayPoints.Add(path[i].worldPosition);
        //wayPoints.Reverse(); //optimize

        return wayPoints.ToArray();

    }

    int getDistance(Node nodeA, Node nodeB)
    {
        //with help of http://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html
            int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
            return Mathf.RoundToInt(10 * Mathf.Sqrt(distX * distX + distY * distY));
    }
}

