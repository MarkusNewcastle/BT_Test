using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Panda;

public class UnitTasks : MonoBehaviour {

    // Testchanges

    public Transform target;
    public Transform eyes;

    public Transform DistractionPoint_1;
    public Transform DistractionPoint_2;
    public Transform DistractionPoint_3;
    public Transform DistractionPoint_4;


    public Door currentDoor;


    public int randomValue;
    public int pointCount;

    [Header("NavPoints")]
    [SerializeField]
    private int nextNavPoint = 0;
    private List<GameObject> rawNP;
    private List<NavPoint> navPointList = new List<NavPoint>();

    private NavMeshAgent myAgent;

    private void Start() {

        myAgent = GetComponent<NavMeshAgent>();
        GetNavPoints();

        pointCount = 0;
        randomValue = Random.Range(2, 5);
    }

    private void Update() {
        //Debug.Log(DistractionBool);
    }

    #region GetNavPoints
    private void GetNavPoints() {

        rawNP = GameObject.FindGameObjectsWithTag(Tags.navPoint).OrderBy(go => go.name).ToList();

        for (int i = 0; i < rawNP.Count; i++) {

            if (rawNP[i].GetComponent<NavPoint>()) {

                navPointList.Add(rawNP[i].GetComponent<NavPoint>());
            }
        }
    }
    #endregion

    #region Patrol

    [Task]
    public void StartPatrol() {

        if (navPointList.Count == 0) {
            Debug.Log("Keine navPoints Gefunden");
            Task.current.Fail();
        }

        else {
            target = navPointList[nextNavPoint].transform;
            Task.current.Succeed();
        }
    }

    [Task]
    public void SetNextWaypoint() {

        Debug.Log("Set next waypoint");
        nextNavPoint = (nextNavPoint + 1) % navPointList.Count;
        Task.current.Succeed();
    }
    #endregion

    [Task]
    public void SetDestination() {
        myAgent.destination = target.transform.position;
        Task.current.Succeed();
    }

    [Task]
    public void MoveToDestination() {
        if (myAgent.remainingDistance <= myAgent.stoppingDistance && !myAgent.pathPending) {
            pointCount++;
            Task.current.Succeed();
        }
    }

    #region Door

    [Task]
    public bool SeeDoor() {
        RaycastHit hit;
        Debug.DrawRay(eyes.transform.position, eyes.transform.forward * 1f, Color.magenta);
        Vector3 fwd = eyes.transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(eyes.transform.position, fwd, out hit, 1f)) {

            if (hit.collider.CompareTag(Tags.door)) {
                Debug.Log("There is a door in my way");
                currentDoor = hit.collider.GetComponent<Door>();
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    }

    [Task]
    public void SetDoorAsDestination() {

        if (currentDoor != null) {
            myAgent.destination = currentDoor.interActionPoint.transform.position;
            Task.current.Succeed();
        }
        else {
            Task.current.Fail();
        }
    }

    [Task]
    public void MoveToDoorDestination() {

        if (Task.isInspected) {
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);
        }

        if (myAgent.remainingDistance <= myAgent.stoppingDistance && !myAgent.pathPending) {
            Task.current.Succeed();
        }
    }

    [Task]
    public void FaceDoor() {
        Quaternion rotationInteractable = currentDoor.interActionPoint.transform.rotation;
        rotationInteractable.x = 0;
        rotationInteractable.z = 0;

        transform.rotation = Quaternion.Lerp(transform.rotation, rotationInteractable, 5f * Time.deltaTime);
        if ((rotationInteractable.eulerAngles - transform.rotation.eulerAngles).sqrMagnitude < .01) {
            Task.current.Succeed();
        }
    }

    [Task]
    public void OpenDoor() {

        if (currentDoor == null) {
            Task.current.Fail();
        }
        else {
            currentDoor.OpenDoor();
            Task.current.Succeed();
        }
    }
    #endregion

    [Task]
    public bool DistractionBool;


    [Task]
    public void IsThereADistraction() {

        if (randomValue == pointCount) {

            pointCount = 0;

            bool distraction = (Random.value > 0.5f);

            if (distraction) {
                DistractionBool = true; ;
            }
  
        }
        Task.current.Succeed();
    }


    [Task]
    public void ResetDistractionBool() {
        DistractionBool = false;
        //CheckTargetReached = false;
        Task.current.Succeed();
    }

    [Task]
    public void DistractionPoint1() {
        target = DistractionPoint_1;
        Task.current.Succeed();
    }


    [Task]
    public void DistractionPoint2() {
        target = DistractionPoint_2;
        Task.current.Succeed();
    }

    [Task]
    public void DistractionPoint3() {
        target = DistractionPoint_3;
        Task.current.Succeed();
    }
    [Task]
    public void DistractionPoint4() {
        target = DistractionPoint_4;
        Task.current.Succeed();
    }


} // class
