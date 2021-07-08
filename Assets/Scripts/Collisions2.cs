using UnityEngine;
using UnityEngine.UI;

public class Collisions2 : MonoBehaviour
{
    [SerializeField] private Toggle ToggleRightUpperArm;
    [SerializeField] private Toggle ToggleRightForeArm;
    [SerializeField] private Toggle ToggleLeftUpperArm;
    [SerializeField] private Toggle ToggleLeftForeArm;
    [SerializeField] private GameObject trackingSpace;
    [SerializeField] private GameObject toggles;

    public static int i = 0;
    private const int Waitframes = 5;
    private const float VelocityRange = 0.1f;
    private GameObject hud;
    private TMPro.TextMeshProUGUI explanations;
    private bool recognizeCollisions;

    private void Awake()
    {
        ToggleLeftForeArm.isOn = false;
        ToggleRightForeArm.isOn = false;
        ToggleRightUpperArm.isOn = false;
        ToggleLeftUpperArm.isOn = false;
        //Textfeld des HUDs finden
        hud = GameObject.FindGameObjectWithTag("HUD");
        explanations = hud.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        // Checkboxen zum Start ausblenden
        toggles.SetActive(false);
        // Erklärungstext anzeigen
        explanations.text =
            "In diesem Szenario geht es darum, wie gut die Position der vom System erkannten Berührung mit der von Dir an Deinen Armen gefühlten übereinstimmt." +
            "\n" +
            "Berühre bitte die angegebenen Körperteile in beliebiger Reihenfolge und merke Dir, wie gut die Erkennung zu Deiner echten Berührung gepasst hat." +
            "\n" + "\n" +
            "Drücke 'A' um zu beginnen.";
    }

    void Update()
    {
        // Wenn bereits alle Körperteile berührt wurden, Aufgabe beenden
        if (ToggleLeftForeArm.isOn && ToggleLeftUpperArm.isOn && ToggleRightForeArm.isOn && ToggleRightUpperArm.isOn)
        {
            // Checkboxen deaktivieren
            toggles.SetActive(false);
            // Ende-Text & Weiter-Button einblenden
            explanations.text = "Super, klicke auf 'Weiter' für das nächste Szenario";
            hud.GetComponent<HUDInteraction>().ActivateInput();
        }

        // A-Taste startet die Aufgabe
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            // Checkboxen einblenden und Erkennungsschleife starten
            explanations.text = "";
            toggles.SetActive(true);
            recognizeCollisions = true;
        }
    }

    // Berührungserkennung
    private void OnTriggerStay(Collider other)
    {
        if (recognizeCollisions)
        {
            if (other.gameObject.name == "RightHandCollider")
            {
                if (gameObject.name != "RightLowerArmCollider")
                {
                    if (Vector3.Distance(trackingSpace.transform.TransformVector(
                            OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch)), Vector3.zero) <=
                        VelocityRange)
                    {
                        if (i < Waitframes)
                        {
                            i++;
                        }
                        else
                        {
                            // linker Oberarm berührt
                            if (gameObject.name == "LeftUpperArmCollider")
                            {
                                i = 0;
                                ToggleLeftUpperArm.isOn = true;
                            }
                            // linker Unterarm berührt
                            else if (gameObject.name == "LeftLowerArmCollider")
                            {
                                ToggleLeftForeArm.isOn = true;
                                i = 0;
                            }
                        }
                    }
                }
            }
            else if (other.gameObject.name == "LeftHandCollider")
            {
                if (gameObject.name != "LeftLowerArmCollider")
                {
                    if (Vector3.Distance(trackingSpace.transform.TransformVector(
                            OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch)), Vector3.zero) <=
                        VelocityRange)
                    {
                        WriteLog.velocityZero = true;

                        if (i < Waitframes)
                        {
                            i++;
                        }
                        else
                        {
                            // rechter Oberarm berührt
                            if (gameObject.name == "RightUpperArmCollider")
                            {
                                i = 0;
                                ToggleRightUpperArm.isOn = true;
                            }
                            // rechter Unterarm berührt
                            else if (gameObject.name == "RightLowerArmCollider")
                            {
                                i = 0;
                                ToggleRightForeArm.isOn = true;
                            }
                        }
                    }
                }
            }
        }
    }

    // Trigger verlassen
    private void OnTriggerExit(Collider other)
    {
        i = 0;
        WriteLog.velocityZero = false;
    }
}