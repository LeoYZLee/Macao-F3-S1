using UnityEngine;
using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA;
using HoloToolkit.Unity.InputModule;
using System;

public class PersistoMatic : MonoBehaviour, IInputClickHandler
{

    public string ObjectAnchorStoreName;
    private GameObject targetObject;
    private GameObject imageTarget;
    private GameObject CarBasePointObject;
    private GameObject ancherCube;
    private Vector3 lockDirect;
    private float lockDist;

    WorldAnchorStore anchorStore;

    bool Placing = false;
    // Use this for initialization
    void Start()
    {
        Debug.Log("WorldAnchorStore.GetAsync()");
  
        targetObject = GameObject.Find("F1Collection_Pivot").gameObject;
        imageTarget = GameObject.Find("ImageTarget").gameObject;

        lockDirect = targetObject.transform.position - imageTarget.transform.position;
        lockDist = lockDirect.magnitude;
    }

    void AnchorStoreReady(WorldAnchorStore store)
    {
        anchorStore = store;
        Placing = true;

        Debug.Log("looking for " + ObjectAnchorStoreName);
        string[] ids = anchorStore.GetAllIds();
        for (int index = 0; index < ids.Length; index++)
        {
            Debug.Log(ids[index]);
            if (ids[index] == ObjectAnchorStoreName)
            {
                WorldAnchor wa = anchorStore.Load(ids[index], targetObject);
                Placing = false;
                targetObject.transform.position = wa.transform.position;
                break;
            }
        }
    }

    void Alignment(Transform tr1P, Transform tr1C, Transform tr2P, Transform tr2C)
    {
        Vector3 v1 = tr1P.position - tr1C.position;
        Vector3 v2 = -tr2P.position;
        tr1P.rotation = Quaternion.FromToRotation(v1, v2) * tr1P.rotation;
        //tr1P.position = tr2C.position + v2.normalized * v1.magnitude;
    }

    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)   {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }
    // Update is called once per frame
    void Update()
    {
        if (Placing)
        {
            if (AppStateManager.Instance.isServerMode == false)
            {
                //gameObject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2;
                //模型定位點
                //Transform fromP = targetObject.transform.parent.transform;
                //Transform fromC =  targetObject.transform;

                //QR定位點
                //Transform toP = imageTarget.transform;
                //Vector3 toC =  new Vector3(0,0,0);
                //模型定位點 位移到 QR定位點 位移差
                //this.Alignment(fromP, fromC, toP, toP);
                //GameObject.Find("F1Collection").gameObject.transform.parent = targetObject.transform;
                targetObject.transform.position = imageTarget.transform.localPosition + lockDirect;
                targetObject.transform.rotation = Quaternion.Euler(0, imageTarget.transform.eulerAngles.y, 0);

                GameObject.Find("AncherCube").GetComponent<Renderer>().material.color = Color.yellow;

                //GameObject.Find("F1Collection").gameObject.transform.parent = targetObject.transform.parent;
                GameObject.Find("ARCamera").gameObject.SetActive(false);
            }
          

            if (AppStateManager.Instance.isServerMode)
            {
                if (CarBasePointObject == null)
                {
                    CarBasePointObject = GameObject.FindGameObjectWithTag("CarBasePoint").gameObject;
                }
                else
                {
                    CarBasePointObject.transform.position = targetObject.transform.position;
                    CarBasePointObject.transform.rotation = targetObject.transform.rotation;
                }
            }
 
        }
        else
        {
            if (GameObject.Find("ARCamera").gameObject.activeSelf && !AppStateManager.Instance.isServerMode)
            {
                targetObject.transform.position = imageTarget.transform.localPosition + lockDirect;
                targetObject.transform.rotation = Quaternion.Euler(0, imageTarget.transform.eulerAngles.y, 0);

            }

        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            OnSelect();
        }
    }

    
    void OnDrawGizmos()
    {
      
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(imageTarget.transform.position, targetObject.transform.position);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(imageTarget.transform.localPosition, imageTarget.transform.localPosition+ lockDirect);
    }
    
    void OnSelect()
    {
        Placing = !Placing;

        if (anchorStore == null)
        {
            //return;
        }

        if (Placing)
        {
            WorldAnchor attachingAnchor = targetObject.AddComponent<WorldAnchor>();
            if (attachingAnchor.isLocated)
            {
                Debug.Log("Saving persisted position immediately");
                bool saved = anchorStore.Save(ObjectAnchorStoreName, attachingAnchor);
                Debug.Log("saved: " + saved);
                GameObject.Find("AncherCube").GetComponent<Renderer>().material.color = Color.green;
            }
            else
            {
                attachingAnchor.OnTrackingChanged += AttachingAnchor_OnTrackingChanged;
            }

        }
        else
        {
            WorldAnchor anchor = imageTarget.GetComponent<WorldAnchor>();
            if (anchor != null)
            {
                DestroyImmediate(anchor);
            }

            string[] ids = anchorStore.GetAllIds();
            for (int index = 0; index < ids.Length; index++)
            {
                Debug.Log(ids[index]);
                if (ids[index] == ObjectAnchorStoreName)
                {
                    bool deleted = anchorStore.Delete(ids[index]);
                    Debug.Log("deleted: " + deleted);
                    break;
                }
            }
        }

    }

    private void AttachingAnchor_OnTrackingChanged(WorldAnchor self, bool located)
    {
        if (located && AppStateManager.Instance.isServerMode)
        {
            Debug.Log("Saving persisted position in callback");
            bool saved = anchorStore.Save(ObjectAnchorStoreName, self);
            Debug.Log("saved: " + saved);
            self.OnTrackingChanged -= AttachingAnchor_OnTrackingChanged;
        }
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        OnSelect();
    }
}
