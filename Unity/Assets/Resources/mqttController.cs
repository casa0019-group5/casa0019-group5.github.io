using System;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class mqttController : MonoBehaviour
{
    [Tooltip("Optional name for the controller")]
    public string nameController = "Controller 1";
    public string tag_mqttManager = ""; //to be set on the Inspector panel. It must match one of the mqttManager.cs GameObject
    [Header("   Case Sensitive!!")]
    [Tooltip("the topic to subscribe must contain this value. !!Case Sensitive!! ")]
    public string topicSubscribed = ""; //the topic to subscribe, it need to match a topic from the mqttManager
    private float pointerValue = 0.0f;
    [Space]
    [Space]

    
    public mqttManager _eventSender;

    public Dictionary<string,TextMeshProUGUI> dashboardDict=new Dictionary<string, TextMeshProUGUI>();

    public List<TextMeshProUGUI> textsDash;
    




    void Awake()
    {
        if (GameObject.FindGameObjectsWithTag(tag_mqttManager).Length > 0)
        {
            _eventSender = GameObject.FindGameObjectsWithTag(tag_mqttManager)[0].gameObject.GetComponent<mqttManager>();
            if (!_eventSender.isConnected)
            {
                _eventSender.Connect(); //Connect tha Manager when the object is spawned
            }

int counter=0;
    foreach (var item in _eventSender.topicSubscribe)
    {

         dashboardDict.Add(item,textsDash[counter]);
         counter++;
        
    }


        }
        else
        {
            Debug.LogError("At least one GameObject with mqttManager component and Tag == tag_mqttManager needs to be provided");
        }
    }

    void OnEnable()
    {
        _eventSender.OnMessageArrived += OnMessageArrivedHandler;

    }

    private void OnDisable()
    {
        _eventSender.OnMessageArrived -= OnMessageArrivedHandler;
    }

    private void OnMessageArrivedHandler(mqttObj mqttObject) //the mqttObj is defined in the mqttManager.cs
    {

       var tempText=  dashboardDict[mqttObject.topic];
       tempText.text=mqttObject.msg;


        }
       
        



    }