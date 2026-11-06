using System;
using System.IO;
using System.Linq;
using System.Text;
using Eco.Client;
using Eco.Client.Utils.Settings;
using Eco.Client.Vehicles.Internal;
using Eco.Client.Vehicles.Internals;
using RootMotion.FinalIK;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Builds the client bundle for the Hot Wheels "Multi-Row Wheel Hoe".
/// The vehicle prefab mirrors the Hot Wheels Scooter (same Eco client components and values) with the
/// Blender model in place of the scooter meshes. Its item reuses the vanilla Hand Plow icon.
/// Mirrors the ModKit's "Build Current Bundle", but saves to a fixed path so it can run unattended.
/// </summary>
public static class HotWheelsHoeBundle
{
    const string Dir        = "Assets/HotWheels";
    const string ModelPath  = Dir + "/Models/MultiRowWheelHoe.fbx";
    const string GenDir     = Dir + "/Generated";
    const string ScenePath  = Dir + "/MultiRowWheelHoe.unity";
    const string PrefabPath = Dir + "/MultiRowWheelHoeObject.prefab";
    const string BundleName = "multirowwheelhoe";
    const string OutputFile = @"C:\Users\Public\hw_assets\MultiRowWheelHoe.unity3d";
    const string ResultFile = "AgentScripts/hoe_build_result.txt";

    const float GroundY = -0.494f;   // Same ground offset as the Scooter: the object origin sits ~0.5 m above the ground.
    const int WheelLayer = 18, ComLayer = 30;

    static StringBuilder log;

    [MenuItem("Hot Wheels/Build Multi-Row Wheel Hoe Bundle")]
    public static void Build()
    {
        log = new StringBuilder();
        try
        {
            EnsureFolder(GenDir);
            var prefab = BuildVehiclePrefab();
            BuildSceneAndBundle(prefab);
            log.AppendLine("OK");
        }
        catch (Exception e) { log.AppendLine("ERROR " + e); }
        Directory.CreateDirectory(Path.GetDirectoryName(ResultFile));
        File.WriteAllText(ResultFile, log.ToString());
        Debug.Log("[HotWheels] " + log);
    }

    // ---------- vehicle prefab ----------
    static GameObject BuildVehiclePrefab()
    {
        var model = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath) ?? throw new Exception("Model not found at " + ModelPath);
        var bodySrc = model.GetComponentsInChildren<MeshFilter>(true).First(m => m.name.Contains("HoeBody"));
        var wheelSrc = model.GetComponentsInChildren<MeshFilter>(true).First(m => m.name.Contains("HoeWheel"));
        Vector3 wheelCentre = model.transform.InverseTransformPoint(wheelSrc.transform.position);
        log.AppendLine($"model: body bounds {bodySrc.sharedMesh.bounds}, wheel centre {wheelCentre}, body rot {bodySrc.transform.localRotation.eulerAngles}");
        bool flip = wheelCentre.z < 0f;   // the wheel must be at the front (+Z)
        if (flip) { log.AppendLine("wheel was behind the body: rotating the visuals 180 degrees"); wheelCentre = new Vector3(-wheelCentre.x, wheelCentre.y, -wheelCentre.z); }

        var root = new GameObject("MultiRowWheelHoeObject") { tag = "ModObject" };
        var rb = root.AddComponent<Rigidbody>();
        rb.mass = 1500f; rb.angularDamping = 0.05f; rb.interpolation = RigidbodyInterpolation.Interpolate;

        var sync = root.AddComponent<SyncPhysics>();
        SetProps(sync, ("SyncPos", 1), ("SyncRot", 1), ("RotSyncMode", 7), ("VelocityLimit", 50f), ("distanceToIgnorePhysics", 25f),
                       ("snapDistance", 5f), ("maxDepenetationVelocity", 0.2f), ("AutomaticChunkDepenetrationEnabled", 1));
        var vehicle = root.AddComponent<Vehicle>();      // adds WrapTarget and NetObjComponent through WorldObject's RequireComponent
        vehicle.hasOccupancy = true; vehicle.overrideOccupancy = true; vehicle.size = new Vector3(1.3f, 1.1f, 1.9f); vehicle.interactable = true;

        // Chassis: the hoe body mesh.
        var chassis = Child(root, "Chassis", new Vector3(0, GroundY, 0));
        var bodyGo = flip ? Child(chassis, "Body", Vector3.zero, Quaternion.Euler(0, 180, 0)) : chassis;
        AddMesh(bodyGo, bodySrc);
        AddBox(chassis, new Vector3(1.24f, 0.08f, 0.10f), new Vector3(0, 0.38f, -0.05f));   // toolbar
        AddBox(chassis, new Vector3(0.30f, 0.12f, 0.50f), new Vector3(0, 0.40f, 0.20f));    // fork and frame
        AddBox(chassis, new Vector3(0.70f, 0.50f, 0.80f), new Vector3(0, 0.70f, -0.50f));   // handles
        chassis.AddComponent<HighlightableObject>();

        // Front wheel: steering node holding the spinning wheel mesh.
        var steer = Child(chassis, "Wheelbarrow", wheelCentre);
        var wheelFront = Child(steer, "WheelFront", Vector3.zero, flip ? Quaternion.Euler(0, 180, 0) : Quaternion.identity);
        AddMesh(wheelFront, wheelSrc);
        AddWheelSync(wheelFront);
        var wheelBack = Child(chassis, "WheelBack", new Vector3(0, 0.10f, -0.05f));   // no rear wheel on a hoe: the tines ride here
        AddWheelSync(wheelBack);

        // Handles: the only place to grab the hoe (E), like the Scooter's handlebar.
        var handles = Child(chassis, "Handles", new Vector3(0, 0.97f, -0.92f));
        var hbox = handles.AddComponent<BoxCollider>(); hbox.size = new Vector3(0.75f, 0.15f, 0.30f);
        handles.AddComponent<HighlightableObject>();
        handles.AddComponent<DriveVehicleInteractable>();

        // Driver: stands on the ground behind the handles.
        var mountGo = Child(chassis, "MountSpot", new Vector3(0, -0.17f, -1.30f));
        var mount = mountGo.AddComponent<MountSpot>();
        var targets = new[]
        {
            Target(mountGo, "TargetPelvis", 0, new Vector3(-0.019f, 1.235f, -0.033f), Quaternion.identity),
            Target(mountGo, "TargetLFoot", 7, new Vector3(-0.15f, 0.19f, 0.05f), new Quaternion(0.622f, 0.614f, -0.337f, 0.350f)),
            Target(mountGo, "TargetRFoot", 8, new Vector3(0.15f, 0.19f, -0.05f), new Quaternion(0.210f, 0.751f, -0.612f, -0.135f)),
            Target(mountGo, "TargetLThigh", 3, new Vector3(-0.247f, 0.849f, 0.039f), Quaternion.identity),
            Target(mountGo, "TargetRThigh", 4, new Vector3(0.160f, 0.849f, 0.051f), Quaternion.identity),
            Target(handles, "TargetRHand", 6, new Vector3(0.31f, 0f, -0.05f), new Quaternion(-0.322f, 0.384f, 0.573f, 0.648f)),
            Target(handles, "TargetLHand", 5, new Vector3(-0.31f, 0f, -0.05f), new Quaternion(-0.387f, 0.397f, 0.637f, 0.535f)),
            Target(mountGo, "TargetLShoulder", 1, new Vector3(-0.179f, 1.293f, -0.029f), Quaternion.identity),
            Target(mountGo, "TargetRShoulder", 2, new Vector3(0.127f, 1.293f, -0.011f), Quaternion.identity),
        };
        mount.IKTargets = targets;
        mount.exitPosition = Child(mountGo, "exit", new Vector3(0.7f, 0f, 0.3f)).transform;
        mount.alternativeExitPosition = Child(mountGo, "exit (1)", new Vector3(-0.7f, 0f, 0.3f)).transform;
        mount.setAsParent = true; mount.overrideAvatarState = 255;

        var com = Child(root, "COM", new Vector3(0, GroundY + 0.25f, 0.05f)); com.layer = ComLayer;

        // Wheel colliders: two at the front wheel, two small ones where the tines ride.
        var wc = Child(root, "Wheel Colliders", new Vector3(0, GroundY, 0));
        float fr = 0.30f, rr = 0.10f;
        var fl = WheelCol(wc, "FrontLeftWheel", new Vector3(-0.12f, fr, wheelCentre.z), fr);
        var frw = WheelCol(wc, "FrontRightWheel", new Vector3(0.12f, fr, wheelCentre.z), fr);
        var rl = WheelCol(wc, "RearLeftWheel", new Vector3(-0.40f, rr, -0.10f), rr);
        var rrw = WheelCol(wc, "RearRightWheel", new Vector3(0.40f, rr, -0.10f), rr);

        var rcc = root.AddComponent<RCCCarControllerV2>();
        rcc.FrontLeftWheelTransform = wheelFront.transform; rcc.FrontRightWheelTransform = wheelFront.transform;
        rcc.RearLeftWheelTransform = wheelBack.transform; rcc.RearRightWheelTransform = wheelBack.transform;
        rcc.FrontLeftWheelCollider = fl; rcc.FrontRightWheelCollider = frw; rcc.RearLeftWheelCollider = rl; rcc.RearRightWheelCollider = rrw;
        rcc.SteeringWheel = steer.transform; rcc.COM = com.transform; rcc.chassis = chassis;
        rcc.engineTorqueCurve = new[] { new AnimationCurve(
            new Keyframe(0f, 1f, -0.0075f, -0.0075f, 0f, 0.3333333f),
            new Keyframe(33.333332f, 0.75f, -0.02625f, -0.02625f, 0.3333333f, 0.3333333f),
            new Keyframe(50f, 0f, -0.045f, -0.045f, 0.3333333f, 0.3333333f)) { preWrapMode = WrapMode.ClampForever, postWrapMode = WrapMode.ClampForever } };
        rcc.asphaltPhysicsMaterial = PhysMat("RCCAsphaltPhysics", 1f); rcc.grassPhysicsMaterial = PhysMat("RCCGrassPhysics", 0.35f);
        rcc.sandPhysicsMaterial = PhysMat("RCCSandPhysics", 0.45f); rcc.terrainPhysicsMaterial = PhysMat("RCCTerrainPhysics", 0.6f);

        var limit = root.AddComponent<LimitVelocity>(); limit.MaxVelocity = 3f;   // walking pace (the Scooter uses 5)
        var mountable = root.AddComponent<Mountable>(); mountable.seats = new[] { mount };
        root.AddComponent<WaitForGround>();
        root.AddComponent<DismountChecker>().settings = Dismount();
        root.AddComponent<InteractionBlocker>();
        root.AddComponent<VehicleInteractionControls>();

        var prefab = PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
        UnityEngine.Object.DestroyImmediate(root);
        log.AppendLine("prefab: " + PrefabPath);
        return prefab;
    }

    static GameObject Child(GameObject parent, string name, Vector3 pos, Quaternion? rot = null)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.transform.localPosition = pos; go.transform.localRotation = rot ?? Quaternion.identity;
        return go;
    }

    static void AddMesh(GameObject go, MeshFilter src)
    {
        go.AddComponent<MeshFilter>().sharedMesh = src.sharedMesh;
        var mr = go.AddComponent<MeshRenderer>();
        mr.sharedMaterials = src.GetComponent<MeshRenderer>().sharedMaterials.Select(m => CurvedMaterial(m ? m.name : "HoeSteel")).ToArray();
    }

    static Material CurvedMaterial(string name)
    {
        var path = $"{GenDir}/{name}.mat";
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            var shader = Shader.Find("Curved/Standard") ?? throw new Exception("Shader Curved/Standard not found");
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, path);
        }
        Color c; float metal, smooth;
        if (name.Contains("Paint")) { c = new Color(0.55f, 0.06f, 0.04f); metal = 0.2f; smooth = 0.55f; }
        else if (name.Contains("Wood")) { c = new Color(0.50f, 0.32f, 0.15f); metal = 0f; smooth = 0.3f; }
        else if (name.Contains("Rubber")) { c = new Color(0.04f, 0.04f, 0.04f); metal = 0f; smooth = 0.1f; }
        else { c = new Color(0.55f, 0.56f, 0.58f); metal = 0.8f; smooth = 0.6f; }
        mat.SetColor("_Color", c); mat.SetFloat("_Metallic", metal); mat.SetFloat("_Glossiness", smooth);
        EditorUtility.SetDirty(mat);
        return mat;
    }

    static void AddBox(GameObject go, Vector3 size, Vector3 centre)
    {
        var b = go.AddComponent<BoxCollider>(); b.size = size; b.center = centre;
    }

    static void AddWheelSync(GameObject go)
    {
        var s = go.AddComponent<SyncPhysics>();
        SetProps(s, ("SyncPos", 0), ("SyncRot", 1), ("RotSyncMode", 7), ("ManuallyUpdated", 1), ("VelocityLimit", 9f),
                    ("distanceToIgnorePhysics", 25f), ("snapDistance", 5f), ("maxDepenetationVelocity", 20f), ("AutomaticChunkDepenetrationEnabled", 1));
        go.AddComponent<Wheel>();
    }

    static InteractionTarget Target(GameObject parent, string name, int effector, Vector3 pos, Quaternion rot)
    {
        var t = Child(parent, name, pos, rot).AddComponent<InteractionTarget>();
        t.effectorType = effector;
        return t;
    }

    static WheelCollider WheelCol(GameObject parent, string name, Vector3 pos, float radius)
    {
        var go = Child(parent, name, pos); go.layer = WheelLayer;
        var w = go.AddComponent<WheelCollider>();
        w.center = new Vector3(0, 0.05f, 0); w.radius = radius; w.mass = 125f; w.wheelDampingRate = 1f;
        w.suspensionDistance = 0.1f; w.forceAppPointDistance = 0.08f;
        w.suspensionSpring = new JointSpring { spring = 35000f, damper = 1500f, targetPosition = 0.5f };
        w.forwardFriction = new WheelFrictionCurve { extremumSlip = 0.4f, extremumValue = 1f, asymptoteSlip = 0.8f, asymptoteValue = 0.75f, stiffness = 1f };
        w.sidewaysFriction = new WheelFrictionCurve { extremumSlip = 0.25f, extremumValue = 1f, asymptoteSlip = 0.5f, asymptoteValue = 0.75f, stiffness = 1f };
        go.AddComponent<RCCWheelCollider>();
        var box = go.AddComponent<BoxCollider>(); box.size = new Vector3(0.23f, radius * 2f, radius * 2f); box.enabled = false;
        return w;
    }

    static PhysicsMaterial PhysMat(string name, float friction)
    {
        var path = $"{GenDir}/{name}.physicMaterial";
        var m = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>(path);
        if (m == null) { m = new PhysicsMaterial(name); AssetDatabase.CreateAsset(m, path); }
        m.dynamicFriction = friction; m.staticFriction = friction; m.bounciness = 0f;
        m.frictionCombine = PhysicsMaterialCombine.Average; m.bounceCombine = PhysicsMaterialCombine.Average;
        EditorUtility.SetDirty(m);
        return m;
    }

    static DismountSettings Dismount()
    {
        var path = $"{GenDir}/DismountSettings.asset";
        var s = AssetDatabase.LoadAssetAtPath<DismountSettings>(path);
        if (s == null) { s = ScriptableObject.CreateInstance<DismountSettings>(); AssetDatabase.CreateAsset(s, path); }
        s.Size = new Vector3(0.6f, 1.8f, 0.6f); s.YOffset = 0.1f; s.Mask = 513;
        EditorUtility.SetDirty(s);
        return s;
    }

    static void SetProps(UnityEngine.Object target, params (string name, object value)[] props)
    {
        var so = new SerializedObject(target);
        foreach (var (name, value) in props)
        {
            var p = so.FindProperty(name);
            if (p == null) { log.AppendLine($"warning: {target.GetType().Name}.{name} not found"); continue; }
            switch (value)
            {
                case float f: p.floatValue = f; break;
                case int i when p.propertyType == SerializedPropertyType.Boolean: p.boolValue = i != 0; break;
                case int i: p.intValue = i; break;
            }
        }
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static void EnsureFolder(string path)
    {
        var parts = path.Split('/');
        for (int i = 1; i < parts.Length; i++)
        {
            var sub = string.Join("/", parts.Take(i + 1));
            if (!AssetDatabase.IsValidFolder(sub)) AssetDatabase.CreateFolder(string.Join("/", parts.Take(i)), parts[i]);
        }
    }

    // ---------- scene and bundle ----------
    static void BuildSceneAndBundle(GameObject prefab)
    {
        AssetDatabase.SaveAssets();
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var objects = new GameObject("Objects");
        objects.AddComponent<ModkitPrefabContainer>().Prefabs = new[] { prefab };
        var items = new GameObject("Items");
        var item = new GameObject("MultiRowWheelHoeItem");
        item.transform.SetParent(items.transform, false);
        item.AddComponent<ReuseVanillaPrefab>().SourceName = "HandPlowItem";   // icon and hand visuals from the vanilla Hand Plow
        EditorSceneManager.SaveScene(scene, ScenePath);

        PlayerSettings.stripUnusedMeshComponents = false;
        AssetImporter.GetAtPath(ScenePath).SetAssetBundleNameAndVariant(BundleName, string.Empty);
        foreach (var r in scene.GetRootGameObjects()) r.SetActive(false);
        if (!Directory.Exists("AssetBundles")) Directory.CreateDirectory("AssetBundles");
        var manifest = BuildPipeline.BuildAssetBundles("AssetBundles", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
        foreach (var r in scene.GetRootGameObjects()) r.SetActive(true);
        if (manifest == null) throw new Exception("BuildAssetBundles returned null (see Console for errors).");
        Directory.CreateDirectory(Path.GetDirectoryName(OutputFile));
        File.Copy(Path.Combine("AssetBundles", BundleName), OutputFile, true);
        log.AppendLine("bundle: " + OutputFile + " (" + new FileInfo(OutputFile).Length + " bytes)");
    }
}
