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

    [SerializeField] private bool doorsHaveBeenOpened = false; 

    public bool PortasAbertas => doorsHaveBeenOpened;

    void Update()
    {

        if (doorsHaveBeenOpened)
        {
            return;
        }


        if (AreAllAltarsActivated())
        {
            OpenDoors();
            doorsHaveBeenOpened = true; 
        }
    }


    private bool AreAllAltarsActivated()
    {

        if (altars == null || altars.Length == 0)
        {
            return false;
        }

        foreach (Altar altar in altars)
        {

            if (!altar.EstaAtivado)
            {
                return false; 
            }
        }


        return true;
    }

    void OpenDoors()
    {
        Debug.Log("<color=cyan>Todos os altares ativados! Abrindo as portas.</color>");


        if (doorLeft != null)
        {

            doorLeft.transform.position += new Vector3(-doorOpenHeight, 0, 0);
        }
        if (doorRight != null)
        {

            doorRight.transform.position += new Vector3(doorOpenHeight, 0, 0);
        }
    }
}

