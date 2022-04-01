using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Material onMaterial;
    public Material offMaterial;
    public GameObject myButton;
    public GameObject goalObj = null;

    private Transform nodeGoal;

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

    public void ResetSwitch(int cpSpawnAreaIndex, int goalSpawnIndex)//CHANGE
    {
        m_AreaComponent.PlaceObject(gameObject, cpSpawnAreaIndex);
        goalObj =  m_AreaComponent.CreateGoalObject(1, goalSpawnIndex);   //pre-create final goal to get all nodes 
        goalObj.gameObject.SetActive(false);
        //m_goalIndex = goalSpawnIndex;

        m_State = false;
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
            //m_AreaComponent.CreateGoalObject(1, m_goalIndex);   //create final goal
            goalObj.gameObject.SetActive(true);

            tag = "switchOn";
        }
    }
}
