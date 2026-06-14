using NUnit.Framework;
using UnityEngine;

public class AstarScript : MonoBehaviour
{
    public static AstarScript instance;

    private void Awake()
    {
        instance = this;
    }

    public List<Node> GenericPath(Node start, Node end)
    {
        List<Node> openSet = new List<Node>();

        foreach (Node n in FindObjectOfType<Node>())
        {
            n.gScore = float.MaxValue;
        }

        start.gScore = 0;
        start.hScore = Vector2.Distance(start.transform.position, end.transform.position);
        openSet.Add(start);

        while(openSet.Count)

        return null;
    }
}
