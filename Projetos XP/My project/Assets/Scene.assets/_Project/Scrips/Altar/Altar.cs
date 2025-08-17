using UnityEngine;

public class Altar : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("A referência ao Transform do jogador. Arraste o objeto do Player para cá.")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject pointPatrol;

    [Header("Parâmetros de Carga")]
    [Tooltip("A distância máxima que o jogador precisa estar para carregar o altar.")]
    [SerializeField] private float distanciaParaAtivar = 3f;
    [Tooltip("O tempo total (em segundos) necessário para carregar o altar completamente.")]
    [SerializeField] private float tempoParaCarregar = 10f;

    [Header("Estado do Altar")]
    [Tooltip("O progresso atual da carga do altar. Não precisa mexer, é apenas para visualização.")]
    [SerializeField]
    [Range(0, 10)] // O valor máximo do slider deve ser igual ao tempoParaCarregar
    private float cargaAtual = 0f;
    private IAPatrol inimigo;

    [SerializeField] private bool estaAtivado = false;

    // NOVO: Propriedade pública para que outros scripts possam ler o estado sem modificá-lo.
    public bool EstaAtivado => estaAtivado;

    private void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogError("A referência do 'playerTransform' não foi definida no Inspector!", this);
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
        Debug.Log($"Carregando altar... Progresso: {cargaAtual:F2} / {tempoParaCarregar}");

        if (cargaAtual == tempoParaCarregar/2)
        {
            inimigo.CallToLocation(pointPatrol.transform);
            return;
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
        inimigo.CallToLocation(pointPatrol.transform);
        Debug.Log("<color=green>ALTAR ATIVADO!</color>");
        this.enabled = false;
    }
}
