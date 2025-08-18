using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{
    [Header("Referências dos Painéis")]
    [SerializeField] private GameObject painelTelaInicial;
    [SerializeField] private GameObject painelTelaGame;
    [SerializeField] private GameObject painelTelaMorte;
    [SerializeField] private GameObject painelTelaFinal;

    [Header("Referências da UI de Vida")]
    [SerializeField] private List<Image> iconesDeVida;

    [Header("Referências da UI do Altar")]
    [SerializeField] private GameObject painelProgressoAltar;
    [SerializeField] private Slider barraProgresso;
    [SerializeField] private TextMeshProUGUI textoEstado;
    [SerializeField] private Image imagemFundoEstado;

    [Header("Referências da UI de Aviso")]
    [SerializeField] private GameObject painelDeAviso;
    [SerializeField] private TextMeshProUGUI textoDeAviso;
    [SerializeField] private Image imagemFundoAviso;
    [SerializeField] private Color corAlerta = Color.yellow;
    [SerializeField] private Color corPerigo = Color.red;

    [Header("Referências do Jogo")]
    [SerializeField] private PlayerMoviment scriptDoPlayer;
    [SerializeField] private AltarManager altarManager;
    [SerializeField] private IAPatrol inimigo;
    [SerializeField] private ProximityDebuff debuffInimigo;

    private Altar[] todosOsAltares;
    private Altar altarMaisProximo;
    private bool jogoAcabou = false;

    void Start()
    {
        painelTelaInicial.SetActive(true);
        painelTelaGame.SetActive(false);
        painelTelaMorte.SetActive(false);
        painelTelaFinal.SetActive(false);
        Time.timeScale = 0f;

        todosOsAltares = FindObjectsOfType<Altar>();
        if (painelProgressoAltar != null) painelProgressoAltar.SetActive(false);
        if (textoEstado != null) textoEstado.gameObject.SetActive(false);
        if (imagemFundoEstado != null) imagemFundoEstado.enabled = false;
        if (painelDeAviso != null) painelDeAviso.SetActive(false);
    }

    void Update()
    {
        if (jogoAcabou || Time.timeScale == 0) return;

        if (scriptDoPlayer != null && scriptDoPlayer.VidaAtual <= 0)
        {
            MostrarTelaDeMorte();
            return;
        }

        AtualizarVidas();
        GerenciarAvisosDePerigo();

        if (!painelDeAviso.activeSelf)
        {
            GerenciarExibicaoUIAltar();
        }
        else
        {
            painelProgressoAltar.SetActive(false);
            textoEstado.gameObject.SetActive(false);
            imagemFundoEstado.enabled = false;
        }
    }

    public void IniciarJogo()
    {
        painelTelaInicial.SetActive(false);
        painelTelaGame.SetActive(true);
        Time.timeScale = 1f;
    }

    public void RecomecarJogo()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MostrarTelaFinal()
    {
        if (jogoAcabou) return;

        jogoAcabou = true;
        painelTelaGame.SetActive(false);
        painelTelaFinal.SetActive(true);
        Time.timeScale = 0f;
    }

    private void MostrarTelaDeMorte()
    {
        if (jogoAcabou) return;

        jogoAcabou = true;
        painelTelaGame.SetActive(false);
        painelTelaMorte.SetActive(true);
        Time.timeScale = 0f;
    }

    private void AtualizarVidas()
    {
        if (scriptDoPlayer == null) return;

        for (int i = 0; i < iconesDeVida.Count; i++)
        {
            if (i < scriptDoPlayer.VidaAtual)
            {
                iconesDeVida[i].enabled = true;
            }
            else
            {
                iconesDeVida[i].enabled = false;
            }
        }
    }

    private void GerenciarAvisosDePerigo()
    {
        if (inimigo == null || debuffInimigo == null || painelDeAviso == null) return;

        bool estaSendoCacado = inimigo.CurrentState == IAPatrol.PatrolState.Hunting;
        bool estaNaAreaDeDebuff = debuffInimigo.SpeedMultiplier < 1.0f;

        if (estaSendoCacado)
        {
            painelDeAviso.SetActive(true);
            textoDeAviso.text = "PERIGO";
            imagemFundoAviso.color = corPerigo;
        }
        else if (estaNaAreaDeDebuff)
        {
            painelDeAviso.SetActive(true);
            textoDeAviso.text = "Alerta";
            imagemFundoAviso.color = corAlerta;
        }
        else
        {
            painelDeAviso.SetActive(false);
        }
    }

    private void GerenciarExibicaoUIAltar()
    {
        if (scriptDoPlayer == null || todosOsAltares.Length == 0 || altarManager == null) return;

        if (altarManager.PortasAbertas)
        {
            painelProgressoAltar.SetActive(false);
            textoEstado.gameObject.SetActive(true);
            imagemFundoEstado.enabled = true;
            textoEstado.text = "A porta foi aberta!";
            return;
        }

        EncontrarAltarMaisProximo();

        if (altarMaisProximo == null || Vector3.Distance(scriptDoPlayer.transform.position, altarMaisProximo.transform.position) > 5f)
        {
            painelProgressoAltar.SetActive(false);
            textoEstado.gameObject.SetActive(false);
            imagemFundoEstado.enabled = false;
            return;
        }

        textoEstado.gameObject.SetActive(true);
        imagemFundoEstado.enabled = true;

        if (altarMaisProximo.EstaAtivado)
        {
            painelProgressoAltar.SetActive(false);
            textoEstado.text = "Altar Ativado";
        }
        else
        {
            painelProgressoAltar.SetActive(true);
            barraProgresso.value = altarMaisProximo.ProgressoDaCarga;
            textoEstado.text = "Carregando Altar...";
        }
    }

    private void EncontrarAltarMaisProximo()
    {
        float menorDistancia = float.MaxValue;
        altarMaisProximo = null;

        foreach (Altar altar in todosOsAltares)
        {
            float distancia = Vector3.Distance(scriptDoPlayer.transform.position, altar.transform.position);
            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                altarMaisProximo = altar;
            }
        }
    }
}
