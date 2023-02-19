using System;
using System.Collections.Generic;
using ML_Agents.PF.Scripts.UtilsScripts;
using UnityEngine;

namespace ML_Agents.PF.Scripts.RL
{
    public class PathRewardManager : MonoBehaviour
    {
        public bool DisableSpawning;

        [SerializeField, Range(0,10)] private int _spawnAmount = 3;

        private GameObject _rewardNodePref;

        private List<GameObject> _spawnedRewards = new List<GameObject>();

        private void Awake()
        {
            _rewardNodePref = Utils.RewardNode;

            // for (int i = 0; i <= _spawnAmount; i++)
            // {
            //     var obj =Instantiate(_rewardNodePref);
            //     obj.SetActive(false);
            //     _spawnedRewards.Add(obj);
            // }
        }


        public void SpawnRewards(Transform startPos, Transform endPos) //todo : add pull pattern
        {

            var endPosVector = Vector3.one +  endPos.localPosition;
            var startPosVector = Vector3.one +  startPos.localPosition;
            endPosVector.y = 0;
            startPosVector.y = 0;

            var distance = Vector3.Distance(endPosVector,  startPosVector);
            var dir = (endPosVector - startPosVector).normalized;
            var fragment = distance / _spawnAmount;

            Debug.Log($"start pos {startPos.name} || end pos {endPos.name} || distance : {distance} \n|| fragments => {fragment} || {dir} => direction");
            Debug.Log(" -------------- ");

            var listOfPints = new List<Vector3>();

            var newPos = dir * fragment;

            for (int i = 0; i < _spawnAmount; i++)
            {
                listOfPints.Add((newPos));
                newPos += dir * fragment;
                Debug.Log(" V : "+listOfPints[i]);
            }

            if(listOfPints.Count <= 0) return;

            foreach (var pos in listOfPints)
            {
                var obj =Instantiate(_rewardNodePref, startPos);
                obj.transform.localPosition = pos;
                obj.SetActive(true);
                _spawnedRewards.Add(obj);
            }

        }

        public void Reset()
        {
            foreach (var obj in _spawnedRewards)
            {
                Destroy(obj);
            }
        }

    }
}
