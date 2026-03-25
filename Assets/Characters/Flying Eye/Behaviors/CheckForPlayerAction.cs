using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Check For Player", story: "Locates the [Player] in the world", category: "Action", id: "998802a2715813530f5d528fa9002c9b")]
public partial class CheckForPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Player = null;
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
 
    protected override Status OnStart()
    {
        if (Player != null)
        {
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        GameObject closestObject;
        closestObject = FindTarget();

        if (closestObject != null)
        {
            Player.Value = closestObject;
            return Status.Success;
        }
        else
        {
            Player.Value = null;
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
    private GameObject FindTarget()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");
        float closestDistanceSq = Mathf.Infinity;
        GameObject closestGameObject = null;
        foreach (GameObject gameObject in gameObjects)
        {
            float distanceSq = Vector3.Distance(Agent.Value.transform.position, gameObject.transform.position);
            if (closestGameObject == null || distanceSq < closestDistanceSq)
            {
                closestGameObject = gameObject;
                closestDistanceSq = distanceSq;
            }
        }
        return closestGameObject;

    }
}

