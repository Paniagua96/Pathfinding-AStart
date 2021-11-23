using UnityEngine;

public class BackgColor : MonoBehaviour
{
    private Renderer rend;
    public Color startColor;
    public Color endColor;
    public float speed = 0.01f;


    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Animate color
        var newColor = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time * speed, 1));
        rend.material.SetColor("_Color", newColor);
    }
}
