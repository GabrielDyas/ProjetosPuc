using UnityEngine;

public class AltarManager : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Arraste todos os objetos de Altar da sua cena para esta lista.")]
    [SerializeField] private Altar[] altars;
    [SerializeField] private GameObject doorLeft;
    [SerializeField] private GameObject doorRight;

    [Header("Parâmetros da Porta")]
    [SerializeField] private float doorOpenHeight = 2f;

    [SerializeField] private bool doorsHaveBeenOpened = false; // Trava para garantir que as portas abram apenas uma vez

    public bool PortasAbertas => doorsHaveBeenOpened;

    void Update()
    {
        // Se as portas já abriram, não precisa verificar mais nada.
        if (doorsHaveBeenOpened)
        {
            return;
        }

        // Verifica se todos os altares estão ativados
        if (AreAllAltarsActivated())
        {
            OpenDoors();
            doorsHaveBeenOpened = true; // Ativa a trava
        }
    }

    /// <summary>
    /// Verifica cada altar na lista. Se encontrar UM que não esteja ativado, retorna falso.
    /// Se o loop terminar, significa que todos estão ativados, então retorna verdadeiro.
    /// </summary>
    private bool AreAllAltarsActivated()
    {
        // Garante que a lista não está vazia
        if (altars == null || altars.Length == 0)
        {
            return false;
        }

        foreach (Altar altar in altars)
        {
            // Usa a nova propriedade pública 'EstaAtivado' para checar o estado
            if (!altar.EstaAtivado)
            {
                return false; // Encontrou um altar inativo, para a verificação.
            }
        }

        // Se chegou até aqui, todos os altares estão ativados.
        return true;
    }

    void OpenDoors()
    {
        Debug.Log("<color=cyan>Todos os altares ativados! Abrindo as portas.</color>");

        // Sua lógica de abrir as portas está correta.
        if (doorLeft != null)
        {
            // Move a porta para baixo
            doorLeft.transform.position += new Vector3(-doorOpenHeight, 0, 0);
        }
        if (doorRight != null)
        {
            // Move a porta para cima
            doorRight.transform.position += new Vector3(doorOpenHeight, 0, 0);
        }
    }
}

