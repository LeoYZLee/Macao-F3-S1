using UnityEngine;
using HoloToolkit.Unity.InputModule;
using Leo.HoloToolkitExtensions;

public class MenuCommandExecuter : MonoBehaviour
{
    public bool IsActive = false;

    private AudioSource _sound;

    void Start()
    {
        _sound = GetComponent<AudioSource>();
    }

    public void Move()
    {
        TryChangeMode(ManipulationMode.Move);
    }

    public void Rotate()
    {
        TryChangeMode(ManipulationMode.Rotate);
    }

    public void Scale()
    {
        TryChangeMode(ManipulationMode.Scale);
    }
    public void Scale_1_1()
    {
        TryChangeMode(ManipulationMode.Scale_1_1);
    }
    public void Scale_1_100()
    {
        TryChangeMode(ManipulationMode.Scale_1_100);
    }

    public void Walk()
    {
        TryChangeMode(ManipulationMode.Walk);
    }

    public void Done()
    {
        TryChangeMode(ManipulationMode.None);
    }

    public void Faster()
    {
        TryChangeSpeed(true);
    }

    public void Slower()
    {
        TryChangeSpeed(false);
    }

    private void TryChangeMode(ManipulationMode mode)
    {
        var manipulator = GetSpatialManipulator();
        if (manipulator == null)
        {
            return;
        }

        if (manipulator.Mode != mode)
        {
            manipulator.Mode = mode;
            TryPlaySound();
        }

        if (mode == ManipulationMode.Scale_1_1)
        {
            changeScale(100.0f);
            BaseSpatialMappingCollisionDetector CollisonDetector;
            CollisonDetector =  gameObject.GetComponent<DefaultMappingCollisionDetector>();
            Vector3 maxDetial = CollisonDetector.GetMaxDelta(Vector3.zero);
            if (CollisonDetector.CheckIfCanMoveBy(maxDetial))
            {
                transform.localPosition += maxDetial;
            }
        }
        

        if (mode == ManipulationMode.Scale_1_100)
        {
            changeScale(1.0f);
        }

    }

    private void changeScale(float scale)
    {
        var lastSelectedObject = AppStateManager.Instance.SelectedGameObject;
        if (lastSelectedObject == null)
        {
            Debug.Log("No selected element found");
            return;
        }
        //物体的实际尺寸=原始尺寸*缩放比例
        Vector3 orgSize = Vector3.one;//lastSelectedObject.GetComponent<BoxCollider>().bounds.size;

        lastSelectedObject.transform.localScale = orgSize * scale;
    }

    private void TryChangeSpeed(bool faster)
    {
        var manipulator = GetSpatialManipulator();
        if (manipulator == null)
        {
            return;
        }

        if (manipulator.Mode == ManipulationMode.None)
        {
            return;
        }

        if (faster)
        {
            manipulator.Faster();
        }
        else
        {
            manipulator.Slower();

        }
        TryPlaySound();

    }

    private void TryPlaySound()
    {
        if (_sound != null && _sound.clip != null)
        {
            _sound.Play();
        }
    }


    private SpatialManipulator GetSpatialManipulator()
    {
        var lastSelectedObject = AppStateManager.Instance.SelectedGameObject;
        if (lastSelectedObject == null)
        {
            Debug.Log("No selected element found");
            return null;
        }
        var manipulator = lastSelectedObject.GetComponent<SpatialManipulator>();
        if (manipulator == null)
        {
            manipulator = lastSelectedObject.GetComponentInChildren<SpatialManipulator>();
        }

        if (manipulator == null)
        {
            Debug.Log("No manipulator component found");
        }
        return manipulator;
    }
}
