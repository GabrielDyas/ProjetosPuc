using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ControleDeSomPersonagem : MonoBehaviour
{
    [Header("Clipes de Áudio de Passos")]
    [Tooltip("Arraste todos os sons do pé esquerdo para esta lista.")]
    [SerializeField] private AudioClip[] sonsDoPeEsquerdo;

    [Tooltip("Arraste todos os sons do pé direito para esta lista.")]
    [SerializeField] private AudioClip[] sonsDoPeDireito;

    private AudioSource fonteDeAudio;

    private void Awake()
    {
        fonteDeAudio = GetComponent<AudioSource>();
        fonteDeAudio.playOnAwake = false;
    }

    public void TocarSomDePasso(string pe)
    {

        AudioClip[] listaDeSons = (pe.ToLower() == "esquerdo") ? sonsDoPeEsquerdo : sonsDoPeDireito;

        if (listaDeSons.Length == 0)
        {
            Debug.LogWarning($"Nenhum som de passo definido para o pé '{pe}' em {gameObject.name}");
            return;
        }

        AudioClip clipeParaTocar = listaDeSons[Random.Range(0, listaDeSons.Length)];

        fonteDeAudio.PlayOneShot(clipeParaTocar);
    }
}