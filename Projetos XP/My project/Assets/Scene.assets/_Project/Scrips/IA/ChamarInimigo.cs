using UnityEngine;

public class ChamarInimigo : MonoBehaviour
{
    [Tooltip("A referência ao script IAPatrol do inimigo.")]
    public IAPatrol inimigo;

    [Tooltip("O local para onde o inimigo será chamado.")]
    public Transform pontoDeDistracao;

    void Update()
    {
        // Exemplo: Pressione a tecla "C" para chamar o inimigo
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (inimigo != null && pontoDeDistracao != null)
            {
                inimigo.CallToLocation(pontoDeDistracao);
            }
        }
    }
}