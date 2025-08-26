using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{

    [Header("Layers")]
    public LayerMask groundLayer;

    [Space]

    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public int wallSide;

    [Space]

    [Header("Collision")]
    [Space]
    [Header("Bottom")]
    [SerializeField] private Vector2 bottomOffset;
    [SerializeField] private Vector2 bottomCollision;
    [Header("Side")]
    [Tooltip("Right and Left")]
    [SerializeField] private Vector2  rightOffset, leftOffset;
    [Header("Size")]
    [SerializeField] private Vector2 sideCollision;
    private Color debugCollisionColor = Color.red;





    void Start()
    {

    }

    void Update()
    {
        _Collison();
    }
    private void _Collison()
    {
        onGround = Physics2D.OverlapBox((Vector2)transform.position + bottomOffset, bottomCollision,0f, groundLayer);
        onWall = Physics2D.OverlapBox((Vector2)transform.position + rightOffset, sideCollision, 0f, groundLayer)
            || Physics2D.OverlapBox((Vector2)transform.position + leftOffset, sideCollision, 0f, groundLayer);

        onRightWall = Physics2D.OverlapBox((Vector2)transform.position + rightOffset, sideCollision, 0f, groundLayer);
        onLeftWall = Physics2D.OverlapBox((Vector2)transform.position + leftOffset, sideCollision, 0f, groundLayer);

        wallSide = onRightWall ? -1 : 1;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };

        Gizmos.DrawWireCube((Vector2)transform.position + bottomOffset, bottomCollision);
        Gizmos.DrawWireCube((Vector2)transform.position + rightOffset, sideCollision);
        Gizmos.DrawWireCube((Vector2)transform.position + leftOffset, sideCollision);
    }
}