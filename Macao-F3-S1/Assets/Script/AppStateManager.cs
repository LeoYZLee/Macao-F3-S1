using HoloToolkit.Unity.InputModule;
using Leo.HoloToolkitExtensions;
using UnityEngine;

public class AppStateManager : BaseAppStateManager, IManipulationHandler
{
    public bool isServerMode;
    //Hololens Disable Object
    [SerializeField]
    private GameObject Disable_Hololens_Car_Base_Road_layer;
    [SerializeField]
    private GameObject Disable_Hololens_360_layer;

    void Start()
    {
        isServerMode = false;
#if UNITY_EDITOR
        isServerMode = true;
#endif

        InputManager.Instance.AddGlobalListener(gameObject);

        if (!isServerMode)
        {
            if (Disable_Hololens_360_layer != null) { Disable_Hololens_360_layer.SetActive(false); }
            if (Disable_Hololens_Car_Base_Road_layer != null) { Disable_Hololens_Car_Base_Road_layer.SetActive(false); }
        }
        
        
    }

    public static new AppStateManager Instance
    {
        get { return (AppStateManager)BaseAppStateManager.Instance; }
    }

    protected override void ResetDeselectedObject(GameObject oldGameObject)
    {
        var manipulator = GetManipulator(oldGameObject);
        if (manipulator != null)
        {
            manipulator.Mode = ManipulationMode.None;
        }
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        if (SelectedGameObject != null)
        {
            var manipulator = GetManipulator(SelectedGameObject);
            if (manipulator != null)
            {
                manipulator.Manipulate(eventData.CumulativeDelta);
            }
        }
    }

    public void OnManipulationStarted(ManipulationEventData eventData)
    {

    }

    public void OnManipulationCompleted(ManipulationEventData eventData)
    {
    }

    public void OnManipulationCanceled(ManipulationEventData eventData)
    {
    }

    protected SpatialManipulator GetManipulator(GameObject obj)
    {
        if (obj == null)
        {
            return null;
        }
        var manipulator = obj.GetComponent<SpatialManipulator>() ??
            obj.GetComponentInChildren<SpatialManipulator>();
        return manipulator;
    }
}