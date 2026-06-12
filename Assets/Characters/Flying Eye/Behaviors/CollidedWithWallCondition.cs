using System;
using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Collided with wall", story: "If [self] collides with the [wall]", category: "Conditions", id: "caed28cb0d7658bf973525abfdc12558")]
public partial class CollidedWithWallCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Wall;
    
    private bool _collided = false;

    public override bool IsTrue()
    {
        return _collided;
    }

    public override void OnStart()
    {
        _collided = false;
        TilemapCollider2D tilemapCollider = Wall.Value.GetComponent<TilemapCollider2D>();
        Collider2D selfCollider = Self.Value.GetComponent<Collider2D>();
    }

    public override void OnEnd()
    {
        _collided = false;
    }
    
    private void OnTriggerEnter2D(Collider2D wall)
    {
        if (wall.gameObject == Wall.Value)
        {
            _collided = true;
        }
    }
}
