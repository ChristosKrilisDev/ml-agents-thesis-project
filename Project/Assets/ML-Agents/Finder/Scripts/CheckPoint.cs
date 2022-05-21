using UnityEngine;


namespace ML_Agents.Finder.Scripts
{
    public class CheckPoint : MonoBehaviour
    {
        private const string SWITCH_ON_TAG = "switchOn";
        private const string SWITCH_OFF_TAG = "switchOff";

        private PathFindArea _areaComponent;
        private GameObject _goalNode;
        private Renderer _renderer;
        private BoxCollider _boxCollider;
        private GameObject _myButton;
        private Material _onMaterial;
        private Material _offMaterial;
        
        private GameObject Area => transform.parent.gameObject;
        public bool GetState { get; private set; }
        
        private void Awake()
        {
            _areaComponent = Area.GetComponent<PathFindArea>();
            _boxCollider = gameObject.GetComponent<BoxCollider>();

            _myButton = transform.GetChild(0).gameObject;
            _renderer = _myButton.GetComponent<Renderer>();
            
            _onMaterial = EpisodeHandler.OnButtonMaterial;
            _offMaterial = EpisodeHandler.OffButtonMaterial;
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
            _boxCollider.enabled = isInitState;
            GetState = !isInitState;
            _goalNode.SetActive(!isInitState);

            _renderer.material = isInitState ? _offMaterial : _onMaterial;
            tag = isInitState ? SWITCH_OFF_TAG : SWITCH_ON_TAG;

        }
    }
}
