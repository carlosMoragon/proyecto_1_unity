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
    public GameObject player;
    public GameObject[] enemies;
    private float prevTime;
    private float prevSaveTime;
    private float logInterval = 1; //En segundos
    private float logSaveInterval = 5;
    private Positions playerPos;
    private Positions enemyPos;
    private string connectionString;
    private IDbConnection dbConnection;

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


        prevTime = Time.realtimeSinceStartup;
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
        float currentTime = Time.realtimeSinceStartup;
        if(currentTime > prevTime + logInterval) {
            prevTime += logInterval;
            CharacterPosition cp = new CharacterPosition(player.name, currentTime, player.transform.position);
            playerPos.positions.Add(cp);
            foreach (GameObject enemy in enemies) {
                CharacterPosition en = new CharacterPosition(enemy.name, currentTime, enemy.transform.position);
                enemyPos.positions.Add(en);
            }
        }
        if(currentTime > prevSaveTime + logSaveInterval) {
            prevSaveTime += logSaveInterval;
            SaveCSVToFile();
            SaveJSONToFile();
            SaveXMLToFile();
        }
        
        
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
}
