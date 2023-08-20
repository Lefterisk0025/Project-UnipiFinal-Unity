using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAG<T>
{
    private readonly Dictionary<T, List<T>> _vertexAdjList = new(); // Graph as Adjacency List

    protected void AddVertex(T vertex)
    {
        if (!_vertexAdjList.ContainsKey(vertex))
            _vertexAdjList[vertex] = new List<T>();
    }

    private bool HasCycle(T vertex, HashSet<T> visited, HashSet<T> path)
    {
        visited.Add(vertex);
        path.Add(vertex);

        foreach (T neighbor in _vertexAdjList[vertex])
        {
            if (!visited.Contains(neighbor))
            {
                if (HasCycle(neighbor, visited, path))
                {
                    return true;
                }
            }
            else if (path.Contains(neighbor))
            {
                return true;
            }
        }

        path.Remove(vertex);
        return false;
    }

    protected bool AddEdge(T fromVertex, T toVertex)
    {
        if (!_vertexAdjList.ContainsKey(fromVertex))
            AddVertex(fromVertex);

        if (!_vertexAdjList.ContainsKey(toVertex))
            AddVertex(toVertex);

        _vertexAdjList[fromVertex].Add(toVertex);

        // Check for cycles
        var visited = new HashSet<T>();
        var path = new HashSet<T>();

        if (HasCycle(fromVertex, visited, path))
        {
            _vertexAdjList[fromVertex].Remove(toVertex);
            return false;
        }

        return true;
    }

    public Dictionary<T, List<T>> GetGraph()
    {
        return _vertexAdjList;
    }

    protected void DisplayGraph()
    {
        foreach (var vertex in _vertexAdjList)
        {
            Debug.Log($"{vertex.Key}: ");

            foreach (var edge in vertex.Value)
            {
                Debug.Log($"{edge} ");
            }
        }
    }

}
