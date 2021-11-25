using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System;
using Random = UnityEngine.Random;

public class AStart : MonoBehaviour
{
    public Volume volume;
    public GameObject psStart;
    public GameObject psEnd;
    private Node start;
    private Node end;
    private Node currentNode;
    private List<Node> testNode = new List<Node>();
    public static List<Node> allNodes = new List<Node>();
    public static Action OnFinishPath;

    private ChromaticAberration chromaticAberration;
    private Vignette vignette;
    private float speedVolume = 0.01f;

    private float originalChromatic;
    private float originalVignette;
    private int randomNodeStart;
    private int randomNodeEnd;

    private void Start()
    {
        //Get chromatic aberration to set it
        volume.profile.TryGet<ChromaticAberration>(out var tmp);
        chromaticAberration = tmp;
        volume.profile.TryGet<Vignette>(out var tmpVig);
        vignette = tmpVig;

        //Get original PPE values
        originalChromatic = chromaticAberration.intensity.value;
        originalVignette = vignette.intensity.value;

        //Init search
        StartCoroutine(SearchPath());
    }

    private void OnDestroy()
    {
        allNodes.Clear();
        allNodes = null;
    }

    private IEnumerator SearchPath()
    {
        while (true)
        {
            SetStartAndEndNode();

            yield return new WaitForSeconds(3f);

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
                    yield return new WaitForSeconds(.03f);
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
                yield return new WaitForSeconds(.03f);

                chromaticAberration.intensity.value = 1f;

                while (chromaticAberration.intensity.value > 0.01f)
                {
                    chromaticAberration.intensity.value -= speedVolume;
                    yield return null;
                }
            }
            yield return new WaitForSeconds(3f);

            ResetSearch();
        }
    }

    private void SetStartAndEndNode()
    {
        randomNodeStart = 0;
        randomNodeEnd = 0;

        var distance = GetNodesDistance(allNodes[randomNodeStart], allNodes[randomNodeEnd]);

        while (distance < 100)
        {
            randomNodeStart = Random.Range(0, allNodes.Count - 1);
            randomNodeEnd = Random.Range(0, allNodes.Count - 1);

            distance = GetNodesDistance(allNodes[randomNodeStart], allNodes[randomNodeEnd]);
        }

        var index = 0;

        foreach (var node in allNodes)
        {
            if (index == randomNodeStart)
            {
                start = allNodes[index];
                psStart.SetActive(true);
                psStart.transform.SetParent(start.gameObject.transform, true);
                psStart.transform.localPosition = Vector3.zero;
                psStart.transform.localScale = Vector3.one;
            }

            if (index == randomNodeEnd)
            {
                end = allNodes[index];
                psEnd.SetActive(true);
                psEnd.transform.SetParent(end.gameObject.transform, true);
                psEnd.transform.localPosition = Vector3.zero;
                psEnd.transform.localScale = Vector3.one;
            }
            index++;
        }
    }

    private void ResetSearch()
    {
        start = null;
        end = null;
        currentNode = null;
        testNode.Clear();

        chromaticAberration.intensity.value = originalChromatic;
        vignette.intensity.value = originalVignette;

        psStart.transform.parent = null;
        psEnd.transform.parent = null;

        psStart.SetActive(false);
        psEnd.SetActive(false);

        OnFinishPath?.Invoke();
    }

    private float GetNodesDistance(Node a, Node b) => Vector3.Distance(a.gameObject.transform.position, b.gameObject.transform.position);
}
