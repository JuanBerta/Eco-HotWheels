using UnityEngine;

// Stand-in for the game's client class of the same name. Eco binds its real class by assembly, namespace and
// class name when it loads the bundle, so this copy only declares the serialized fields the Hot Wheels prefabs set.
public class Mountable : MonoBehaviour
{
    public MountSpot[] seats = new MountSpot[0];
    public bool handleDismount = true;
    public bool blockDismountWhenNoExit = true;
    public bool freezeWhenUnmounted;
}
