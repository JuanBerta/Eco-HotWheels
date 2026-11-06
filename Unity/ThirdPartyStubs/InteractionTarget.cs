using UnityEngine;

// Stand-in for the game's client class of the same name. Eco binds its real class by assembly, namespace and
// class name when it loads the bundle, so this copy only declares the serialized fields the Hot Wheels prefabs set.
namespace RootMotion.FinalIK
{
    public class InteractionTarget : MonoBehaviour
    {
        public int effectorType;               // FullBodyBipedEffector: 0 body, 1/2 shoulders, 3/4 thighs, 5/6 hands, 7/8 feet
        public float interactionSpeedMlp = 1f;
        public Transform pivot;
        public int rotationMode;
        public Vector3 twistAxis = Vector3.up;
        public float twistWeight = 1f;
        public float swingWeight = 0f;
        public float threeDOFWeight = 1f;
        public bool rotateOnce = true;
        public bool usePoser = true;
    }
}
