using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using Unity.MLAgents.Policies;

namespace Unity.MLAgents.Tests
{
    [TestFixture]
    public class TestSerialization
    {
        private const string k_oldPrefabPath = "Packages/com.unity.ml-agents/Tests/Editor/TestModels/old_serialized_agent.prefab";
        private const int k_numVecObs = 212;
        private const int k_numContinuousActions = 39;

        [Test]
        public void TestDeserialization()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(k_oldPrefabPath);
            var agent = Object.Instantiate(prefab);
            var bp = agent.GetComponent<BehaviorParameters>();
            Assert.AreEqual(bp.BrainParameters.ActionSpec.NumContinuousActions, k_numContinuousActions);
            Assert.AreEqual(bp.BrainParameters.VectorObservationSize, k_numVecObs);
        }

    }
}
