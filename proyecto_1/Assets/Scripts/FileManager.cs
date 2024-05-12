using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public static class FileManager
{
    public static bool WriteToFile(string filename, string data)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, filename);

        try
        {
            File.WriteAllText(fullPath, data);
            Debug.Log("Fichero guardado correctamente en: " +  fullPath);
            return true;
        }
        catch (Exception e){
            Debug.Log("Error al guardar el fichero en: " + fullPath + " con el error " + e);
            return false;
        }
    }
}

