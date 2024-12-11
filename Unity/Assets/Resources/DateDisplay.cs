using UnityEngine;
using TMPro; 

public class DateDisplay : MonoBehaviour
{
    public TMP_Text dateText; 
    void Start()
    {
        string currentDate = System.DateTime.Now.ToString("dd/MM/yyyy"); 
        dateText.text = $"Today's Date: {currentDate}";
    }

    void Update()
    {
        string currentDate = System.DateTime.Now.ToString("dd/MM/yyyy");
        dateText.text = $"Today's Date: {currentDate}";
    }
}
