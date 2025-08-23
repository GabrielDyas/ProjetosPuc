using UnityEngine;

public class Altar : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject pointPatrol;

    [Header("Parâmetros de Carga")]
    [SerializeField] private float distanciaParaAtivar = 3f;
    [SerializeField] private float tempoParaCarregar = 10f;

    [Header("Estado do Altar")]
    [SerializeField]
    [Range(0, 10)]
    private float cargaAtual = 0f;

    [SerializeField] private bool estaAtivado = false;

    private IAPatrol inimigo;


    public bool EstaAtivado => estaAtivado;
    public float ProgressoDaCarga => cargaAtual;

    private void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogError("A referência do 'playerTransform' não foi definida no Inspector!", this);
        }

        inimigo = FindObjectOfType<IAPatrol>();
        if (inimigo == null)
        {
            Debug.LogWarning("Nenhum inimigo (IAPatrol) encontrado na cena.", this);
        }
    }

    void Update()
    {
        if (estaAtivado || playerTransform == null)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

        if (distanceToPlayer <= distanciaParaAtivar)
        {
            CarregarAltar();
        }
    }

    private void CarregarAltar()
    {
        cargaAtual += Time.deltaTime;

        if (cargaAtual >= tempoParaCarregar / 2)
        {
            if (inimigo != null)
            {
                inimigo.CallToLocation(pointPatrol.transform);
            }

        }

        if (cargaAtual >= tempoParaCarregar)
        {
            cargaAtual = tempoParaCarregar;
            estaAtivado = true;
            AtivarEfeitoFinal();
        }
    }

    private void AtivarEfeitoFinal()
    {
        if (inimigo != null)
        {
            inimigo.CallToLocation(pointPatrol.transform);
        }

        Debug.Log("<color=green>ALTAR ATIVADO!</color>");
        this.enabled = false;
    }
}
