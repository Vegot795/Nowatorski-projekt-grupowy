using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Patrol", story: "[Agent] walks to random [Point] around him", category: "Action", id: "af63d86a38dd118a6ecbc39eb8449d7c")]
public partial class PatrolAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector2> Point;
    [SerializeReference] public BlackboardVariable<float> Speed;
    [SerializeReference] public BlackboardVariable<float> VisionRange;
    [SerializeReference] public BlackboardVariable<float> ARRIVAL_THRESHOLD;

    private Vector2 m_CurrentPosition;
    private Vector2 m_TargetPoint;
    private Rigidbody2D m_Rigidbody2D;
    private bool m_isInitialized = false;

    protected override Status OnStart()
    {
        if (Agent == null || VisionRange == null || Speed == null)
        {
            return Status.Failure;
        }
        return Initialize();
    }

    protected override Status OnUpdate()
    {
        if (Agent == null || VisionRange == null || Speed == null)
        {
            return Status.Failure;
        }

        if(!m_isInitialized)
        {
            return Status.Failure;
        }

        m_CurrentPosition = Agent.Value.transform.position;
        float distance = Vector2.Distance(m_CurrentPosition, m_TargetPoint);

        if (distance <= ARRIVAL_THRESHOLD)
        {
            return Status.Success; 
        }
        // Move toward target point

        Vector2 direction = (m_TargetPoint - m_CurrentPosition).normalized;
        Vector2 movement = direction * Speed.Value * Time.fixedDeltaTime;

        Rigidbody2D rb = Agent.Value.GetComponent<Rigidbody2D>();
        if (m_Rigidbody2D != null)
        {
            m_Rigidbody2D.MovePosition(m_CurrentPosition + movement);
        }
        else
        {
            Agent.Value.transform.position = m_CurrentPosition + movement;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }

    private Status Initialize ()
    {
        float randomX = UnityEngine.Random.Range(m_CurrentPosition.x - VisionRange.Value, m_CurrentPosition.x + VisionRange.Value);
        float randomY = UnityEngine.Random.Range(m_CurrentPosition.y - VisionRange.Value, m_CurrentPosition.y + VisionRange.Value);
        m_TargetPoint = new Vector2(m_TargetPoint.x + randomX, m_TargetPoint.y + randomY);

        m_Rigidbody2D = Agent.Value.GetComponent<Rigidbody2D>();

        m_isInitialized = true;
        Point.Value = m_TargetPoint;


        float distance = Vector2.Distance(m_CurrentPosition, m_TargetPoint);
        if(distance <= ARRIVAL_THRESHOLD)
        {
            return Status.Success;
        }

        return Status.Running;
    }


}

