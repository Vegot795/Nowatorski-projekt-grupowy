using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Patrol", story: "Patrol Nearby area", category: "Action", id: "af63d86a38dd118a6ecbc39eb8449d7c")]
public partial class PatrolAction : Action
{
    [SerializeReference] public BlackboardVariable<float> Speed;
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> PatrolRange;
    [SerializeReference] public BlackboardVariable<float> ARRIVAL_THRESHOLD;
    [SerializeReference] public BlackboardVariable<bool> PlayerInVision;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> VisionRange = new BlackboardVariable<float>(3f);

    private Vector2 m_CurrentPosition;
    private Vector2 m_TargetPoint;
    private Rigidbody2D m_Rigidbody2D;
    private bool m_isInitialized;

    protected override Status OnStart()
    {
        if (Agent?.Value == null || Speed == null || PatrolRange == null || ARRIVAL_THRESHOLD == null)
        {
            return Status.Failure;
        }

        m_isInitialized = false;
        return Initialize();
    }

    protected override Status OnUpdate()
    {
        if (Agent?.Value == null || Speed == null || ARRIVAL_THRESHOLD == null)
        {
            return Status.Failure;
        }

        if (IsPlayerInVision())
        {
            StopMovement();
            return Status.Failure;
        }

        if (!m_isInitialized)
        {
            return Status.Failure;
        }

        m_CurrentPosition = Agent.Value.transform.position;
        float distance = Vector2.Distance(m_CurrentPosition, m_TargetPoint);

        if (distance <= ARRIVAL_THRESHOLD.Value)
        {
            return Status.Success;
        }

        Vector2 direction = (m_TargetPoint - m_CurrentPosition).normalized;
        Vector2 movement = direction * Speed.Value * Time.deltaTime;

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
        StopMovement();
        m_isInitialized = false;
    }

    private Status Initialize()
    {
        m_CurrentPosition = Agent.Value.transform.position;

        float randomX = UnityEngine.Random.Range(
            m_CurrentPosition.x - PatrolRange.Value,
            m_CurrentPosition.x + PatrolRange.Value);

        float randomY = UnityEngine.Random.Range(
            m_CurrentPosition.y - PatrolRange.Value,
            m_CurrentPosition.y + PatrolRange.Value);

        m_TargetPoint = new Vector2(randomX, randomY);

        m_Rigidbody2D = Agent.Value.GetComponent<Rigidbody2D>();
        m_isInitialized = true;

        return Status.Running;
    }

    private bool IsPlayerInVision()
    {
        if (PlayerInVision != null && PlayerInVision.Value)
        {
            return true;
        }

        GameObject target = Target?.Value;

        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }

        if (target == null || VisionRange == null)
        {
            return false;
        }

        float distanceToTarget = Vector2.Distance(
            Agent.Value.transform.position,
            target.transform.position);

        return distanceToTarget <= VisionRange.Value;
    }

    private void StopMovement()
    {
        if (m_Rigidbody2D != null)
        {
            m_Rigidbody2D.linearVelocity = Vector2.zero;
        }
    }
}