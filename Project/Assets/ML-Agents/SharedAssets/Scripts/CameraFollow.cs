using UnityEngine;

namespace Unity.MLAgentsExamples
{
    public class CameraFollow : MonoBehaviour
    {
        [Tooltip("The target to follow")] public Transform target;

        [Tooltip("The time it takes to move to the new position")]
        public float SmoothingTime = 0.005f; //The time it takes to move to the new position

        [SerializeField]
        private Vector3 _mCamVelocity; //Camera's velocity (used by SmoothDamp)


        private void FixedUpdate()
        {
            var newPosition = new Vector3(target.position.x, transform.position.y, target.position.z);

            gameObject.transform.position =
                Vector3.SmoothDamp(transform.position, newPosition, ref _mCamVelocity, SmoothingTime, Mathf.Infinity, Time.fixedDeltaTime);
        }
    }
}
