// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using HoloToolkit.Unity.InputModule.Tests;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using HoloToolkit.Sharing;

namespace Leo.HoloToolkitExtensions
{
    public class HeadManager : MonoBehaviour
    {
        private bool isServerMode;

        //[SerializeField]
        private TestButton button = null;

        //[SerializeField]
        private ManipulationMode ButtonAction = ManipulationMode.None;


        [System.Serializable]
        public struct ButtonActionResponse
        {
           // [Tooltip("The KeyCode to recognize.")]
           // public KeyCode KeyCode;
            [Tooltip("The UnityEvent to be invoked when the keyword is recognized.")]
            public UnityEvent Response;
        }

        [Tooltip("An array of string keywords and UnityEvents, to be set in the Inspector.")]
        public ButtonActionResponse[] ButtonActionsResponse;

        private readonly Dictionary<int, UnityEvent> responses = new Dictionary<int, UnityEvent>();
        //private enum buttonAction { Move, Reset, Grow, Shrink };

        //private Vector3 InitialScale;

        private bool isHeadCreat = false;
        private GameObject HeadObject;
        private GameObject imageTarget;
        private GameObject CarBasePointObject;
        private GameObject localCarBasePointObject;
        public SharingStage shareStage;

        private void Start()
        {
            isServerMode = AppStateManager.Instance.isServerMode;
            int keywordCount = ButtonActionsResponse.Length;
            if (keywordCount > 0)
            {
                try
                {
                    string[] keywords = new string[keywordCount];
                    // Convert the struct array into a dictionary, with the keywords and the keys and the methods as the values.
                    // This helps easily link the keyword recognized to the UnityEvent to be invoked.
                    for (int index = 0; index < keywordCount; index++)
                    {
                        ButtonActionResponse buttonActionResponse = ButtonActionsResponse[index];
                        responses[index] = buttonActionResponse.Response;
 
                    }


                }
                catch (ArgumentException)
                {
                    Debug.LogError("Duplicate ButtonActionsResponse specified in the Inspector on " + gameObject.name + ".");
                }
            }
            else
            {
                Debug.LogError("Must have at least one ButtonActionsResponse specified in the Inspector on " + gameObject.name + ".");
            }

             shareStage = GameObject.Find("Sharing").GetComponent<SharingStage>();
            localCarBasePointObject = GameObject.Find("F1Collection_Pivot").gameObject;
            imageTarget = GameObject.Find("ImageTarget").gameObject;
        }

        void FixedUpdate()
        {
            if ( isHeadCreat == false && !isServerMode)
            {
                if (SharingStage.Instance.IsConnected)
                {
                    ProcessKeyBindings();
                    isHeadCreat = true;

                }else
                {
                    isHeadCreat = false;
                }
               

            }

           
        }

        private void Update()
        {
            //hololens
            if (isHeadCreat  && !isServerMode)
            {
                if (HeadObject == null)
                {
                    HeadObject = GameObject.FindGameObjectWithTag("Player").gameObject;
                }
                else
                {
                    HeadObject.transform.position = Camera.main.transform.localPosition;
                    HeadObject.transform.rotation = Camera.main.transform.localRotation;
                }

                if (CarBasePointObject == null)
                {
                    CarBasePointObject = GameObject.FindGameObjectWithTag("CarBasePoint").gameObject;
                }
                else
                {
                    CarBasePointObject.transform.position = localCarBasePointObject.transform.position;
                    CarBasePointObject.transform.rotation = localCarBasePointObject.transform.rotation;
                }

            }
            //
            //server
            if (isServerMode)
            {
                if (HeadObject == null)
                {
                    HeadObject = GameObject.FindGameObjectWithTag("Player").gameObject;
                }
                else
                {
                   Camera.main.transform.position = HeadObject.transform.position;
                   Camera.main.transform.rotation = HeadObject.transform.rotation ;

                   // if (Camera.main.transform.parent != HeadObject.transform)
                   // {
                   //     Camera.main.transform.parent = HeadObject.transform;
                   // }
                }

            }

            if (isServerMode)
            {
                if (CarBasePointObject == null)
                {
                    CarBasePointObject = GameObject.FindGameObjectWithTag("CarBasePoint").gameObject;
                }
                else
                {
                    localCarBasePointObject.transform.position = CarBasePointObject.transform.position;
                    localCarBasePointObject.transform.rotation = CarBasePointObject.transform.rotation;
                }
            }
        }

        private void ProcessKeyBindings()
        {
            foreach (var kvp in ButtonActionsResponse)
            {
               // if (Input.GetKeyDown(kvp.KeyCode) || isHeadCreat ==false )
               // {
                    kvp.Response.Invoke();
 
                    return;
               // }
            }
        }


        private void OnEnable()
        {
            button.Activated += OnButtonPressed;
        }

        private void OnDisable()
        {
            button.Activated -= OnButtonPressed;
        }

        private void OnButtonPressed(TestButton source)
        {
            ButtonActionResponse buttonActionResponse;

                    buttonActionResponse = ButtonActionsResponse[0];
                    buttonActionResponse.Response.Invoke();
 

            button.Selected = false;
        }
    }


}