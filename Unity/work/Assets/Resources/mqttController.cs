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

    public RingChart ringChart;
    public mqttManager _eventSender;
    public TextMeshProUGUI deviceName;
    public TextMeshProUGUI todayEnergy;
    public TextMeshProUGUI yesterdayEnergy;
    public TextMeshProUGUI totalEnergy;
    public TextMeshProUGUI since;
    public Image toggle;
    public Color color1 = new Color(0, 1, 0); // Bright Green
    public Color color2 = new Color(0, 0.5f, 0); // Darker Green
    public float blinkDuration = 1.0f; // Duration of one blink cycle
    public int blinkCount = 3; // Number of blinks


    void Awake()
    {
        if (GameObject.FindGameObjectsWithTag(tag_mqttManager).Length > 0)
        {
            _eventSender = GameObject.FindGameObjectsWithTag(tag_mqttManager)[0].gameObject.GetComponent<mqttManager>();
            if (!_eventSender.isConnected)
            {
                _eventSender.Connect(); //Connect tha Manager when the object is spawned
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
        //https://github.com/XCharts-Team/XCharts/blob/master/Documentation~/en/configuration.md#labelstyle
        ringChart.series[0].label.formatter = "{c:f0}W";

        //We need to check the topic of the message to know where to use it 
        if (mqttObject.topic.Contains(topicSubscribed))
        {
            StartCoroutine(Blink());
            var response = JsonUtility.FromJson<tasmotaSensor.Root>(mqttObject.msg);
            List<double> values = new List<double>
            {
                response.ENERGY.Power,
                1000

            };
            ringChart.UpdateData(0, 0, values);

            todayEnergy.text = "Today: " + response.ENERGY.Today.ToString() + "kWh";
            yesterdayEnergy.text = "Yesterday: " + response.ENERGY.Yesterday.ToString() + "kWh";
            totalEnergy.text = "Total: " + response.ENERGY.Total.ToString() + "kWh";

            since.text = response.ENERGY.TotalStartTime;

            Debug.Log("Event Fired. The message, from Object " + nameController + " is = " + pointerValue);
        }
    }

    IEnumerator Blink()
    {
        for (int i = 0; i < blinkCount; i++)
        {
            // Lerp from color1 to color2
            float elapsedTime = 0f;
            while (elapsedTime < blinkDuration)
            {
                toggle.color = Color.Lerp(color1, color2, elapsedTime / blinkDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Lerp from color2 to color1
            elapsedTime = 0f;
            while (elapsedTime < blinkDuration)
            {
                toggle.color = Color.Lerp(color2, color1, elapsedTime / blinkDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

}

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
[Serializable]
public class tasmotaSensor
{
    [Serializable]
    public class ENERGY
    {
        public string TotalStartTime;
        public double Total;
        public double Yesterday;
        public double Today;
        public int Period;
        public int Power;
        public int ApparentPower;
        public int ReactivePower;
        public double Factor;
        public int Voltage;
        public double Current;
    }
    [Serializable]
    public class Root
    {
        public DateTime Time;
        public ENERGY ENERGY;
    }
}