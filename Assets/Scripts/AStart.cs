using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.Universal;

public class AStart : MonoBehaviour
{
    public Node start;
    public Node end;
    public Material matCurrent;
    public Material matVisited;
    public Volume volume;

    private ChromaticAberration chromaticAberration;
    private float speedVolume = 0.01f;
    private Node currentNode;
    private List<Node> testNode = new List<Node>();

    private void Start()
    {
        volume.profile.TryGet<ChromaticAberration>(out var tmp);
        chromaticAberration = tmp;

        StartCoroutine(SearchPath());
    }

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
            yield return new WaitForSeconds(0.1f);
        }

        print("End searching");

        //Show the closest path
        var resultNodes = end;
        while (resultNodes != null)
        {
            chromaticAberration.intensity.value = 1f;
            while (chromaticAberration.intensity.value > 0.01f)
            {
                chromaticAberration.intensity.value -= speedVolume;
                yield return null;
            }
            resultNodes.GetComponent<MeshRenderer>().material = matCurrent;
            resultNodes = resultNodes.parent;
            yield return new WaitForSeconds(.1f);
        }
        print("Path showed");
    }

    private float GetNodesDistance(Node a, Node b) => Vector3.Distance(a.gameObject.transform.position, b.gameObject.transform.position);
}
