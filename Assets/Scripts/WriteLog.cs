using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;

#endif

[System.Serializable]
public class LogEntry
{
    public int Task;
    public float AccelerationX_active;
    public float AccelerationY_active;
    public float AccelerationZ_active;
    public float VelocityDistance_active;
    public bool TriggerPressed;
    public bool VelocityZero;
    public Vector3 CollidedControllerPosition;
    public Vector3 TriggeredControllerPosition;
    public float DistanceControllerPositions;
    public int ElapsedFrames;

    public LogEntry()
    {
    }

    // für Studie verwendeter Konstruktor
    public LogEntry(int task, int elapsedFrames, float velocityDistance_active,
        float accelerationX_active, float accelerationY_active, float accelerationZ_active,
        bool triggerPressed, bool velocityZero, Vector3 collidedControllerPosition, Vector3 triggeredControllerPosition,
        float distanceControllerPositions)
    {
        Task = task;
        ElapsedFrames = elapsedFrames;
        AccelerationX_active = accelerationX_active;
        AccelerationY_active = accelerationY_active;
        AccelerationZ_active = accelerationZ_active;
        VelocityDistance_active = velocityDistance_active;
        TriggerPressed = triggerPressed;
        VelocityZero = velocityZero;
        CollidedControllerPosition = collidedControllerPosition;
        TriggeredControllerPosition = triggeredControllerPosition;
        DistanceControllerPositions = distanceControllerPositions;
    }
}


public class WriteLog : MonoBehaviour
{
    private List<LogEntry> logEntries = new List<LogEntry>(10000);
    public static bool logging;
    private bool triggerPressed;
    public static int task = 1;

    private GameObject hud;

    private TMPro.TextMeshProUGUI explanations;
    public static bool task1Collision;
    public static bool task2Collision;
    public static bool task3Collision;
    public static bool task4Collision;
    public static bool velocityZero;
    private GameObject trackingSpace;
    public static Vector3 collidedControllerPosition;
    private Vector3 triggeredControllerPosition;
    private float distanceControllerPositions;
    public int startingFrame;


    //Erklärungstexte für HUD
    private string textStart =
        "Ich werde Dir nun Aufgaben geben, in denen Du verschiedene Deiner eigenen Körperteile mit einem der Controller berühren sollst." +
        "\n" + "Sobald Du das jeweilige Körperteil berührst, drücke bitte die Trigger-Taste am Controller!" + "\n" +
        "\n" + "Starte nun das Experiment, indem Du die Taste \"A\" auf dem rechten Controller drückst.";

    private string textTask1 =
        "Berühre nun bitte den linken Oberarm Deines Körpers." + "\n" +
        "Drücke die Trigger-Taste, wenn Du ihn erreicht hast.";

    private string textTask2 =
        "Berühre nun bitte den linken Unterarm Deines Körpers." + "\n" +
        "Drücke die Trigger-Taste, wenn Du ihn erreicht hast.";

    private string textTask3 =
        "Berühre nun bitte den rechten Oberarm Deines Körpers." + "\n" +
        "Drücke die Trigger-Taste, wenn Du ihn erreicht hast.";

    private string textTask4 =
        "Berühre nun bitte den rechten Unterarm Deines Körpers." + "\n" +
        "Drücke die Trigger-Taste, wenn Du ihn erreicht hast.";

    private string textBetween =
        "Super gemacht!" + "\n" + "Drücke bitte die \"A\"-Taste, wenn Du mit der nächsten Aufgabe fortfahren möchtest.";

    private string textEnde =
        "Super, das war's!" + "\n" + "Starte bitte das nächste Szenario, indem Du auf 'Weiter' klickst.";

    // Start is called before the first frame update
    void Start()
    {
        //Textfeld des HUDs finden
        hud = GameObject.FindGameObjectWithTag("HUD");
        explanations = hud.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        //Starttext einblenden
        explanations.text = textStart;
        trackingSpace = GameObject.Find("TrackingSpace");
    }

    void Update()
    {
        // A-Taste startet die Aufgabe und das Mitloggen
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            logging = true;
            startingFrame = Time.frameCount;
        }

        //Text der Aufgabe einblenden und Mitloggen bis Trigger-Taste gedrückt wurde
        if (logging)
        {
            //Loggen bis einer der beiden Triggerbuttons gedrückt wurde
            if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger) || OVRInput.Get(OVRInput.RawButton.LIndexTrigger))
            {
                triggerPressed = true;
                if (task == 1 || task == 2)
                {
                    triggeredControllerPosition =
                        OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
                }
                else if (task == 3 || task == 4)
                {
                    triggeredControllerPosition =
                        OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
                }
            }

            if (!triggerPressed)
            {
                StartCoroutine(LogEntry());
            }
            else
            {
                GestureCamera.TakePic("Z2_Trigger_Task" + task);
                StartCoroutine(LogEntry());
                StartCoroutine(Write2Disk());
                logEntries.Clear();
                triggerPressed = false;
                logging = false;
                if (task < 4)
                {
                    explanations.text = textBetween;
                    task++;
                }
                else
                {
                    explanations.text = textEnde;
                    hud.GetComponent<HUDInteraction>().ActivateInput();
                }
            }
        }
    }

    //Logeintrag zur Liste hinzufügen
    IEnumerator LogEntry()
    {
        OVRInput.Controller controller_active = OVRInput.Controller.None;
        bool collision = false;
        if (task == 1 || task == 2)

        {
            controller_active = OVRInput.Controller.RTouch;
        }
        else if (task == 3 || task == 4)
        {
            controller_active = OVRInput.Controller.LTouch;
        }

        if (task == 1)
        {
            explanations.text = textTask1;
            collision = task1Collision;
        }
        else if (task == 2)
        {
            explanations.text = textTask2;
            collision = task2Collision;
        }
        else if (task == 3)
        {
            explanations.text = textTask3;
            collision = task3Collision;
        }
        else if (task == 4)
        {
            explanations.text = textTask4;
            collision = task4Collision;
        }


        logEntries.Add(new LogEntry(
            task,
            Time.frameCount - startingFrame,
            Vector3.Distance(
                trackingSpace.transform.TransformVector(OVRInput.GetLocalControllerVelocity(controller_active)),
                Vector3.zero),
            OVRInput.GetLocalControllerAcceleration(controller_active).x,
            OVRInput.GetLocalControllerAcceleration(controller_active).y,
            OVRInput.GetLocalControllerAcceleration(controller_active).z,
            triggerPressed,
            velocityZero,
            collidedControllerPosition,
            triggeredControllerPosition,
            Vector3.Distance(collidedControllerPosition, triggeredControllerPosition)
        ));
        yield return new WaitForSeconds(1f);
    }

//Listeneinträge in semikolonseparierte Strings umwandeln
    public string ToCSV()

    {
        var sb = new StringBuilder(
            "Task;Frame;ActiveVelocityDistance;" +
            "ActiveAccelerationX;ActiveAccelerationY;ActiveAccelerationZ;TriggerPressed;VelocityZero; " +
            "CollidedControllerPosition; TriggeredControllerPosition; DistanceControllerPosition");
        foreach (var entry in logEntries)
        {
            sb.Append('\n')
                .Append(entry.Task.ToString()).Append(';')
                .Append(entry.ElapsedFrames.ToString()).Append(';')
                .Append(entry.VelocityDistance_active.ToString()).Append(';')
                .Append(entry.AccelerationX_active.ToString()).Append(';')
                .Append(entry.AccelerationY_active.ToString()).Append(';')
                .Append(entry.AccelerationZ_active.ToString()).Append(';')
                .Append(entry.TriggerPressed.ToString()).Append(';')
                .Append(entry.VelocityZero.ToString()).Append(';')
                .Append(entry.CollidedControllerPosition.ToString()).Append(';')
                .Append(entry.TriggeredControllerPosition.ToString()).Append(';')
                .Append(entry.DistanceControllerPositions.ToString()).Append(';');
        }

        return sb.ToString();
    }

//Datei auf die Festplatte schreiben
    IEnumerator Write2Disk()
    {
        string fileName =
            ("Z2_" + DateTime.Now.Day.ToString() + "_"
             + DateTime.Now.Month.ToString() + "_"
             + DateTime.Now.Year.ToString() + "_"
             + DateTime.Now.Hour.ToString() + "_"
             + DateTime.Now.Minute.ToString() + "_"
             + DateTime.Now.Second.ToString() + ".csv");
        // CSV-Zeilen aus der Liste erstellen
        var content = ToCSV();

        // Zielverzeichnis
#if UNITY_EDITOR
        var folder = Application.streamingAssetsPath;

        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
#else
    var folder = Application.persistentDataPath;
#endif

        var filePath = Path.Combine(folder, fileName);

        using (var writer = new StreamWriter(filePath, false))
        {
            writer.Write(content);
        }


#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
        yield return new WaitForSeconds(2f);
    }
}