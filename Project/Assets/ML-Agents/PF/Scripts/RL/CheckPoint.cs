using ML_Agents.PF.Scripts.Data;
using ML_Agents.PF.Scripts.UtilsScripts;
using UnityEngine;

namespace ML_Agents.PF.Scripts.RL
{
    public class CheckPoint : MonoBehaviour
    {
        private PathFindArea _areaComponent;
        private GameObject _goalNode;
        private Renderer _renderer;
        private BoxCollider _boxCollider;
        private GameObject _myButton;
        private Material _onMaterial;
        private Material _offMaterial;

        private GameObject Area => transform.parent.gameObject;
        public bool HasPressed { get; private set; }

        private void Awake()
        {
            _areaComponent = Area.GetComponent<PathFindArea>();
            _boxCollider = gameObject.GetComponent<BoxCollider>();

            _myButton = transform.GetChild(0).gameObject;
            _renderer = _myButton.GetComponent<Renderer>();

            _onMaterial = Utils.OnButtonMaterial;
            _offMaterial = Utils.OffButtonMaterial;
        }

        public void Init(int cpSpawnIndex, int goalSpawnIndex)
        {
            _areaComponent.PlaceNode(gameObject, cpSpawnIndex);
            _goalNode = _areaComponent.CreateGoalNode(goalSpawnIndex); //pre-create final node to get all nodes

            var pos = transform.localPosition;
            transform.localPosition = new Vector3(pos.x , 0 , pos.z);

            transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            ToggleState(true);
        }

        private void OnCollisionEnter(Collision other)
        {
            if((int)GameManager.Instance.PhaseType <= 2) return;

            if (other.gameObject.CompareTag(TagData.AGENT_TAG) && !HasPressed)
                ToggleState(false);
        }

        private void ToggleState(bool isInitState)
        {
            _boxCollider.enabled = isInitState;
            HasPressed = !isInitState;
            _goalNode.SetActive(!isInitState);

            _renderer.material = isInitState ? _offMaterial : _onMaterial;
            tag = isInitState ? TagData.SWITCH_OFF_TAG : TagData.SWITCH_ON_TAG;
        }
    }
}
