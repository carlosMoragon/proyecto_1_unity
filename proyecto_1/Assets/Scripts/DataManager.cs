using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

public class DataManager : MonoBehaviour
{

    private string connectionString;
    private IDbConnection dbConnection;
    

    // Start is called before the first frame update
    void Start()
    {
        
        // ESTABLECEMOS CONEXIÓN
        connectionString = "URI=file:" + Application.dataPath + "/database.db";
        dbConnection = new SqliteConnection(connectionString);
        dbConnection.Open();

        
    }

    // Update is called once per frame
    void Update()
    {

        
        
    }

    public void InsertResultados(int puntos, float tiempo){

        int ultimoIdJugador = ObtenerUltimoIdJugador();
        Debug.Log($"El ID del último jugador insertado es: {ultimoIdJugador}");

        // Insertar partida en la base de datos
        string sentenciaInsertarPartida = "INSERT INTO partidas (puntuacion, tiempo, jugadorid) VALUES (@puntuacion, @tiempo, @jugadorid);";
        IDbCommand insertPartidaCmd = dbConnection.CreateCommand();
        insertPartidaCmd.CommandText = sentenciaInsertarPartida;

        IDbDataParameter puntuacionParam = insertPartidaCmd.CreateParameter();
        puntuacionParam.ParameterName = "@puntuacion";
        puntuacionParam.Value = puntos;
        insertPartidaCmd.Parameters.Add(puntuacionParam);

        IDbDataParameter tiempoParam = insertPartidaCmd.CreateParameter();
        tiempoParam.ParameterName = "@tiempo";
        tiempoParam.Value = tiempo; // Guardar el tiempo de juego en segundos
        insertPartidaCmd.Parameters.Add(tiempoParam);

        IDbDataParameter jugadoridParam = insertPartidaCmd.CreateParameter();
        jugadoridParam.ParameterName = "@jugadorid";
        jugadoridParam.Value = ultimoIdJugador;
        insertPartidaCmd.Parameters.Add(jugadoridParam);

        insertPartidaCmd.ExecuteNonQuery();
    }

    private int ObtenerUltimoIdJugador()
    {
        string sentenciaSelect = "SELECT max(id) as id FROM jugadores;";
        IDbCommand selectCmd = dbConnection.CreateCommand();
        selectCmd.CommandText = sentenciaSelect;
        object result = selectCmd.ExecuteScalar();
        return int.Parse(result.ToString());
    }

    private void CerrarConexionBBDD(){
        dbConnection.Close();
        dbConnection = null;
    }

    /*
    private void SaveCSVToFile()
    {
        string data = "Name; Timestamp; x; y; z\n";
        foreach (CharacterPosition cp in playerPos.positions)
        {
            data += cp.ToCSV() + "\n";
        }
        foreach (CharacterPosition cp in enemyPos.positions)
        {
            data += cp.ToCSV() + "\n";
        }
        FileManager.WriteToFile("positions.csv", data);
    }

    private void SaveJSONToFile()
    {
        string data = "[";
        foreach (CharacterPosition cp in playerPos.positions)
        {
            data += JsonUtility.ToJson(cp) + ",\n";
        }
        foreach (CharacterPosition cp in enemyPos.positions)
        {
            data += JsonUtility.ToJson(cp) + ",\n";
        }
        data += "]";
        FileManager.WriteToFile("positions.json", data);

        // Forma alternativa
        FileManager.WriteToFile("playerPostions.json", JsonUtility.ToJson(playerPos));
        FileManager.WriteToFile("enemyPostions.json", JsonUtility.ToJson(enemyPos));
    }

    private void SaveXMLToFile()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Positions));
        using (FileStream stream = new FileStream("playerPositions.xml", FileMode.Create))
        {
            serializer.Serialize(stream, playerPos);
        }

        using (FileStream stream = new FileStream("enemyPositions.xml", FileMode.Create))
        {
            serializer.Serialize(stream, enemyPos);
        }
    }
    */
}
