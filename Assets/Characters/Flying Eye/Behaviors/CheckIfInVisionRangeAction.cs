using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckIfInVisionRange", story: "[Self] Agent checks if [Target] is in [VisionRange]", category: "Action", id: "7d641478eb989a19b71490d931261be4")]
public partial class CheckIfInVisionRangeAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> VisionRange;

    private Vector2 m_TargetPoint;
    private Vector2 m_AgentPoint;
    private bool m_IsInitialized = false;

    protected override Status OnStart()
    {
        if(Self == null || Target == null || VisionRange == null)
        {
            return Status.Failure;
        }

        return Initialize();
    }

    protected override Status OnUpdate()
    {
        if (Self == null || Target == null || VisionRange == null)
        {
            return Status.Failure;
        }

        if (!m_IsInitialized)
        {
            return Status.Failure;
        }

        GameObject closestGameObject = FindTarget();
        if (closestGameObject != null)
        {
            m_TargetPoint = closestGameObject.transform.position;
        }
        //float distance = Vector2.Distance(m_AgentPoint, m_TargetPoint);
        //if (distance <= VisionRange.Value)
        if (closestGameObject != null)
        {
            Target.Value = closestGameObject; // Update the target to the closest one within vision range
            return Status.Success;

        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
        m_IsInitialized = false;
    }

    private Status Initialize()
    {
        m_TargetPoint = Target.Value.transform.position;
        m_AgentPoint = Self.Value.transform.position;
        m_IsInitialized = true;


        return Status.Running;
    }

    private GameObject FindTarget()
    {
               GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");
        float closestDistanceSq = Mathf.Infinity;
        GameObject closestGameObject = null;
        foreach (GameObject gameObject in gameObjects)
        {
            float distanceSq = Vector3.Distance(m_AgentPoint, gameObject.transform.position);
            if (closestGameObject == null || distanceSq < closestDistanceSq)
            {
                closestGameObject = gameObject;
                closestDistanceSq = distanceSq;
            }
        }
        return closestGameObject;

    }
}

