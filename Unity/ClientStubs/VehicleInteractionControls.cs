using UnityEngine;

// Stand-in for the game's client class of the same name. Eco binds its real class by assembly, namespace and
// class name when it loads the bundle, so this copy only declares the serialized fields the Hot Wheels prefabs set.
namespace Eco.Client.Vehicles.Internals
{
    public class VehicleInteractionControls : MonoBehaviour { }
}
