using UnityEngine;

// Stand-in for the game's client class of the same name. Eco binds its real class by assembly, namespace and
// class name when it loads the bundle, so this copy only declares the serialized fields the Hot Wheels prefabs set.
public class Vehicle : WorldObject
{
    public int AttachedToID;
    public Vector3 centerPosition;
    public string vehicleControlHintsTemplate = "";
    public string UIName = "VehicleToolbar";
    public bool hasLights;
    public bool hasHonk;
    public string wheelsGameobjectLayer = "IgnoreInteraction";
    public bool ignoreMoveThroughCosts;
    public bool syncWheelPos;
    public GameObject frontWheels;
    public Collider[] AllVehicleColliders = new Collider[0];
}
