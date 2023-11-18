using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar
{
    Node[,] nodes;

    public List<Node> CreatePath(Vector3Int[,] grid, Vector2Int vStart, Vector2Int vEnd)
    {
        Node nStart = null;
        Node nEnd = null;

        int gridY = grid.GetUpperBound(0) + 1;
        int gridX = grid.GetUpperBound(1) + 1;

        //��� ���� �ʱ�ȭ
        nodes = new Node[gridY, gridX];
        for(int i = 0; i < gridY; i++)
        {
            for(int j = 0; j < gridX; j++)
            {
                //Ư�� �ε����� �ش��ϴ� grid�� ������ ��忡 �ʱ�ȭ
                nodes[i, j] = new Node(grid[i, j].x, grid[i, j].y, grid[i, j].z < 1 ? true : false);
            }
        }

        for (int i = 0; i < gridY; i++)
        {
            for (int j = 0; j < gridX; j++)
            {
                //����� ������� ������ �߰�
                nodes[i, j].AddAdjacents(nodes, i, j);

                //���Ϳ� �ش��ϴ� ���۰� �� ��带 Ž��
                if (nodes[i, j].x == vStart.x && nodes[i, j].y == vStart.y)
                    nStart = nodes[i, j];
                else if (nodes[i, j].x == vEnd.x && nodes[i, j].y == vEnd.y)
                    nEnd = nodes[i, j];
            }
        }

        //���۰� ���� �����ϴ��� �˻�
        if (nStart == null || nEnd == null || !nEnd.isAccessible)
            return null;

        List<Node> openList = new List<Node>();
        List<Node> closeList = new List<Node>();
        openList.Add(nStart);

        while(openList.Count > 0)
        {
            /*
            A* �˰���
            1. openList���� ���� ����� ���� ��带 ���� (���� �� ��带 ���� ����� Ī�Ѵ�)
            2. ���� ��尡 ���� ������ ��� ��� ���� �� ��ȯ
            3. �ƴ� ��� ���� ��带 clostList�� �߰� �� openList���� ���ó�带 ���� 
            4. ���� ����� ���� ����� f,g,h�� ���
            5. ���� ��尡 closeList�� ���� ��� openList�� �߰�
            6. 2�� ���� Ȥ�� openList�� ���� ���� ������ 1~5�� �ݺ�
            */

            //1. openList���� ���� ����� ���� ��带 ���� (����� ���� ��� h�� �� ���� ���� ����)
            int selectedIndex = 0;
            for(int i = 0; i< openList.Count;i++)
            {
                if (openList[i].f < openList[selectedIndex].f)
                    selectedIndex = i;
                else if (openList[i].f == openList[selectedIndex].f)
                {
                    if (openList[i].h < openList[selectedIndex].h)
                        selectedIndex = i;
                }
            }

            //2. ���� ��尡 ���� ������ ��� ��� ���� �� ��ȯ
            var currentNode = openList[selectedIndex];
            if(currentNode == nEnd)
            {
                List<Node> path = new List<Node>();
                
                var tempNode = currentNode;

                path.Add(tempNode);

                while (tempNode.parentNode != null)
                {
                    path.Add(tempNode.parentNode);
                    tempNode = tempNode.parentNode;
                }

                path.Reverse();
                return path;
            }

            //3. �ƴ� ��� ���� ��带 clostList�� �߰� �� openList���� ���ó�带 ���� 
            closeList.Add(currentNode);
            openList.Remove(currentNode);

            var adjacentList = currentNode.adjacentNodes;
            for(int i = 0;i<adjacentList.Count;i++)
            {
                var selectedAdjacent = adjacentList[i];

                //���� ��忡 ������ �������� üũ
                if (!closeList.Contains(selectedAdjacent) && selectedAdjacent.isAccessible)
                {
                    if (openList.Contains(selectedAdjacent))
                        continue;

                    else
                    {
                        selectedAdjacent.g = 10;
                        selectedAdjacent.h = Heuristic(selectedAdjacent, nEnd);
                        selectedAdjacent.f = selectedAdjacent.g + selectedAdjacent.h;
                        selectedAdjacent.parentNode = currentNode;
                        openList.Add(selectedAdjacent);
                    }              
                }
            }
        }

        return null;
    }

    private int Heuristic(Node a, Node b)
    {
        int x = Mathf.Abs(a.x - b.x);
        int y = Mathf.Abs(a.y - b.y);

        return x + y;
    }

}
public class Node
{
    public int x { get; private set; }
    public int y { get; private set; }

    public int h;
    public int g;
    public int f;

    public bool isAccessible { get; private set; }

    public List<Node> adjacentNodes;
    public Node parentNode = null;

    public Node(int _x, int _y, bool _isAccessible)
    {
        this.x = _x;
        this.y = _y;
        this.h = 0;
        this.g = 0;
        this.f = 0;
        this.isAccessible = _isAccessible;
        adjacentNodes = new List<Node>();
    }
    public void AddAdjacents(Node[,] nodes, int x, int y)
    {
        /*
        AddAdjacents
        1. �����¿쿡 �����ϴ� ������带 ����Ʈ�� �ִ´�
        2. ���� ���� ��쿡�� ���� �ʴ´�
        */

        if (x < nodes.GetUpperBound(0))
            adjacentNodes.Add(nodes[x + 1, y]);

        if (x > 0)
            adjacentNodes.Add(nodes[x - 1, y]);

        if (y < nodes.GetUpperBound(1))
            adjacentNodes.Add(nodes[x, y + 1]);

        if (y > 0)
            adjacentNodes.Add(nodes[x, y - 1]);
    }
}