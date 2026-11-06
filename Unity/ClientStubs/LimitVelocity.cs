using UnityEngine;

// Stand-in for the game's client class of the same name. Eco binds its real class by assembly, namespace and
// class name when it loads the bundle, so this copy only declares the serialized fields the Hot Wheels prefabs set.
public class LimitVelocity : MonoBehaviour
{
    public float MaxVelocity = 5f;
    public float MaxAngularVelocity = 50f;
    public bool ignoreY = true;
    public float MaxAccel = 9.8f;
}
