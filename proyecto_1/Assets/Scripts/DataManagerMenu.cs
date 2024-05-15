using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using System.Data;
using UnityEngine.UI;
using TMPro;
using Mono.Data.Sqlite;

public class DataManagerMenu : MonoBehaviour
{


    private string connectionString;
    private IDbConnection dbConnection;
    public TMP_InputField nombreInputField;
    public TMP_InputField edadInputField;

    // Start is called before the first frame update
    void Start()
    {
        // ESTABLECEMOS CONEXIÓN
        connectionString = "URI=file:" + Application.dataPath + "/database.db";
        dbConnection = new SqliteConnection(connectionString);
        dbConnection.Open();

        // CONFIGURACIÓN DE LAS TABLAS DE LA BBDD

        string sentenciaCreacionJugadores = "CREATE TABLE IF NOT EXISTS jugadores(id INTEGER PRIMARY KEY, nombre TEXT not null, edad INTEGER not null);";
        string sentenciaCreacionPartidas = "CREATE TABLE IF NOT EXISTS partidas(id INTEGER PRIMARY KEY, puntuacion INTEGER not null, tiempo TIME not null, jugadorid INTEGER REFERENCES jugadores(id));";

        Creartabla(sentenciaCreacionJugadores);
        Creartabla(sentenciaCreacionPartidas);


        /*prevTime = Time.realtimeSinceStartup;
        prevSaveTime = prevTime;
        playerPos = new Positions();
        enemyPos = new Positions();

        //Prueba de XML
        CharacterPosition cp = new CharacterPosition("Prueba", 123123123, Vector3.right);
        XmlSerializer serializer = new XmlSerializer(typeof(CharacterPosition));
        using (FileStream stream = new FileStream("exampleXML.xml", FileMode.Create))
        {
            serializer.Serialize(stream, cp);
        }
        PlayerPrefs.SetString("nombre", "MaxUser");
        Debug.Log(PlayerPrefs.GetString("nombre"));
        */
    }

    // Update is called once per frame
    void Update()
    {

    }

    // ESTABLECEMOS LAS TABLAS DE LA BBDD SI NO ESTÁN CREADAS
    private void Creartabla(string sentencia){


        IDbCommand createTableCmd = dbConnection.CreateCommand();

        createTableCmd.CommandText = sentencia;
        createTableCmd.ExecuteNonQuery();
    }

    private void CerrarConexionBBDD(){
        dbConnection.Close();
        dbConnection = null;
    }
    /*
    public void EnviarInfoJugador()
    {
        int edadint = int.Parse(edad);
        string sentencia = "INSERT INTO jugadores VALUES(@nombre, @edad);";

        IDbCommand insertCmd = dbConnection.CreateCommand();

        insertCmd.CommandText = sentencia;

        // Agregar parámetros
        IDbDataParameter nombreParam = insertCmd.CreateParameter();
        nombreParam.ParameterName = "@nombre";
        nombreParam.Value = nombre;
        insertCmd.Parameters.Add(nombreParam);

        IDbDataParameter edadParam = insertCmd.CreateParameter();
        edadParam.ParameterName = "@edad";
        edadParam.Value = edadint;
        insertCmd.Parameters.Add(edadParam);


        insertCmd.ExecuteNonQuery();
    }
    */
     public void EnviarInfoJugador()
    {
        // Obtener los valores de los campos de texto
        string nombre = nombreInputField.text;
        string edad = edadInputField.text;

        // Convertir edad a entero
        int edadInt;
        if (!int.TryParse(edad, out edadInt))
        {
            Debug.LogError("La edad ingresada no es un número válido.");
            return;
        }

        // Crear la sentencia SQL con parámetros
        string sentencia = "INSERT INTO jugadores (nombre, edad) VALUES (@nombre, @edad);";

        // Crear comando SQL
        IDbCommand insertCmd = dbConnection.CreateCommand();
        insertCmd.CommandText = sentencia;

        // Agregar parámetros
        IDbDataParameter nombreParam = insertCmd.CreateParameter();
        nombreParam.ParameterName = "@nombre";
        nombreParam.Value = nombre;
        insertCmd.Parameters.Add(nombreParam);

        IDbDataParameter edadParam = insertCmd.CreateParameter();
        edadParam.ParameterName = "@edad";
        edadParam.Value = edadInt;
        insertCmd.Parameters.Add(edadParam);

        // Ejecutar comando
        insertCmd.ExecuteNonQuery();
    }

    void OnDestroy()
    {
        // Cerrar conexión cuando el objeto sea destruido
        if (dbConnection != null && dbConnection.State != ConnectionState.Closed)
        {
            dbConnection.Close();
        }
    }

}