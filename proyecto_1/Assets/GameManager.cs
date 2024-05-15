using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int vidaMaxima = 10;
    public int vidaActual;
    public float tiempoDeJuego;
    public int puntaje;

    public float tiempoParaGanar = 300f; // 5 minutos en segundos

    public TextMeshProUGUI vidaText;
    public TextMeshProUGUI tiempoText;
    public TextMeshProUGUI puntajeText;

    void Start()
    {
        vidaActual = vidaMaxima;
        tiempoDeJuego = 0f;
        puntaje = 0;
        ActualizarInterfaz();
    }

    void Update()
    {
        if (vidaActual > 0)
        {
            tiempoDeJuego += Time.deltaTime;
            ActualizarTiempo();
            SumarPuntosPorMinuto();
        }
        else
        {
            // Si la vida llega a 0, cambiar de escena al "Game Over"
            SceneManager.LoadScene("Menu");
        }

        if (tiempoDeJuego >= tiempoParaGanar)
        {
            // Si se alcanza el tiempo necesario para ganar, cambiar de escena al "Ganar"
            SceneManager.LoadScene("Menu");
        }
    }

    void ActualizarTiempo()
    {
        // Actualiza el texto del tiempo en la interfaz de usuario
        tiempoText.text = "Tiempo: " + Mathf.FloorToInt(tiempoDeJuego / 60f) + "m " + Mathf.FloorToInt(tiempoDeJuego % 60f) + "s";
    }

    void SumarPuntosPorMinuto()
    {
        // Suma un punto cada minuto transcurrido
        if (Mathf.FloorToInt(tiempoDeJuego) % 60 == 0)
        {
            puntaje++;
            ActualizarInterfaz();
        }
    }

    void ActualizarInterfaz()
    {
        // Actualiza los textos en la interfaz de usuario
        vidaText.text = "Vida: " + vidaActual;
        puntajeText.text = "Puntaje: " + puntaje;
    }

    // Método para disminuir la vida del jugador
    public void DanioAlJugador(int danio)
    {
        vidaActual -= danio;
        ActualizarInterfaz();
    }
}