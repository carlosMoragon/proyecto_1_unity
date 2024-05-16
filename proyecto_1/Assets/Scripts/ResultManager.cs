using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Data;
using Mono.Data.Sqlite;


public class ResultManager : MonoBehaviour
{

    public TextMeshProUGUI lastScoreText;
    public TextMeshProUGUI podiumText;

    private IDbConnection dbConnection;

    void Start()
    {
        // Configura la conexión a la base de datos
        string connectionString = "URI=file:" + Application.dataPath + "/database.db";
        dbConnection = new SqliteConnection(connectionString);
        dbConnection.Open();

        MostrarUltimaPuntuacion();
        MostrarPodio();
    }

    void MostrarUltimaPuntuacion()
    {
        // Consulta para obtener la última puntuación con el nombre del jugador asociado
        string query = "SELECT jugadores.nombre, partidas.puntuacion FROM jugadores INNER JOIN partidas ON jugadores.id = partidas.jugadorid ORDER BY partidas.id DESC LIMIT 1;";
    
        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = query;
        IDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            string nombreJugador = reader.GetString(0);
            int ultimaPuntuacion = reader.GetInt32(1);
            lastScoreText.text = "Última puntuación de " + nombreJugador + ": " + ultimaPuntuacion.ToString();
        }

        reader.Close();
    }


    
    void MostrarPodio()
    {
        // Consulta para obtener las mejores puntuaciones
        string query = "SELECT nombre, puntuacion FROM jugadores INNER JOIN partidas ON jugadores.id = partidas.jugadorid ORDER BY puntuacion DESC LIMIT 3;";
        
        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = query;
        IDataReader reader = command.ExecuteReader();

        List<string> podio = new List<string>();
        int rank = 1;

        while (reader.Read())
        {
            string nombre = reader.GetString(0);
            int puntuacion = reader.GetInt32(1);
            podio.Add($"{rank}. {nombre}: {puntuacion} puntos");
            rank++;
        }

        podiumText.text = string.Join("\n", podio);

        reader.Close();
    }


    public void HacerAnonima()
    {
        string query = "UPDATE partidas SET jugadorid = 0 WHERE id = (SELECT MAX(id) FROM partidas);";
    
        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = query;
        command.ExecuteNonQuery();

        MostrarUltimaPuntuacion();
        MostrarPodio();
    }

    public void EliminarUltimaPartida()
    {
        string query = "DELETE FROM partidas WHERE id = (SELECT MAX(id) FROM partidas);";

        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = query;

        command.ExecuteNonQuery();
    }


    void OnDestroy()
    {
        dbConnection.Close();
    }

}
