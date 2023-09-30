using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGraph
{
    public int MapDepth { get; set; }
    public int MaxNodesPerLine { get; set; }
    public List<List<MapNode>> NodeGroups { get; private set; }

    public MapGraph(int mapDepth, int maxNodesPerLine)
    {
        MapDepth = mapDepth;
        MaxNodesPerLine = maxNodesPerLine;
        NodeGroups = new List<List<MapNode>>();
    }

    public MapNode GetNodeById(int id)
    {
        foreach (var nodeGroup in NodeGroups)
        {
            foreach (var node in nodeGroup)
            {
                if (node.Id == id)
                    return node;
            }
        }

        return null;
    }

    public void AddNodesGroup(List<MapNode> _nodesGroup)
    {
        NodeGroups.Add(_nodesGroup);
    }

    public void CreateRandomGraph()
    {
        int prevSize = 0;
        int id = 0;
        // Create node groups
        for (int i = 0; i < MapDepth; i++)
        {
            var tempGroup = new List<MapNode>();

            // Check for root or final node
            if (i == 0 || i == (MapDepth - 1))
            {
                MapNode mapNode = null;
                if (i == 0)
                    mapNode = new MapNode(NodeType.Begin);
                else if (i == (MapDepth - 1))
                    mapNode = new MapNode(NodeType.Final);

                mapNode.Id = id;
                tempGroup.Add(mapNode);
                AddNodesGroup(tempGroup);

                id++;
                continue;
            }

            // Select random number of nodes per vertical line (group)
            // In the second group or the one before final (i == 1 or i == mapDepth-2), map can't have a single node
            int randGroupSize = 0;
            if (i == 1 || i == (MapDepth - 1))
                randGroupSize = Random.Range(2, MaxNodesPerLine + 1);
            else
                randGroupSize = Random.Range(1, MaxNodesPerLine + 1);

            // Check for avoiding same number of nodes per neighboring groups
            while (randGroupSize == prevSize)
            {
                randGroupSize = Random.Range(1, MaxNodesPerLine + 1);
            }

            // Initialize empty nodes in the node groups
            int randNodeType = 0;
            for (int j = 0; j < randGroupSize; j++)
            {
                randNodeType = Random.Range(0, 2);
                MapNode mapNode = randNodeType == 0 ? new MapNode(NodeType.Attack) : new MapNode(NodeType.BoostHub);
                mapNode.Id = id;
                tempGroup.Add(mapNode);
                id++;
            }

            AddNodesGroup(tempGroup);
            prevSize = randGroupSize;
        }

        // Place BoostHub Nodes
        /*int nodesAfter = 0; // number that indicates the number of nodes to be passed before placing new ones, can be 2 or 3
        for (int i = 2; i < NodeGroups.Count - 1; i++)
        {
            // The node group before the final group must always contains BoostHub nodes
            if (i == NodeGroups.Count - 2)
            {
                if (NodeGroups[i].Count == 1)
                    NodeGroups[i][0].NodeType = NodeType.BoostHub;

                // All nodes, except one, at this particular group must be BoostHubs
                int nodeToBeExcluded = Random.Range(0, NodeGroups[i].Count);
                for (int j = 0; j < NodeGroups[i].Count; j++)
                {
                    if (j == nodeToBeExcluded)
                        continue;

                    NodeGroups[i][j].NodeType = NodeType.BoostHub;
                }
            }

            // Decide the number of BoostHub nodes in the chosen group
            int numOfBHNodesToBePlaced = 0;
            if (NodeGroups[i].Count == 2)
                numOfBHNodesToBePlaced = 1;
            else if (NodeGroups[i].Count == 3 || NodeGroups[i].Count == 4)
                numOfBHNodesToBePlaced = 2;

            // Decide the indexes of the nodes that wont't become BoostHubs in a group
            HashSet<int> indexes = new HashSet<int>();
            while (indexes.Count < numOfBHNodesToBePlaced)
            {
                int num = Random.Range(0, NodeGroups[i].Count);
                indexes.Add(num);
            }

            // Place BoostHubNodes
            for (int j = 0; j < NodeGroups[i].Count; j++)
            {
                if (indexes.Contains(j))
                    continue;

                NodeGroups[i][j].NodeType = NodeType.BoostHub;
            }

            nodesAfter = Random.Range(2, 3);
            i += nodesAfter; // 2 or 3 nodes after to place the
        }*/

        // Connect Nodes between Node Groups
        // Iterate node groups
        for (int i = 0; i < MapDepth; i++)
        {
            // Check for the root node
            // Connect the root node with every node in the 2nd group
            if (i == 0)
            {
                var rootNode = NodeGroups[0][0];
                foreach (var node in NodeGroups[1])
                {
                    rootNode.ConnectedNodes.Add(node.Id);
                }
                continue;
            }

            // Check for the destination node
            // Connect the last node for every node the previous group
            if (i == (MapDepth - 2))
            {
                var lastNode = NodeGroups[i + 1][0];
                foreach (var node in NodeGroups[i])
                {
                    node.ConnectedNodes.Add(lastNode.Id);
                }
                break;
            }

            var currNodeGroup = NodeGroups[i];
            var nextNodeGroup = NodeGroups[i + 1];

            // When the next group has only one node, it must be connected with every node on the current group
            if (currNodeGroup.Count == 1)
            {
                foreach (var nextNode in nextNodeGroup)
                {
                    currNodeGroup[0].ConnectedNodes.Add(nextNode.Id);
                }
                continue;
            }

            if (nextNodeGroup.Count == 1)
            {
                foreach (var currNode in currNodeGroup)
                {
                    currNode.ConnectedNodes.Add(nextNodeGroup[0].Id);
                }
                continue;
            }

            int randNum = 0;
            if (currNodeGroup.Count == 2)
            {
                // Always connect the two upper edge nodes 
                currNodeGroup[0].ConnectedNodes.Add(nextNodeGroup[0].Id);
                // Always connect the two bottom edge nodes 
                currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[nextNodeGroup.Count - 1].Id);

                if (nextNodeGroup.Count == 3)
                {
                    randNum = Random.Range(0, 3); // 0 or 1 or 2
                    if (randNum == 2)
                    {
                        currNodeGroup[0].ConnectedNodes.Add(nextNodeGroup[1].Id);
                        currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[1].Id);
                        continue;
                    }
                    currNodeGroup[randNum].ConnectedNodes.Add(nextNodeGroup[1].Id);
                }

                if (nextNodeGroup.Count == 4)
                {
                    for (int j = 0; j < currNodeGroup.Count; j++)
                    {
                        // Node with index 0 in the current group
                        randNum = Random.Range(1, 4); // 1 or 2 or 3
                        if (randNum == 3)
                        {
                            currNodeGroup[j].ConnectedNodes.Add(nextNodeGroup[1].Id);
                            currNodeGroup[j].ConnectedNodes.Add(nextNodeGroup[2].Id);
                            continue;
                        }
                        currNodeGroup[j].ConnectedNodes.Add(nextNodeGroup[randNum].Id);
                    }
                }
            }

            if (currNodeGroup.Count == 3)
            {
                // Always connect the two upper edge nodes 
                currNodeGroup[0].ConnectedNodes.Add(nextNodeGroup[0].Id);
                // Always connect the two bottom edge nodes 
                currNodeGroup[2].ConnectedNodes.Add(nextNodeGroup[nextNodeGroup.Count - 1].Id);

                if (nextNodeGroup.Count == 2)
                {
                    randNum = Random.Range(0, 3); // 0 or 1 or 2
                    if (randNum == 2)
                    {
                        currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[0].Id);
                        currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[1].Id);
                        continue;
                    }
                    currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[randNum].Id);
                }

                if (nextNodeGroup.Count == 4)
                {
                    bool isNodeWithIndex1Connected = false;
                    bool isNodeWithIndex2Connected = false;
                    // For node with index 0 in the current group
                    randNum = Random.Range(0, 2); // 0 or 1
                    if (randNum == 1)
                    {
                        currNodeGroup[0].ConnectedNodes.Add(nextNodeGroup[1].Id);
                        isNodeWithIndex1Connected = true;
                    }

                    // For node with index 1 in the current group
                    randNum = Random.Range(1, 4); // 1 or 2 or 3
                    if (randNum == 3)
                    {
                        currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[1].Id);
                        currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[2].Id);
                        isNodeWithIndex1Connected = true;
                        isNodeWithIndex2Connected = true;
                    }
                    currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[randNum].Id);

                    // For node with index 2 in the current group
                    randNum = Random.Range(0, 2); // 0 or 1
                    if (randNum == 1)
                    {
                        currNodeGroup[2].ConnectedNodes.Add(nextNodeGroup[2].Id);
                        isNodeWithIndex2Connected = true;
                    }

                    if (!isNodeWithIndex1Connected)
                    {
                        randNum = Random.Range(0, 2); // 0 or 1
                        currNodeGroup[randNum].ConnectedNodes.Add(nextNodeGroup[1].Id);
                    }

                    if (!isNodeWithIndex2Connected)
                    {
                        randNum = Random.Range(1, 3); // 1 or 2
                        currNodeGroup[randNum].ConnectedNodes.Add(nextNodeGroup[2].Id);
                    }

                }
            }

            if (currNodeGroup.Count == 4)
            {
                // Always connect the two upper edge nodes 
                currNodeGroup[0].ConnectedNodes.Add(nextNodeGroup[0].Id);
                // Always connect the two bottom edge nodes 
                currNodeGroup[3].ConnectedNodes.Add(nextNodeGroup[nextNodeGroup.Count - 1].Id);

                if (nextNodeGroup.Count == 2)
                {
                    randNum = Random.Range(0, 3); // 0 or 1 or 2
                    if (randNum == 2)
                    {
                        currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[0].Id);
                        currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[1].Id);
                    }
                    else
                    {
                        currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[randNum].Id);
                    }


                    randNum = Random.Range(0, 3); // 0 or 1 or 2
                    if (randNum == 2)
                    {
                        currNodeGroup[2].ConnectedNodes.Add(nextNodeGroup[0].Id);
                        currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[1].Id);
                    }
                    else
                    {
                        currNodeGroup[2].ConnectedNodes.Add(nextNodeGroup[randNum].Id);
                    }
                }

                if (nextNodeGroup.Count == 3)
                {
                    bool isTheMiddleNodeConnected = false;
                    // For node with index 1 in the current group
                    randNum = Random.Range(0, 3); // 0 or 1 or 2
                    if (randNum == 2)
                    {
                        currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[0].Id);
                        currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[1].Id);
                        isTheMiddleNodeConnected = true;
                    }
                    else
                    {
                        currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[randNum].Id);
                        if (randNum == 1)
                            isTheMiddleNodeConnected = true;
                    }

                    // For node with index 2 in the current group
                    randNum = Random.Range(1, 4); // 1 or 2 or 3
                    if (randNum == 3)
                    {
                        currNodeGroup[2].ConnectedNodes.Add(nextNodeGroup[1].Id);
                        currNodeGroup[2].ConnectedNodes.Add(nextNodeGroup[2].Id);
                        isTheMiddleNodeConnected = true;
                    }
                    else
                    {
                        currNodeGroup[2].ConnectedNodes.Add(nextNodeGroup[randNum].Id);
                        if (randNum == 1)
                            isTheMiddleNodeConnected = true;
                    }

                    if (!isTheMiddleNodeConnected)
                    {
                        randNum = Random.Range(1, 3); // 1 or 2
                        currNodeGroup[randNum].ConnectedNodes.Add(nextNodeGroup[1].Id);
                    }
                }
            }
        }
    }
}
