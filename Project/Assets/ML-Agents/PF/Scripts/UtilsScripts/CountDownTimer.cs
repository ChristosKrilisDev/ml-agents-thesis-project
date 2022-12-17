using System.Collections;
using UnityEngine;

namespace ML_Agents.PF.Scripts.UtilsScripts
{
    public class CountDownTimer
    {

        private float _timerValue;
        public bool StartTimer;

        public CountDownTimer(float timerValue, bool startTimer)
        {
            _timerValue = timerValue;
            StartTimer = startTimer;
        }


        public void TimerValueChanged(float newValue)
        {
            _timerValue = newValue;
        }

        public IEnumerator IdleMovementCountDown()
        {
            while (StartTimer)
            {
                yield return new WaitForSeconds(1f);

                if (_timerValue <= 0) continue;


                _timerValue -= 1f;
            }
        }

        public bool IsOutOfTime()
        {
            return _timerValue <= 0;
        }


    }
}
