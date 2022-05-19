using UnityEngine;


namespace ML_Agents.Finder.Scripts
{
    public class CheckPoint : MonoBehaviour
    {
        [SerializeField] private Material _onMaterial;
        [SerializeField] private Material _offMaterial;
        [SerializeField] private GameObject _myButton;

        private GameObject _goalNode = null;
        private PFArea _areaComponent;

        private const string SWITCH_OFF_TAG = "switchOff";
        private const string SWITCH_ON_TAG = "switchOn";

        
        private Renderer Renderer => _myButton.GetComponent<Renderer>();
        private GameObject Area => transform.parent.gameObject;
        private BoxCollider BoxCollider => gameObject.GetComponent<BoxCollider>();
        
        public bool GetState { get; private set; }


        private void Awake()
        {
            _areaComponent = Area.GetComponent<PFArea>();
        }

        public void Init(int cpSpawnIndex, int goalSpawnIndex)
        {
            _areaComponent.PlaceNode(gameObject, cpSpawnIndex);
            _goalNode = _areaComponent.CreateGoalNode(goalSpawnIndex); //pre-create final node to get all nodes

            transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            ToggleState(true);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("agent") && !GetState)
                ToggleState(false);
        }


        private void ToggleState(bool isInitState)
        {
            BoxCollider.enabled = isInitState;
            GetState = !isInitState;
            _goalNode.SetActive(!isInitState);

            Renderer.material = isInitState ? _offMaterial : _onMaterial;
            tag = isInitState ? SWITCH_OFF_TAG : SWITCH_ON_TAG;

        }
    }
}
