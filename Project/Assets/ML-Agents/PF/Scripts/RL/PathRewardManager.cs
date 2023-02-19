using System;
using System.Collections.Generic;
using ML_Agents.PF.Scripts.UtilsScripts;
using UnityEngine;

namespace ML_Agents.PF.Scripts.RL
{
    public class PathRewardManager : MonoBehaviour
    {
        public bool DisableSpawning;

        [SerializeField, Range(0,10)] private int _spawnAmount = 1;

        private GameObject _rewardNodePref;

        private void Awake()
        {
            _rewardNodePref = Utils.RewardNode;
        }


        public void SpawnRewards(Transform startPos, Transform endPos) //todo : add pull pattern
        {

            var endPosVector = Vector3.one +  endPos.localPosition;
            var startPosVector = Vector3.one +  startPos.localPosition;
            endPosVector.y = 0;
            startPosVector.y = 0;

            var distance = Vector3.Distance(endPosVector,  startPosVector);
            var dir = endPosVector - startPosVector;

            Debug.Log($"{startPos} start pos - {endPos} end pos  = {distance} = framgents => {distance/_spawnAmount} *** {dir.normalized} direction");

            var listOfPints = new List<Vector3>();
            Debug.Log(" -------------- ");

            for (int i = 0; i < _spawnAmount; i++)
            {
                listOfPints.Add(dir * distance);
                Debug.Log(" V : "+listOfPints[i]);
            }

            if(listOfPints.Count <= 0) return;

            foreach (var pos in listOfPints)
            {
                var obj =Instantiate(_rewardNodePref);
                obj.transform.localPosition = pos;
            }

        }
    }
}
