using HoloToolkit.Unity.InputModule;
using System;
using UnityEngine;
using Leo.HoloToolkitExtensions;

namespace Leo.HoloToolkitExtensions
{
    public class TapToSelect : MonoBehaviour, IInputClickHandler
    {
        private bool isCanMove;
        Vector3 hitPosition;

        public void OnInputClicked(InputClickedEventData eventData)
         {
            if (BaseAppStateManager.IsInitialized)
            {
                // If not already selected - select, otherwise, deselect
                if (BaseAppStateManager.Instance.SelectedGameObject != gameObject)
                {
                    BaseAppStateManager.Instance.SelectedGameObject = gameObject;
                    isCanMove = false;
                }
                else
                {
                    //Walk模式
                    if (GetMode().Mode == ManipulationMode.Walk && AppStateManager.Instance.SelectedGameObject != null)
                    {
                        if (GazeManager.Instance.IsGazingAtObject)
                        {
                            hitPosition = GazeManager.Instance.HitPosition;
                            isCanMove = true;
                        }

                        return;
                    }
                    isCanMove = false;
                    BaseAppStateManager.Instance.SelectedGameObject = null;
                }
                var audioSource = GetAudioSource(gameObject);
                if (audioSource != null)
                {
                    audioSource.Play();
                }
            }
            else
            {
                Debug.Log("No BaseAppStateManager found or initialized");
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

        private AudioSource GetAudioSource(GameObject obj)
        {
            var audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.GetComponentInParent<AudioSource>();
            }
            return audioSource;
        }

        private SpatialManipulator GetMode()
        {
            var manipulator = this.GetSpatialManipulator();
            return manipulator;
        }

        void Update()
        {
            if (isCanMove)
            {
                Camera.main.transform.position = Vector3.MoveTowards( new Vector3(hitPosition.x, 0 , hitPosition.z), Camera.main.transform.position, Time.deltaTime * 0.5f);
                isCanMove = false;
            }
        }


    }
}
