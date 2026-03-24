using System;
using Unity.Properties;
using UnityEngine;
using Unity.Behavior;
using Action = Unity.Behavior.Action;

namespace Unity.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Navigate To Target 2D",
        description: "Navigates a GameObject towards another GameObject in 2D space (XY plane)." +
        "\nMoves the Agent using its transform for 2D movement.",
        story: "[Agent] navigates to [Target] in 2D",
        category: "Action/Navigation",
        id: "2dc19d3122374cc9a985d90351633311")]
    public partial class NavigateToTarget2DAction : Action
    {
        public enum TargetPositionMode
        {
            ClosestPointOnAnyCollider,      // Use the closest point on any collider, including child objects
            ClosestPointOnTargetCollider,   // Use the closest point on the target's own collider only
            ExactTargetPosition             // Use the exact position of the target, ignoring colliders
        }

        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(1.0f);
        [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new BlackboardVariable<float>(0.2f);
        [SerializeReference] public BlackboardVariable<string> AnimatorSpeedParam = new BlackboardVariable<string>("SpeedMagnitude");
        [SerializeReference] public BlackboardVariable<float> SlowDownDistance = new BlackboardVariable<float>(1.0f);
        [Tooltip("Defines how the target position is determined for navigation:" +
            "\n- ClosestPointOnAnyCollider: Use the closest point on any collider, including child objects" +
            "\n- ClosestPointOnTargetCollider: Use the closest point on the target's own collider only" +
            "\n- ExactTargetPosition: Use the exact position of the target, ignoring colliders. Default if no collider is found.")]
        [SerializeReference] public BlackboardVariable<TargetPositionMode> m_TargetPositionMode = new(TargetPositionMode.ClosestPointOnAnyCollider);

        private Animator m_Animator;
        private Vector2 m_LastTargetPosition;
        private Vector2 m_ColliderAdjustedTargetPosition;
        private float m_ColliderOffset;
        private float m_CurrentSpeed;
        private Rigidbody2D m_Rigidbody2D;

        protected override Status OnStart()
        {
            if (Agent.Value == null || Target.Value == null)
            {
                return Status.Failure;
            }

            return Initialize();
        }

        protected override Status OnUpdate()
        {
            if (Agent.Value == null || Target.Value == null)
            {
                return Status.Failure;
            }

            // Check if the target position has changed.
            Vector2 currentTargetPosition = Target.Value.transform.position;
            bool boolUpdateTargetPosition = !Mathf.Approximately(m_LastTargetPosition.x, currentTargetPosition.x)
                || !Mathf.Approximately(m_LastTargetPosition.y, currentTargetPosition.y);

            if (boolUpdateTargetPosition)
            {
                m_LastTargetPosition = currentTargetPosition;
                m_ColliderAdjustedTargetPosition = GetPositionColliderAdjusted();
            }

            float distance = GetDistanceXY();
            bool destinationReached = distance <= (DistanceThreshold + m_ColliderOffset);

            if (destinationReached)
            {
                return Status.Success;
            }
            else
            {
                // Transform-based 2D movement
                m_CurrentSpeed = SimpleMoveTowards2D(Agent.Value.transform, m_ColliderAdjustedTargetPosition,
                    Speed, distance, SlowDownDistance);
            }

            UpdateAnimatorSpeed();

            return Status.Running;
        }

        protected override void OnEnd()
        {
            UpdateAnimatorSpeed(0f);
            m_Animator = null;
        }

        private Status Initialize()
        {
            m_LastTargetPosition = Target.Value.transform.position;
            m_ColliderAdjustedTargetPosition = GetPositionColliderAdjusted();

            // Add the extents of the 2D colliders to the stopping distance.
            m_ColliderOffset = 0.0f;
            Collider2D agentCollider = Agent.Value.GetComponentInChildren<Collider2D>();
            if (agentCollider != null)
            {
                Vector2 colliderSize = agentCollider.bounds.size;
                m_ColliderOffset += Mathf.Max(colliderSize.x, colliderSize.y) * 0.5f;
            }

            if (GetDistanceXY() <= (DistanceThreshold + m_ColliderOffset))
            {
                return Status.Success;
            }

            m_Animator = Agent.Value.GetComponentInChildren<Animator>();
            m_Rigidbody2D = Agent.Value.GetComponent<Rigidbody2D>();
            UpdateAnimatorSpeed(0f);

            return Status.Running;
        }

        private Vector2 GetPositionColliderAdjusted()
        {
            switch (m_TargetPositionMode.Value)
            {
                case TargetPositionMode.ClosestPointOnAnyCollider:
                    Collider2D anyCollider = Target.Value.GetComponentInChildren<Collider2D>(includeInactive: false);
                    if (anyCollider == null || !anyCollider.enabled)
                        break;
                    return anyCollider.ClosestPoint(Agent.Value.transform.position);
                case TargetPositionMode.ClosestPointOnTargetCollider:
                    Collider2D targetCollider = Target.Value.GetComponent<Collider2D>();
                    if (targetCollider == null || !targetCollider.enabled)
                        break;
                    return targetCollider.ClosestPoint(Agent.Value.transform.position);
            }

            // Default to target position.
            return Target.Value.transform.position;
        }

        private float GetDistanceXY()
        {
            Vector2 agentPosition = Agent.Value.transform.position;
            return Vector2.Distance(agentPosition, m_ColliderAdjustedTargetPosition);
        }

        private float SimpleMoveTowards2D(Transform agentTransform, Vector2 targetPosition, float speed, float distance, float slowDownDistance)
        {
            Vector2 currentPosition = agentTransform.position;
            Vector2 direction = (targetPosition - currentPosition).normalized;

            // Calculate speed with slow down
            float currentSpeed = speed;
            if (distance < slowDownDistance)
            {
                currentSpeed = speed * (distance / slowDownDistance);
                currentSpeed = Mathf.Max(currentSpeed, speed * 0.1f); // Minimum speed
            }

            // Move using Rigidbody2D if available, otherwise use transform
            Vector2 movement = direction * currentSpeed * Time.fixedDeltaTime;
            
            if (m_Rigidbody2D != null)
            {
                m_Rigidbody2D.MovePosition(currentPosition + movement);
            }
            else
            {
                agentTransform.position = currentPosition + movement;
            }

            return currentSpeed;
        }

        private void UpdateAnimatorSpeed(float explicitSpeed = -1)
        {
            if (m_Animator == null || string.IsNullOrEmpty(AnimatorSpeedParam.Value))
                return;

            float speedToSet = explicitSpeed >= 0 ? explicitSpeed : m_CurrentSpeed;
            
            if (m_Animator.parameters != null)
            {
                foreach (var param in m_Animator.parameters)
                {
                    if (param.name == AnimatorSpeedParam.Value && param.type == AnimatorControllerParameterType.Float)
                    {
                        m_Animator.SetFloat(AnimatorSpeedParam.Value, speedToSet);
                        break;
                    }
                }
            }
        }
    }
}