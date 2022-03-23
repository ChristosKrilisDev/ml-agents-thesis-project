using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Material onMaterial;
    public Material offMaterial;
    public GameObject myButton;
    bool m_State;
    GameObject m_Area;
    PFArea m_AreaComponent;
    int m_goalIndex;

    public bool GetState()
    {
        return m_State;
    }

    void Start()
    {
        m_Area = gameObject.transform.parent.gameObject;
        m_AreaComponent = m_Area.GetComponent<PFArea>();
    }

    public void ResetSwitch(int cpSpawnAreaIndex, int goalSpawnIndex)
    {
        m_AreaComponent.PlaceObject(gameObject, cpSpawnAreaIndex);
        m_State = false;
        m_goalIndex = goalSpawnIndex;
        tag = "switchOff";
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        myButton.GetComponent<Renderer>().material = offMaterial;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("agent") && m_State == false)
        {
            myButton.GetComponent<Renderer>().material = onMaterial;
            m_State = true;
            m_AreaComponent.CreateGoalObject(1, m_goalIndex);   //create final goal
            tag = "switchOn";
        }
    }
}
