using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack Target", story: "[Self] gets attack position for [Target]", category: "Action", id: "a1b2c3d4e5f6789012345678901234ab")]
public partial class AttackTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> AttackRange = new BlackboardVariable<float>(1.5f);
    [SerializeReference] public BlackboardVariable<Vector2> AttackPosition;

    private Collider2D m_SelfCollider;
    private Collider2D m_TargetCollider;

    protected override Status OnStart()
    {
        if (Self.Value == null || Target.Value == null)
        {
            Debug.LogWarning("AttackTarget: Self or Target is null");
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

        // Calculate attack position based on target location
        Vector2 attackPos = CalculateAttackPosition();

        if (AttackPosition != null)
        {
            AttackPosition.Value = attackPos;
        }

        // Check if we can attack from current position
        if (IsInAttackRange())
        {
            return Status.Success;
        }
        else
        {
            return Status.Running; // Need to move to attack position first
        }
    }

    protected override void OnEnd()
    {
        // Clean up if needed
    }

    private Status Initialize()
    {
        // Get colliders
        m_SelfCollider = Self.Value.GetComponentInChildren<Collider2D>();
        m_TargetCollider = Target.Value.GetComponentInChildren<Collider2D>();

        if (m_SelfCollider == null)
        {
            Debug.LogWarning($"AttackTarget: No Collider2D found on {Self.Value.name}");
            return Status.Failure;
        }

        if (m_TargetCollider == null)
        {
            Debug.LogWarning($"AttackTarget: No Collider2D found on {Target.Value.name}");
            return Status.Failure;
        }

        return Status.Running;
    }

    private Vector2 CalculateAttackPosition()
    {
        // Get target's current position
        Vector2 targetPosition = Target.Value.transform.position;
        Vector2 selfPosition = Self.Value.transform.position;

        // Calculate direction from self to target
        Vector2 direction = (targetPosition - selfPosition).normalized;

        // Calculate ideal attack position (close enough to attack but not overlapping)
        float targetRadius = 0f;
        if (m_TargetCollider != null)
        {
            targetRadius = Mathf.Max(m_TargetCollider.bounds.extents.x, m_TargetCollider.bounds.extents.y);
        }

        // Position ourselves at attack range from target
        Vector2 attackPosition = targetPosition - direction * (AttackRange.Value + targetRadius);

        return attackPosition;
    }

    private bool IsInAttackRange()
    {
        if (m_SelfCollider == null || m_TargetCollider == null)
            return false;

        // Calculate distance between colliders
        Vector2 selfPosition = m_SelfCollider.bounds.center;
        Vector2 targetPosition = m_TargetCollider.bounds.center;
        float distance = Vector2.Distance(selfPosition, targetPosition);

        // Account for collider sizes
        float selfRadius = Mathf.Max(m_SelfCollider.bounds.extents.x, m_SelfCollider.bounds.extents.y);
        float targetRadius = Mathf.Max(m_TargetCollider.bounds.extents.x, m_TargetCollider.bounds.extents.y);
        float totalRadius = selfRadius + targetRadius;

        return distance <= (AttackRange.Value + totalRadius);
    }
}