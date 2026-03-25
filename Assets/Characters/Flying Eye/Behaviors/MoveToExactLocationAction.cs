using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move to Exact Location", story: "As [Self] move to exact location of [Target]", category: "Action", id: "e04f245ec2fe29f9bf4ece0d42c5bf8a")]
public partial class MoveToExactLocationAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<Vector2> DestinationPoint;
    [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(2.0f);
    [SerializeReference] public BlackboardVariable<float> SlowDownDistance = new BlackboardVariable<float>(1.5f);

    private Rigidbody2D m_Rigidbody2D;
    private Vector2 m_LastTargetPosition;
    private float m_CurrentSpeed;
    private const float ARRIVAL_THRESHOLD = 0.05f; // Very small threshold for "exact" positioning
    private bool m_IsInitialized = false;
    private float m_ColliderOffset = 0.0f; // Additional distance to account for colliders  

    protected override Status OnStart()
    {
        if (Self.Value == null || Target.Value == null)
        {
            return Status.Failure;
        }

        return Initialize();
    }

    protected override Status OnUpdate()
    {
        if (Self.Value == null || Target.Value == null)
        {
            return Status.Failure;
        }

        if (!m_IsInitialized)
        {
            return Status.Failure;
        }

        // Get current target position
        Vector2 currentTargetPosition = Target.Value.transform.position;

        // Update target if it has moved
        bool targetMoved = !Mathf.Approximately(m_LastTargetPosition.x, currentTargetPosition.x)
            || !Mathf.Approximately(m_LastTargetPosition.y, currentTargetPosition.y);

        if (targetMoved)
        {
            m_LastTargetPosition = currentTargetPosition;
        }

        // Calculate distance to target's exact position
        float distance = GetDistanceToTarget();
        bool destinationReached = distance <= ARRIVAL_THRESHOLD;

        if (destinationReached)
        {
            return Status.Success;
        }
        else
        {
            // Move toward target's exact position in 2D space
            m_CurrentSpeed = MoveTowardTarget(distance);
        }

        DestinationPoint.Value = currentTargetPosition;
        return Status.Running;
    }

    protected override void OnEnd()
    {
        m_IsInitialized = false;
    }

    private Status Initialize()
    {
        m_LastTargetPosition = Target.Value.transform.position;

        // Calculate collider offset for stopping distance
        m_ColliderOffset = 0.0f;
        Collider2D selfCollider = Self.Value.GetComponentInChildren<Collider2D>();
        if (selfCollider != null)
        {
            Vector2 colliderSize = selfCollider.bounds.size;
            m_ColliderOffset += Mathf.Max(colliderSize.x, colliderSize.y) * 0.5f;
        }

        Collider2D playerCollider = Target.Value.GetComponentInChildren<Collider2D>();
        if (playerCollider != null)
        {
            Vector2 colliderSize = playerCollider.bounds.size;
            m_ColliderOffset += Mathf.Max(colliderSize.x, colliderSize.y) * 0.5f;
        }

        // Check if already at exact destination
        if (GetDistanceToTarget() <= ARRIVAL_THRESHOLD)
        {
            return Status.Success;
        }

        // Get Rigidbody2D component for physics-based movement
        m_Rigidbody2D = Self.Value.GetComponent<Rigidbody2D>();
        m_IsInitialized = true;

        return Status.Running;
    }

    private float GetDistanceToTarget()
    {
        Vector2 selfPosition = Self.Value.transform.position;
        Vector2 targetPosition = Target.Value.transform.position;
        return Vector2.Distance(selfPosition, targetPosition);
    }

    private float MoveTowardTarget(float distance)
    {
        Vector2 selfPosition = Self.Value.transform.position;
        Vector2 targetPosition = Target.Value.transform.position;

        // Calculate direction toward target's exact position (NOT away from target)
        Vector2 direction = (targetPosition - selfPosition).normalized;

        float currentSpeed = Speed.Value;

        // Apply movement
        Vector2 movement = direction * currentSpeed * Time.fixedDeltaTime;

        if (m_Rigidbody2D != null)
        {
            // Physics-based movement
            m_Rigidbody2D.MovePosition(selfPosition + movement);
        }
        else
        {
            // Transform-based movement
            Self.Value.transform.position = selfPosition + movement;
        }

        return currentSpeed;
    }
}