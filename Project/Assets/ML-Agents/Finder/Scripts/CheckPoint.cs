using UnityEngine;


namespace ML_Agents.Finder.Scripts
{
    public class CheckPoint : MonoBehaviour
    {
        [SerializeField]
        private Material _onMaterial;
        [SerializeField]
        private Material _offMaterial;
        [SerializeField]
        private GameObject _myButton;

        private GameObject _goalNode = null;
        private PfArea _areaComponent;
        private bool _hasPushed;

        private Renderer Renderer
        {
            get
            {
                return _myButton.GetComponent<Renderer>();
            }
        }

        private GameObject Area
        {
            get
            {
                return transform.parent.gameObject;
            }
        }

        private BoxCollider BoxCollider
        {
            get
            {
                return gameObject.GetComponent<BoxCollider>();
            }
        }

        public bool GetState { get { return _hasPushed; } private set { } }


        private void Awake()
        {
            _areaComponent = Area.GetComponent<PfArea>();
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
            if (other.gameObject.CompareTag("agent") && !_hasPushed)
                ToggleState(false);
        }


        private void ToggleState(bool isInitState)
        {
            BoxCollider.enabled = isInitState;
            _hasPushed = !isInitState;
            _goalNode.SetActive(!isInitState);

            Renderer.material = isInitState ? _offMaterial : _onMaterial;
            tag = isInitState ? "switchOff" : "switchOn";

        }
    }
}
