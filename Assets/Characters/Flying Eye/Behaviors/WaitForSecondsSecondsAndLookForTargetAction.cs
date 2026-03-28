using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Wait for seconds seconds and look for target", story: "Wait for [seconds] seconds and look for [target]", category: "Action", id: "28413d0829b3da2041679d172409070b")]
public partial class WaitForSecondsSecondsAndLookForTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<float> SecondsToWait;
    [SerializeReference] public BlackboardVariable<float> Fov;
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [CreateProperty] private float m_Timer = 0.0f;

    protected override Status OnStart()
    {
        m_Timer = SecondsToWait;
        if (m_Timer <= 0.0f)
        {
            return Status.Success;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {

        if(Self.Value == null || Target.Value == null)
        {
            return Status.Failure;
        }

        m_Timer -= Time.deltaTime;

        float distanceToTarget = GetDistanceToTarget();

        if(distanceToTarget < Fov)
        {
            return Status.Success;
        }

        if (m_Timer <= 0)
        {
            return Status.Success;
        }

        return Status.Running;
    }
    private float GetDistanceToTarget()
    {
        Vector2 selfPosition = Self.Value.transform.position;
        Vector2 targetPosition = Target.Value.transform.position;
        return Vector2.Distance(selfPosition, targetPosition);
    }
}
