using System.IO;
namespace ML_Agents.PF.Scripts
{
    public class TextFileHandler
    {
        private const string FILE_PATH = "D:/RL_Vs_Dijktra_Thesis_Project/Thesis/Project/Assets/Resources/RL_DATA.txt";
        private const string HEADER = "Episode , agent distance , dijkstra distance ,  goal , avr rewards";

        public TextFileHandler(string index)
        {
            var writer = new StreamWriter(FILE_PATH, true);
            writer.WriteLine("\n----------\nTake : " + index + "\t" + HEADER + "\n----------\n");
            writer.Close();
        }

        public static void WriteString(float episodeCounter, float agentDistance, float dijkstraDistance, bool hasFindTarget, float avrRewards)
        {
            var formattedText = GetFormattedString(episodeCounter, agentDistance, dijkstraDistance, hasFindTarget, avrRewards);
            var writer = new StreamWriter(FILE_PATH, true);
            writer.WriteLine(formattedText);
            writer.Close();
        }

        private static string GetFormattedString(float episodeCounter, float agentDistance, float dijkstraDistance, bool hasFindTarget, float avrRewards)
        {
            return $"{string.Join(", ", episodeCounter, agentDistance, dijkstraDistance, hasFindTarget, avrRewards)}";
        }
    }
}
