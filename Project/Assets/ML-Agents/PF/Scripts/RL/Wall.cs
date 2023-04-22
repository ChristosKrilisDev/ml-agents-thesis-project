using System;
using UnityEngine;
using Random = UnityEngine.Random;
namespace ML_Agents.PF.Scripts.RL
{
    public class Wall : MonoBehaviour
    {

        private enum Anchor
        {
            Left,
            Right,
            Up,
            Down
        }

        [SerializeField] private Anchor _anchor;
        [SerializeField] private bool _randomize = true;

        private const int MAX_WIDTH = 20;
        private const int INITIAL_WIDTH = 8;

        private Transform _initialTransform;

        private void Start()
        {
            _initialTransform = transform;
        }

        public void RandomizeSize()
        {
            if (!_randomize) return;

            var initialScale = transform.localScale;

            var randomWidth = Random.Range(INITIAL_WIDTH, MAX_WIDTH);
            var newRandomScale = new Vector3(initialScale.x, initialScale.y, randomWidth);
            transform.localScale = newRandomScale;


            var diff = (initialScale.z - transform.localScale.z)/2;
            var initialLocalPosition = _initialTransform.localPosition;

            transform.localPosition = _anchor switch
            {
                Anchor.Left => new Vector3(initialLocalPosition.x - diff, initialLocalPosition.y, initialLocalPosition.z),
                Anchor.Right => new Vector3(initialLocalPosition.x + diff, initialLocalPosition.y, initialLocalPosition.z),
                Anchor.Up => new Vector3(initialLocalPosition.x, initialLocalPosition.y, initialLocalPosition.z + diff),
                Anchor.Down => new Vector3(initialLocalPosition.x, initialLocalPosition.y, initialLocalPosition.z - diff),
                _ => throw new ArgumentOutOfRangeException()
            };

        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
