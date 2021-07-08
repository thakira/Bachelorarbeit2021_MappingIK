using UnityEngine;

public class GestureCam : MonoBehaviour
{
    private bool training;
    public int gestureTask = 1;
    public GameObject TrainerStart;
    public GameObject Trainer1;
    public GameObject Trainer2;
    public GameObject Trainer3;
    public GameObject Trainer4;
    public GameObject Trainer5;
    private GameObject hud;
    private TMPro.TextMeshProUGUI explanations;
    private bool end;



    //Erklärungstexte für HUD
    private string textStart =
        "Der Trainer wird Dir nun Armstellungen zeigen, welche Du nachmachen sollst." + "\n" +
        "Sobald Du diese Geste erreicht hast, drücke bitte die Trigger-Taste am Controller." + "\n" +
        "Solltest Du Dich dafür entschieden haben, die Studie mit Fotografien zu unterstützen, lasse bitte zeitgleich das Foto erstellen." +
        "\n" + "\n" + "Starte nun das Experiment, indem Du die Taste \"A\" auf dem rechten Controller drückst.";

    private string textTask =
        "Drücke die Triggertaste, wenn Du die gleiche Haltung wie der Trainer eingenommen hast.";

    private string textBetween =
        "Super gemacht!" + "\n" + "Drücke bitte die \"A\"-Taste, wenn Du mit der nächsten Aufgabe fortfahren möchtest.";

    private string textEnde =
        "Das war's!" + "\n" + "Führe, sofern Du noch keine 5 Durchgänge gemacht hast, nun bitte den nächsten durch." +
        "\n" + "\n" + "Wenn dies der letzte Durchgang war:" + "\n" + "Vielen Dank für Deine Teilnahme! " + "\n" + "\n" +
        "Folge nun bitte der Anleitung in der Studienbeschreibung zur Übermittlung der Daten.";


    void Start()
    {
        //Textfeld des HUDs finden
        hud = GameObject.FindGameObjectWithTag("HUD");
        explanations = hud.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        //Starttext einblenden
        explanations.text = textStart;
    }

    void Update()
    {
        // A-Taste startet die Aufgabe & wechselt die Trainergeste
        if (!end)
        {
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                if (gestureTask == 1)
                {
                    TrainerStart.SetActive(false);
                    Trainer1.SetActive(true);
                }
                else if (gestureTask == 2)
                {
                    Trainer1.SetActive(false);
                    Trainer2.SetActive(true);
                }
                else if (gestureTask == 3)
                {
                    Trainer2.SetActive(false);
                    Trainer3.SetActive(true);
                }
                else if (gestureTask == 4)
                {
                    Trainer3.SetActive(false);
                    Trainer4.SetActive(true);
                }
                else if (gestureTask == 5)
                {
                    Trainer4.SetActive(false);
                    Trainer5.SetActive(true);
                    end = true;
                }

                training = true;
            }
        }


        //Text der Aufgabe einblenden und warten, bis Trigger-Taste gedrückt wurde
        if (training)
        {
            explanations.text = textTask;

            // Wenn Trigger-Taste gedrückt, Screenshot anfertigen
            if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger) || OVRInput.Get(OVRInput.RawButton.LIndexTrigger))
            {
                GestureCamera.TakePic("Z4_Task" + gestureTask);
                // SideCamera.TakePic("Z4_Task" + gestureTask + "_Side");
                // BackCamera.TakePic("Z4_Task" + gestureTask + "_Back");
                training = false;

                // nächsten Task anzeigen
                if (gestureTask < 5)
                {
                    explanations.text = textBetween;
                    gestureTask++;
                }

                else
                {
                    Trainer5.SetActive(false);
                    explanations.text = textEnde;
                    hud.GetComponent<HUDInteraction>().ActivateInput();
                }
            }
        }
    }
}