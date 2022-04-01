using UnityEngine;
using System.IO;
using UnityEditor;

public class TextFileHandler : MonoBehaviour
{
    const string m_path = "D:/RL_Vs_Dijktra_Thesis_Project/Thesis/Project/Assets/Resources";
    const string m_name = "RL_DATA";


    [MenuItem("Tools/Write file")]
    public static void WriteString()
    {
        string path = m_path + "/RL_DATA.txt";
        print(path);

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine("Test");
        writer.Close();


        //StreamReader reader = new StreamReader(path);
        ////Print the text from the file
        //Debug.Log(reader.ReadToEnd());
        //reader.Close();
    }

    //[MenuItem("Tools/Read file")]
    //public static void ReadString()
    //{
    //    string path = Application.persistentDataPath + "/test.txt";
    //    //Read the text from directly from the test.txt file
    //    StreamReader reader = new StreamReader(path);
    //    Debug.Log(reader.ReadToEnd());
    //    reader.Close();
    //}
}
