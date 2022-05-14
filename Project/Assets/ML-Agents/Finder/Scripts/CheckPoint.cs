using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private Material _onMaterial;
    [SerializeField] private Material _offMaterial;
    [SerializeField] private GameObject _myButton;

    private GameObject _goalNode = null;
    private PFArea _areaComponent;
    private bool _hasPushed;

    private Renderer _renderer => _myButton.GetComponent<Renderer>();
    private GameObject _area => transform.parent.gameObject;
    private BoxCollider _boxCollider => gameObject.GetComponent<BoxCollider>();
    public bool GetState { get { return _hasPushed; } private set { } }


    private void Awake()
    {
        _areaComponent = _area.GetComponent<PFArea>();
    }

    public void Init(int cpSpawnIndex, int goalSpawnIndex)
    {
        _areaComponent.PlaceNode(this.gameObject, cpSpawnIndex);
        _goalNode = _areaComponent.CreateGoalNode(goalSpawnIndex);   //pre-create final node to get all nodes 

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
        _boxCollider.enabled = isInitState;
        _hasPushed = !isInitState;
        _goalNode.SetActive(!isInitState);

        _renderer.material = isInitState ? _offMaterial : _onMaterial;
        tag = isInitState ? "switchOff" : "switchOn";

    }
}
