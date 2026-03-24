using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

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

        float distance = Vector2.Distance(m_AgentPoint, m_TargetPoint);
        if (distance <= VisionRange.Value)
        {
            return Status.Success;

        }
        return Status.Failure;
    }
}

