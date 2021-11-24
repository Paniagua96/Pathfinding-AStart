using System.Collections;
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
    [HideInInspector]
    public bool belongPath;

    private Material matVisited;
    private Material matBelongPath;

    private void Awake()
    {
        gValue = 999999f;
        hValue = 999999f;
        fValue = 999999f;
    }

    private void Start()
    {
        //Get materials
        var aStart = FindObjectOfType<AStart>();
        matVisited = aStart.matVisited;
        matBelongPath = aStart.matBelongPath;

        //Init change materials
        StartCoroutine(NodeVisited());
        StartCoroutine(NodeBelongPath());
    }

    private IEnumerator NodeVisited()
    {
        yield return new WaitUntil(() => visited);

        gameObject.GetComponent<MeshRenderer>().material = matVisited;
    }

    private IEnumerator NodeBelongPath()
    {
        yield return new WaitUntil(() => belongPath);

        gameObject.GetComponent<MeshRenderer>().material = matBelongPath;
    }
}
