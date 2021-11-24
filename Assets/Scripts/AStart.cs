using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class AStart : MonoBehaviour
{
    public Node start;
    public Node end;
    public Material matVisited;
    public Material matBelongPath;
    public Volume volume;

    private ChromaticAberration chromaticAberration;
    private Vignette vignette;
    private float speedVolume = 0.01f;
    private Node currentNode;
    private List<Node> testNode = new List<Node>();

    private void Start()
    {
        //Get chromatic aberration to set it
        volume.profile.TryGet<ChromaticAberration>(out var tmp);
        chromaticAberration = tmp;
        volume.profile.TryGet<Vignette>(out var tmpVig);
        vignette = tmpVig;

        //Init search
        StartCoroutine(SearchPath());
    }

    private IEnumerator SearchPath()
    {
        //set current to start node
        currentNode = start;
        //Mark as visited to avoid back
        start.visited = true;

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
                }
                yield return null;
            }

            //Get closest neoghbour and set as current
            currentNode = testNode.OrderBy(node => node.fValue).FirstOrDefault();
            testNode.Clear();
            yield return new WaitForSeconds(0.1f);
        }

        //Show the closest path
        var resultNodes = end;
        vignette.intensity.value = 0.5f;

        while (resultNodes != null)
        {
            resultNodes.belongPath = true;
            resultNodes = resultNodes.parent;
            yield return new WaitForSeconds(.1f);

            chromaticAberration.intensity.value = 1f;
            while (chromaticAberration.intensity.value > 0.01f)
            {
                chromaticAberration.intensity.value -= speedVolume;
                yield return null;
            }
        }
    }

    private float GetNodesDistance(Node a, Node b) => Vector3.Distance(a.gameObject.transform.position, b.gameObject.transform.position);
}
