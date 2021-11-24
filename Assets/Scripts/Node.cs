using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> neighbours = new List<Node>();
    private Renderer rend;

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
    [HideInInspector]
    public bool belongPath;

    private void Awake()
    {
        gValue = 999999f;
        hValue = 999999f;
        fValue = 999999f;
    }

    private void Start()
    {
        //Get material to set shader properties
        rend = GetComponent<Renderer>();

        //Get materials
        var aStart = FindObjectOfType<AStart>();

        //Init change materials
        StartCoroutine(NodeVisited());
        StartCoroutine(NodeBelongPath());
    }

    private IEnumerator NodeVisited()
    {
        yield return new WaitUntil(() => visited);

        rend.material.SetInt("wasVisited", 1);
    }

    private IEnumerator NodeBelongPath()
    {
        yield return new WaitUntil(() => belongPath);

        rend.material.SetInt("belongPath", 1);
    }
}
