using RootMotion.FinalIK;
using UnityEngine;

// Stand-in for the game's client class of the same name. Eco binds its real class by assembly, namespace and
// class name when it loads the bundle, so this copy only declares the serialized fields the Hot Wheels prefabs set.
public class MountSpot : MonoBehaviour
{
    public InteractionTarget[] IKTargets = new InteractionTarget[0];
    public Transform exitPosition;
    public Transform alternativeExitPosition;
    public float thirdPersonCameraOffset;
    public bool setAsParent = true;
    public Transform cameraTarget;
    public int overrideAvatarState = 255;
    public Collider occupiedCollider;
    public bool lockRotation;
    public bool AlignWithPlayerSide;
    public int InteractionFromInside;
    public bool ApplyHandsGripOnMount;
}
