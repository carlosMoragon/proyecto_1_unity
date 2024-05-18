using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml.Serialization;
using System.IO;
using TMPro;
using System.Data;
using Mono.Data.Sqlite;
using System;
using System.Globalization;
using System.Text;


public class ResultManager : MonoBehaviour
{

    public TextMeshProUGUI lastScoreText;
    public TextMeshProUGUI podiumText;

    private IDbConnection dbConnection;
    private string connectionString;


    void Start()
    {
        // Configura la conexión a la base de datos
        connectionString = "URI=file:" + Application.dataPath + "/database.db";
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

    /*public void SaveJSONToFile()
    {
        List<Jugador> jugadores = ObtenerJugadores();
        List<Partida> partidas = ObtenerPartidas();

        // Imprime la cantidad de jugadores y partidas obtenidas
        Debug.Log("Cantidad de jugadores: " + jugadores.Count);
        Debug.Log("Cantidad de partidas: " + partidas.Count);

        // Serializar jugadores
        StringBuilder jugadoresJson = new StringBuilder("[");
        foreach (var jugador in jugadores)
        {
            // Imprime el objeto jugador antes de serializarlo
            Debug.Log("Jugador antes de serializar: " + jugador.ToString());

            jugadoresJson.Append(JsonUtility.ToJson(jugador));
            jugadoresJson.Append(",");
        }
        jugadoresJson.Remove(jugadoresJson.Length - 1, 1); // Remover la última coma
        jugadoresJson.Append("]");

        // Imprime la cadena JSON de jugadores
        Debug.Log("JSON de jugadores: " + jugadoresJson.ToString());

        // Serializar partidas
        StringBuilder partidasJson = new StringBuilder("[");
        foreach (var partida in partidas)
        {
            // Imprime el objeto partida antes de serializarlo
            Debug.Log("Partida antes de serializar: " + partida.ToString());

            partidasJson.Append(JsonUtility.ToJson(partida));
            partidasJson.Append(",");
        }
        partidasJson.Remove(partidasJson.Length - 1, 1); // Remover la última coma
        partidasJson.Append("]");

        // Imprime la cadena JSON de partidas
        Debug.Log("JSON de partidas: " + partidasJson.ToString());

        FileManager.WriteToFile("jugadores.json", jugadoresJson.ToString());
        FileManager.WriteToFile("partidas.json", partidasJson.ToString());
    }
    */
    
    public void SaveJSONToFile()
    {
        List<Jugador> jugadores = ObtenerJugadores();
        List<Partida> partidas = ObtenerPartidas();

        // Serializar jugadores
        string jsonJugadores = "[";
        foreach (var jugador in jugadores)
        {
            jsonJugadores += SerializeJugador(jugador) + ",";
        }
        jsonJugadores = jsonJugadores.TrimEnd(',') + "]";

        FileManager.WriteToFile("jugadores.json", jsonJugadores);

        // Serializar partidas
        string jsonPartidas = "[";
        foreach (var partida in partidas)
        {
            jsonPartidas += SerializePartida(partida) + ",";
        }
        jsonPartidas = jsonPartidas.TrimEnd(',') + "]";

        FileManager.WriteToFile("partidas.json", jsonPartidas);
    }

    private string SerializeJugador(Jugador jugador)
    {
        return "{" +
            "\"Id\":" + jugador.Id + "," +
            "\"Nombre\":\"" + jugador.Nombre + "\"," +
            "\"Edad\":" + jugador.Edad +
            "}";
    }

    private string SerializePartida(Partida partida)
    {
        return "{" +
            "\"Id\":" + partida.Id + "," +
            "\"Puntuacion\":" + partida.Puntuacion + "," +
            "\"Tiempo\":\"" + partida.Tiempo + "\"," +
            "\"JugadorId\":" + partida.JugadorId +
            "}";
    }





    /*
    public void SaveJSONToFile()
    {
        List<Jugador> jugadores = ObtenerJugadores();
        List<Partida> partidas = ObtenerPartidas();

        string jsonJugadores = JsonUtility.ToJson(new { jugadores }, true);
        string jsonPartidas = JsonUtility.ToJson(new { partidas }, true);

        FileManager.WriteToFile("jugadores.json", jsonJugadores);
        FileManager.WriteToFile("partidas.json", jsonPartidas);
    }
    */
    
    public void SaveXMLToFile()
    {
        List<Jugador> jugadores = ObtenerJugadores();
        List<Partida> partidas = ObtenerPartidas();

        XmlSerializer jugadorSerializer = new XmlSerializer(typeof(List<Jugador>));
        XmlSerializer partidaSerializer = new XmlSerializer(typeof(List<Partida>));

        using (FileStream jugadorStream = new FileStream("jugadores.xml", FileMode.Create))
        {
            jugadorSerializer.Serialize(jugadorStream, jugadores);
        }

        using (FileStream partidaStream = new FileStream("partidas.xml", FileMode.Create))
        {
            partidaSerializer.Serialize(partidaStream, partidas);
        }
    }
    
   

    /*
    private void SaveJSONToFile()
    {
        List<Jugador> jugadores = ObtenerJugadores();
        List<Partida> partidas = ObtenerPartidas();

        string jsonJugadores = JsonUtility.ToJson(new { jugadores }, true);
        string jsonPartidas = JsonUtility.ToJson(new { partidas }, true);

        FileManager.WriteToFile("jugadores.json", jsonJugadores);
        FileManager.WriteToFile("partidas.json", jsonPartidas);
    }

    private void SaveXMLToFile()
    {
        List<Jugador> jugadores = ObtenerJugadores();
        List<Partida> partidas = ObtenerPartidas();

        XmlSerializer jugadorSerializer = new XmlSerializer(typeof(List<Jugador>));
        XmlSerializer partidaSerializer = new XmlSerializer(typeof(List<Partida>));

        using (MemoryStream jugadorStream = new MemoryStream())
        {
            jugadorSerializer.Serialize(jugadorStream, jugadores);
            FileManager.WriteToFile("jugadores.xml", System.Text.Encoding.UTF8.GetString(jugadorStream.ToArray()));
        }

        using (MemoryStream partidaStream = new MemoryStream())
        {
            partidaSerializer.Serialize(partidaStream, partidas);
            FileManager.WriteToFile("partidas.xml", System.Text.Encoding.UTF8.GetString(partidaStream.ToArray()));
        }
    }
    */

    private List<Jugador> ObtenerJugadores()
    {
        List<Jugador> jugadores = new List<Jugador>();

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM jugadores";
            using (var command = connection.CreateCommand())
            {
                command.CommandText = query;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        jugadores.Add(new Jugador
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Edad = reader.GetInt32(2)
                        });
                    }
                }
            }
        }

        return jugadores;
    }

    private List<Partida> ObtenerPartidas()
    {
        string formatoFechaHora = "yyyy-MM-dd HH:mm:ss";

        List<Partida> partidas = new List<Partida>();

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM partidas";
            using (var command = connection.CreateCommand())
            {
                command.CommandText = query;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        partidas.Add(new Partida
                        {
                            Id = reader.GetInt32(0),
                            Puntuacion = reader.GetInt32(1),
                            Tiempo = DateTime.Now.ToString(formatoFechaHora),
                            JugadorId = reader.GetInt32(3)
                        });
                    }
                }
            }
        }

        return partidas;
    }

}
