using System.Collections;
using UnityEngine;
using UnityEngine.Networking; // For HTTP requests
using XCharts.Runtime; // XCharts library

public class HttpToSingleBarChartController : MonoBehaviour
{
    public BarChart barChart; // Single BarChart for SAC1 and SAC2
    public string apiEndpointSAC = "http://10.129.111.13:3001/total/SAC"; // Single API endpoint

    void Start()
    {
        // Fetch data for both SAC1 and SAC2
        FetchDataForSAC();
    }

    // Fetch data from the endpoint
    void FetchDataForSAC()
    {
        StartCoroutine(GetData(apiEndpointSAC));
    }

    IEnumerator GetData(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseData = request.downloadHandler.text; // JSON response
            Debug.Log("Response: " + responseData);

            // Parse the JSON and update the bar chart
            UpdateBarChart(responseData);
        }
        else
        {
            Debug.LogError("Error fetching data: " + request.error);
        }
    }

    void UpdateBarChart(string jsonData)
    {
        // Deserialize JSON into the Root object
        Root sacValues = JsonUtility.FromJson<Root>(jsonData);

        // Debug: Log the parsed values
        Debug.Log($"SAC1: {sacValues.sac1}, SAC2: {sacValues.sac2}");

        // Clear the existing chart data
        barChart.ClearData();

        // Add SAC1 data (Series 0)
        barChart.AddData(0, sacValues.sac1);
        barChart.AddXAxisData("SAC1");

        // Add SAC2 data (Series 0)
        barChart.AddData(0, sacValues.sac2);
        barChart.AddXAxisData("SAC2");

        // Force refresh to ensure chart updates
        barChart.RefreshChart();
    }
}

// Root class for JSON deserialization
[System.Serializable] // Required for JsonUtility
public class Root
{
    public int sac1;
    public int sac2;
}


