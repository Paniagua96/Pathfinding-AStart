using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class AStart : MonoBehaviour
{
    private List<Node> testNode = new List<Node>();
    public Node start;
    public Node end;
    public Material matCurrent;
    public Material matVisited;
    private Node currentNode;

    private void Start() => StartCoroutine(SearchPath());

    private IEnumerator SearchPath()
    {
        //set current to start node
        currentNode = start;
        //Mark as visited to avoid back
        start.visited = true;
        start.GetComponent<MeshRenderer>().material = matVisited;

        //Search closest nodes to the end
        while (currentNode.gameObject.name != end.gameObject.name)
        {
            //Verified each neighbour and set values
            foreach (var neighbour in currentNode.neighbours)
            {
                if (!neighbour.visited)
                {
                    testNode.Add(neighbour);
                    neighbour.gValue = GetNodesDistance(neighbour, end);
                    neighbour.hValue = GetNodesDistance(neighbour, start);

                    neighbour.fValue = neighbour.gValue + neighbour.hValue;
                    neighbour.parent = currentNode;
                    neighbour.visited = true;
                    neighbour.GetComponent<MeshRenderer>().material = matVisited;
                }
                yield return null;
            }

            //Get closest neoghbour and set as current
            currentNode = testNode.OrderBy(node => node.fValue).FirstOrDefault();
            testNode.Clear();
            yield return new WaitForSeconds(0.5f);
        }

        print("End searching");

        //Show the closest path
        var resultNodes = end;
        while (resultNodes != null)
        {
            resultNodes.GetComponent<MeshRenderer>().material = matCurrent;
            resultNodes = resultNodes.parent;
            yield return new WaitForSeconds(0.1f);
        }
        print("Path showed");
    }

    private float GetNodesDistance(Node a, Node b) => Vector3.Distance(a.gameObject.transform.position, b.gameObject.transform.position);
}
