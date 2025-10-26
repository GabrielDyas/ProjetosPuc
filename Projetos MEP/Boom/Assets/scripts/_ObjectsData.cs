using UnityEngine;

public class _ObjectsData : MonoBehaviour
{
    [SerializeField][Range(1f, 10f)]
    public float life;

    private void Update()
    {
        if (life <= 0)
        {
            Destroy(gameObject);
        }
    }
}