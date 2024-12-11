using System; // Basic system functions
using System.Collections.Generic; // For lists and dictionaries
using UnityEngine; // Unity-specific functionality
using UnityEngine.Networking; // For HTTP requests
using TMPro; // TextMeshPro for handling text on UI
using System.Collections; // For coroutines

public class HttpController : MonoBehaviour
{
    [Tooltip("Optional name for the controller")]
    public string nameController = "Controller 1";

    [Tooltip("List of URLs to fetch data from")]
    public List<string> urls; // A list of HTTP endpoints to fetch data from

    [Tooltip("List of TextMeshProUGUI components for displaying the fetched data")]
    public List<TextMeshProUGUI> textFields; // A list of UI text elements for displaying data

    private Dictionary<string, TextMeshProUGUI> urlToTextMap = new Dictionary<string, TextMeshProUGUI>();

    void Awake()
    {
        // Ensure the URLs and textFields match in number
        if (urls.Count != textFields.Count)
        {
            Debug.LogError("The number of URLs must match the number of TextMeshPro text fields.");
            return;
        }

        // Map URLs to their corresponding TextMeshPro text fields
        for (int i = 0; i < urls.Count; i++)
        {
            urlToTextMap.Add(urls[i], textFields[i]);
        }
    }

    void Start()
    {
        // Start fetching data for each URL in the list
        foreach (var url in urls)
        {
            StartCoroutine(FetchData(url));
        }
    }

    IEnumerator FetchData(string url)
    {
        while (true) // Continuous polling loop
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                // Send the request
                yield return request.SendWebRequest();

                // Handle the response
                if (request.result == UnityWebRequest.Result.Success)
                {
                    // Parse the response and update the corresponding text field
                    if (urlToTextMap.TryGetValue(url, out var textField))
                    {
                        textField.text = request.downloadHandler.text;
                    }
                }
                else
                {
                    Debug.LogError($"Error fetching data from {url}: {request.error}");
                }
            }

            // Wait for 5 seconds before the next request (adjust as needed)
            yield return new WaitForSeconds(5);
        }
    }
}
