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
        // Establecemos conexión
        connectionString = "URI=file:" + Application.dataPath + "/database.db";
        dbConnection = new SqliteConnection(connectionString);
        dbConnection.Open();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void InsertResultados(int puntos, float tiempo)
    {
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

    private void CerrarConexionBBDD()
    {
        dbConnection.Close();
        dbConnection = null;
    }
    
}

