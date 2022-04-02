using UnityEngine;
using System.IO;
using UnityEditor;

public class TextFileHandler
{
    const string m_path = "D:/RL_Vs_Dijktra_Thesis_Project/Thesis/Project/Assets/Resources/RL_DATA.txt";
    const string m_Header = "Episode , agent distance , dijkstra distance ,  goal , avr rewards";


    public TextFileHandler(string index)
    {
        StreamWriter writer = new StreamWriter(m_path, true);
        writer.WriteLine("\n\nTake : " + index + "\t" + m_Header + "\n\n");
        writer.Close();
    }


    //[MenuItem("Tools/Write file")]
    public void WriteString(float episodeCounter, float agentDistance, float dijkstraDistance, bool hasFindTarget, float avrRewards)
    {

        string formatedText = getFormatedString(episodeCounter, agentDistance, dijkstraDistance, hasFindTarget, avrRewards);

        //Write some text to the .txt file
        StreamWriter writer = new StreamWriter(m_path, true);
        writer.WriteLine(formatedText);
        writer.Close();

    }


    private string getFormatedString(float episodeCounter, float agentDistance, float dijkstraDistance, bool hasFindTarget, float avrRewards)
    {
        return string.Format("{0}",
            string.Join(", ",
            episodeCounter,
            agentDistance,
            dijkstraDistance,
            hasFindTarget,
            avrRewards));
    }

    //string UnitTest()
    //{
    //    string s = getString(1, "134", "45", true, 2);
    //    return s;
    //}

}
