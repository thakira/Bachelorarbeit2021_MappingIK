using UnityEngine;


[System.Serializable]
public class VRMap
{
    public Transform vrTarget;
    public Transform rigTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;
    
    // Mapping Rig-Objekt auf VR-Objekt
    public void Map()
    {
        rigTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }

}

public class IK : MonoBehaviour
{
    public VRMap head;
    public VRMap leftHand;
    public VRMap rightHand;
    
    [SerializeField]
    private Transform headConstraint;
    [SerializeField]
    private Vector3 headBodyOffset;
    void Start()
    {
        headBodyOffset = transform.position - headConstraint.position;
    }
    
    void FixedUpdate()
    {
        transform.position = headConstraint.position + headBodyOffset;

        
        // Mapping der Körperteile auf die VR-Komponenten
        head.Map();
        leftHand.Map();
        rightHand.Map();
    }
}
