using NUnit.Framework;
using Unity.MLAgents.Sensors;

namespace Unity.MLAgents.Tests
{
    public static class SensorTestHelper
    {
        public static void CompareObservation(ISensor sensor, float[] expected)
        {
            string errorMessage;
            var isOK = SensorHelper.CompareObservation(sensor, expected, out errorMessage);
            Assert.IsTrue(isOK, errorMessage);
        }

        public static void CompareObservation(ISensor sensor, float[,,] expected)
        {
            string errorMessage;
            var isOK = SensorHelper.CompareObservation(sensor, expected, out errorMessage);
            Assert.IsTrue(isOK, errorMessage);
        }
    }
}
