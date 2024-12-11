/*
The MIT License (MIT)
Copyright (c) 2018 Giovanni Paolo Vigano'
****
Modified by Valerio Signorelli, UCL Connected Environments 2021
Subscribed to list of topics
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
****
Modified by Valerio Signorelli, UCL Connected Environments 2021
# Add Subscribed to list of topics
*/

using System.Collections.Generic;
using UnityEngine;
using M2MqttUnity;
using uPLibrary.Networking.M2Mqtt.Messages;

//the mqttObj is use to store the message received and topic subscribed, so we can select the right object from the controller
//C# Property GET/SET and event listener is used to reduce Update overhead in the controlled objects
public class mqttObj{
    private string m_msg;
    public string msg
    {
        get{return m_msg;}
        set{
            if (m_msg == value) return;
            m_msg = value;
        }
    }
    private string m_topic;
    public string topic
    {
        get
        {
            return m_topic;
        }
        set
        {
            if (m_topic == value) return;
            m_topic = value;
        }
    }
}


public class mqttManager : M2MqttUnityClient
{
    [Header("MQTT topics")]
    [Tooltip("Set the topic to subscribe. !!!ATTENTION!!! multi-level wildcard # subscribes to all topics")]
    //public string topicSubscribe = "#"; // topic to subscribe. !!! The multi-level wildcard # is used to subscribe to all the topics. Attention i if #, subscribe to all topics. Attention if MQTT is on data plan
    public List<string> topicSubscribe = new List<string>(); //list of topics to subscribe

    [Tooltip("Set the topic to publish (optional)")]
    public string topicPublish = ""; // topic to publish

    public string messagePublish = ""; // message to publish

    [Tooltip("Set this to true to perform a testing cycle automatically on startup")]
    public bool autoTest = false;

     mqttObj mqttObject = new mqttObj();

    public event OnMessageArrivedDelegate OnMessageArrived;
    public delegate void OnMessageArrivedDelegate(mqttObj mqttObject);

    //using C# Property GET/SET and event listener to expose the connection status
    private bool m_isConnected;


    public bool isConnected
    {
        get
        {
            return m_isConnected;
        }
        set
        {
            if (m_isConnected == value) return;
            m_isConnected = value;
            if (OnConnectionSucceeded != null)
            {
                OnConnectionSucceeded(isConnected);
            }
        }
    }
    public event OnConnectionSucceededDelegate OnConnectionSucceeded;
    public delegate void OnConnectionSucceededDelegate(bool isConnected);

     // a list to store the mqttObj received
    private List<mqttObj> eventMessages = new List<mqttObj>();

    public void Publish()
    {
        client.Publish(topicPublish, System.Text.Encoding.UTF8.GetBytes(messagePublish), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        Debug.Log("Test message published");
    }
    public void SetEncrypted(bool isEncrypted)
    {
        this.isEncrypted = isEncrypted;
    }

    protected override void OnConnecting()
    {
        base.OnConnecting();
    }

    protected override void OnConnected()
    {
        base.OnConnected();
        isConnected = true;

        if (autoTest)
        {
            Publish();
        }
    }

    protected override void OnConnectionFailed(string errorMessage)
    {
        Debug.Log("CONNECTION FAILED! " + errorMessage);
    }

    protected override void OnDisconnected()
    {
        Debug.Log("Disconnected.");
        isConnected = false;
    }

    protected override void OnConnectionLost()
    {
        Debug.Log("CONNECTION LOST!");
    }

    protected override void SubscribeTopics()
    {
        foreach (string item in topicSubscribe) //subscribe to all the topics of the Public List topicSubscribe, not most efficient way (e.g. JSON object works better), but it might be useful in certain circumstances 
        {
         client.Subscribe(new string[] { item }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });   
        }
        
    }

    protected override void UnsubscribeTopics()
    {
        foreach (string item in topicSubscribe)
        {
            client.Unsubscribe(new string[] { item });
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void DecodeMessage(string topicReceived, byte[] message)
    {
        //The message is decoded and stored into the mqttObj (defined at the lines 40-63)
        
        mqttObject.msg = System.Text.Encoding.UTF8.GetString(message);
        mqttObject.topic=topicReceived;

        Debug.Log("Received: " + mqttObject.msg + "from topic: " + mqttObject.topic);

        StoreMessage(mqttObject);
        
        if(OnMessageArrived !=null){
        OnMessageArrived(mqttObject);
        }

        
    }

    private void StoreMessage(mqttObj eventMsg)
    {
        if (eventMessages.Count > 50)
        {
            eventMessages.Clear();
        }
        eventMessages.Add(eventMsg);
    }

    protected override void Update()
    {
        base.Update(); // call ProcessMqttEvents()

    }

    private void OnDestroy()
    {
        Disconnect();
    }

    private void OnValidate()
    {
        if (autoTest)
        {
            autoConnect = true;
        }
    }


}