using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace MoreMountains.Tools
{
    public class MMSaveLoadManagerMethodJson : IMMSaveLoadManagerMethod
    {
        /// <summary>
        ///     Saves the specified object at the specified location after converting it to json
        /// </summary>
        /// <param name="objectToSave"></param>
        /// <param name="saveFile"></param>
        public void Save(object objectToSave, FileStream saveFile)
        {
            var json = JsonUtility.ToJson(objectToSave);
            // if you prefer using NewtonSoft's JSON lib uncomment the line below and commment the line above
            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(objectToSave);
            var streamWriter = new StreamWriter(saveFile);
            streamWriter.Write(json);
            streamWriter.Close();
            saveFile.Close();
        }

        /// <summary>
        ///     Loads the specified file and decodes it
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="saveFile"></param>
        /// <returns></returns>
        public object Load(Type objectType, FileStream saveFile)
        {
            object savedObject; // = System.Activator.CreateInstance(objectType);
            var streamReader = new StreamReader(saveFile, Encoding.UTF8);
            var json = streamReader.ReadToEnd();
            savedObject = JsonUtility.FromJson(json, objectType);
            // if you prefer using NewtonSoft's JSON lib uncomment the line below and commment the line above
            //savedObject = Newtonsoft.Json.JsonConvert.DeserializeObject(json,objectType);
            streamReader.Close();
            saveFile.Close();
            return savedObject;
        }
    }
}