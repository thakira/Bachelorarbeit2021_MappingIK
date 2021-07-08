using UnityEngine;


public class Collisions : MonoBehaviour
{
    [SerializeField] private GameObject trackingSpace;
    private static int i = 0;
    private const int Waitframes = 5;
    private const float VelocityRange = 0.1f;
    private bool colliderposition;
    private bool pic1;
    private bool pic2;
    private bool pic3;
    private bool pic4;

    // Berührungserkennung
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "RightHandCollider")
        {
            if (gameObject.name != "RightLowerArmCollider")
            {
                // Controllergeschwindigkeit < 0,1?
                if (Vector3.Distance(trackingSpace.transform.TransformVector(
                    OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch)), Vector3.zero) <= VelocityRange)
                {
                    WriteLog.velocityZero = true;
                    // Erkennung erst nach <waitframes>-Frames innerhalb des Colliders
                    if (i < Waitframes)
                    {
                        i++;
                    }
                    else
                    {
                        // rechte Controller-Position loggen
                        if(!colliderposition) {
                        WriteLog.collidedControllerPosition =
                            OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
                        colliderposition = true;
                        }
                        if (gameObject.name == "LeftUpperArmCollider")
                        {
                            if (WriteLog.task == 1 && WriteLog.logging)
                            {
                                // Kollision loggen
                                WriteLog.task1Collision = true;
                                // Screenshot Kollision
                                if (!pic1)
                                {
                                    GestureCamera.TakePic("Z2_Collision_Task1");
                                    pic1 = true;
                                }

                                i = 0;
                            }
                        }
                        else if (gameObject.name == "LeftLowerArmCollider")
                        {
                            if (WriteLog.task == 2 && WriteLog.logging)
                            {
                                WriteLog.task2Collision = true;
                                if (!pic2)
                                {
                                    GestureCamera.TakePic("Z2_Collision_Task2");
                                    pic2 = true;
                                }
                                colliderposition = false;
                                i = 0;
                            }
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
                    OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch)), Vector3.zero) <= VelocityRange)
                {
                    WriteLog.velocityZero = true;

                    if (i < Waitframes)
                    {
                        i++;
                    }
                    else
                    {
                        if (!colliderposition)
                        {
                            WriteLog.collidedControllerPosition =
                                OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
                            colliderposition = true;
                        }

                        if (gameObject.name == "RightUpperArmCollider")
                        {
                            if (WriteLog.task == 3 && WriteLog.logging)
                            {
                                WriteLog.task3Collision = true;
                                if (!pic3)
                                {
                                    GestureCamera.TakePic("Z2_Collision_Task3");
                                    pic3 = true;
                                }

                                i = 0;
                            }
                        }
                        else if (gameObject.name == "RightLowerArmCollider")
                        {
                            if (WriteLog.task == 4 && WriteLog.logging)
                            {
                                WriteLog.task4Collision = true;
                                if (!pic4)
                                {
                                    GestureCamera.TakePic("Z2_Collision_Task4");
                                    pic4 = true;
                                }

                                i = 0;
                                colliderposition = false;
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        i = 0;
        WriteLog.velocityZero = false;
        colliderposition = false;
    }
}