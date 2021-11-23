using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> neighbours = new List<Node>();

    [HideInInspector]
    public float gValue = 999999f;
    [HideInInspector]
    public float hValue = 999999f;
    [HideInInspector]
    public float fValue = 999999f;
    [HideInInspector]
    public Node parent;
    [HideInInspector]
    public bool visited;

    private void Awake()
    {
        gValue = 999999f;
        hValue = 999999f;
        fValue = 999999f;
    }
}
