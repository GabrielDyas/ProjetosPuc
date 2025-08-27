using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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


    public Vector2 GetTilemapIntersectionSize(Tilemap map, BoxCollider2D checkBox)
    {
        // Se não houver mapa ou colisor, não há como calcular
        if (map == null || checkBox == null)
        {
            return Vector2.zero;
        }

        // 1. Define a caixa de checagem usando a posição, tamanho e offset do colisor fornecido
        Vector2 boxCenter = (Vector2)checkBox.transform.position + checkBox.offset;
        Vector2 boxSize = checkBox.size;
        Bounds checkBoxBounds = new Bounds(boxCenter, boxSize);

        // 2. Converte os limites da caixa de checagem para coordenadas de célula do tilemap
        Vector3Int minCell = map.WorldToCell(checkBoxBounds.min);
        Vector3Int maxCell = map.WorldToCell(checkBoxBounds.max);

        float realWidth = 0;
        bool tileFoundInRow = false;

        // 3. Itera apenas nas células que estão dentro da caixa de checagem
        for (int y = minCell.y; y <= maxCell.y; y++)
        {
            tileFoundInRow = false; // Reseta para cada linha
            float rowWidth = 0; // Largura para esta linha específica

            for (int x = minCell.x; x <= maxCell.x; x++)
            {
                // 4. Verifica se a célula atual contém um tile
                if (map.GetTile(new Vector3Int(x, y, 0)) != null)
                {
                    tileFoundInRow = true;
                    rowWidth += map.cellSize.x;
                }
            }

            // Pega a maior largura encontrada entre as linhas verificadas
            if (rowWidth > realWidth)
            {
                realWidth = rowWidth;
            }

            // Otimização: se encontramos algum tile, podemos parar,
            // pois geralmente a checagem do chão só precisa da primeira linha de contato.
            if (tileFoundInRow)
            {
                break;
            }
        }

        // 5. A altura da interseção é a altura da própria caixa de checagem
        float height = checkBoxBounds.size.y;

        return new Vector2(realWidth, height);
    }
}