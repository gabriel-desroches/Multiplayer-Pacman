using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Transform player;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        //Hardcoded value for this level because custom radius
        gridSizeX = 28;
        gridSizeY = 26;
        createGrid();
    }

    private bool gridActive = false;
    private void Update()
    {
        if (Input.GetKeyDown("1")) gridActive = true;
        else if (Input.GetKeyDown("2")) gridActive = false;
    }

    void createGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - (Vector3.right * gridWorldSize.x / 2) - (Vector3.forward * gridWorldSize.y / 2) + new Vector3(0.0f, 1.0f, 0.0f);

        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (i * 1.0f + 0.5f) + Vector3.forward * (j * 1.0f + 0.5f); //Change here to make the nodes be 1 unit apart
                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);
                grid[i, j] = new Node(walkable, worldPoint, i, j);
            }
        }

    }
    
    /*
     * For Pac-Man: Neighbours are only on top and on sides,
     * so algorithm is adjusted accordingly
     * I wasn't sure the best way to do this actually, so I just continue;
     * when abs x == abs y
     */
    public List<Node> getNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x * x == y * y) continue; //Removes corner neighbours and this node

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    //convert the player's position to correspond to a node
    public Node getNodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }


    public List<Node> path;
    public List<Node> openSet;
    public HashSet<Node> closedSet;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if(grid != null && gridActive)
        {
            Node playerNode = getNodeFromWorldPoint(player.position);
            foreach(Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red; //walkable is white, unwalkable is red

                if (path != null)
                {
                    if (openSet.Contains(n)) Gizmos.color = Color.green;
                    if (closedSet.Contains(n)) Gizmos.color = Color.yellow;
                    if (path.Contains(n)) Gizmos.color = Color.blue; //hlighlights path in green
                }
                //if (playerNode == n) Gizmos.color = Color.cyan; //show player's current node
                Gizmos.DrawSphere(n.worldPosition, nodeRadius);
            }
        }

    }


}
