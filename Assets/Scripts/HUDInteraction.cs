using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDInteraction : MonoBehaviour
{
    private static GameObject[] input;

    private void Awake()
    {
        input = GameObject.FindGameObjectsWithTag("Input");
        DeactivateInput();

    }
    // Interaktionsmöglichkeiten (Laserstrahl & Button) aktivieren
    public void ActivateInput()
    {

        foreach (GameObject item in input)
        {
            if (SceneManager.GetActiveScene().name == "Z4_Gestures" && item.name == "Button") 
            {
                var button = item.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                button.text = "Beenden";
            }
            item.SetActive(true);
        }
    }
    // Interaktionsmöglichkeiten (Laserstrahl & Button) deaktivieren
    public void DeactivateInput()
    {
        foreach (var item in input)
        {
            if (SceneManager.GetActiveScene().name != "Z1_Start") 
            {
                item.SetActive(false);
            }

        }
    }
    // Szenenwechsel bzw. Ende bei Button-Interaktion
    public void ButtonPressed()
    {
        if (SceneManager.GetActiveScene().name == "Z1_Start")
        {
            SceneManager.LoadScene("Z2_Graphs");
        }
        else if (SceneManager.GetActiveScene().name == "Z2_Graphs")
        {
            SceneManager.LoadScene("Z3_Detection");
        }
        else if (SceneManager.GetActiveScene().name == "Z3_Detection")
        {
            SceneManager.LoadScene("Z4_Gestures");
        }
        else if (SceneManager.GetActiveScene().name == "Z4_Gestures")
        {

            UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
        }
    }
}
