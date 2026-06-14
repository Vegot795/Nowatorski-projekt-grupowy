using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;


public class Node : MonoBehaviour
{
    public Node cameFrom;
    public List<Node> nodes;
    public Transform cell;

    public float gScore;
    public float hScore;
    public float FScore()
    {
        return gScore + hScore;
    }

    public void ResetNode()
    {
        gScore = float.MaxValue;
        hScore = 0;
        cameFrom = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (nodes.Count > 0)
        {
            for(int i=0; i < nodes.Count; i++)
            {
                Gizmos.DrawLine(transform.position, nodes[i].transform.position);
            }
        }

    }
}
