using System;
using System.IO;
using PaleozoicBeach.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace PaleozoicBeach.Runtime
{
    // Presentation owns Unity objects; all expedition and economy rules live in Core.
    public sealed class BeachGame : MonoBehaviour
    {
        [Serializable] private sealed class Catalog { public Species[] items; }
        private Progress progress;
        private Expedition expedition;
        private Species animalSpecies;
        private GameObject world;
        private Transform submarine, animal;
        private Camera view;
        private Text status;
        private RectTransform panel;
        private string page = "Menu", notice = "";
        private float forward, turn, vertical, pitch, photoCooldown, swimTime;
        private bool paused;
        private Vector3 previousPosition;
        private string SavePath => Path.Combine(Application.persistentDataPath, "research.json");

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (FindFirstObjectByType<BeachGame>() == null) new GameObject("Paleozoic Beach").AddComponent<BeachGame>();
        }
        private void Awake()
        {
            Application.targetFrameRate = 30;
            animalSpecies = JsonUtility.FromJson<Catalog>(Resources.Load<TextAsset>("species").text).items[0];
            progress = Load();
            var cam = new GameObject("Research camera"); view = cam.AddComponent<Camera>();
            view.backgroundColor = new Color(.025f,.12f,.16f); view.clearFlags = CameraClearFlags.SolidColor;
            view.nearClipPlane = .1f; view.farClipPlane = 150;
            var canvas = new GameObject("Interface", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvas.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1280,720); scaler.matchWidthOrHeight = .5f;
            if (FindFirstObjectByType<EventSystem>() == null) new GameObject("Input", typeof(EventSystem), typeof(StandaloneInputModule));
            panel = new GameObject("Content", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            panel.SetParent(canvas.transform,false); panel.anchorMin = Vector2.zero; panel.anchorMax = Vector2.one; panel.offsetMin = new Vector2(30,20); panel.offsetMax = new Vector2(-30,-20);
            panel.GetComponent<Image>().color = new Color(.02f,.06f,.09f,.65f);
            Show("Menu");
        }
        private Progress Load()
        {
            foreach (var path in new[] { SavePath, SavePath + ".bak" })
            {
                if (!File.Exists(path)) continue;
                try
                {
                    var data = JsonUtility.FromJson<Progress>(File.ReadAllText(path));
                    if (data == null || data.version != 1 || data.funds < 0 || data.discoveries == null || data.exhibits < 0 || data.exhibits > data.ResearchCount) throw new InvalidDataException();
                    return data;
                }
                catch (Exception ex) { Debug.LogWarning("Save unavailable: " + ex.Message); notice = "Save could not be loaded; recovered backup or started fresh."; }
            }
            return new Progress();
        }
        private void Save()
        {
            try
            {
                Directory.CreateDirectory(Application.persistentDataPath);
                File.WriteAllText(SavePath + ".tmp", JsonUtility.ToJson(progress,true));
                if (File.Exists(SavePath)) File.Copy(SavePath, SavePath + ".bak",true);
                // Rename within the same directory; retain the previous version for recovery.
                if (File.Exists(SavePath)) File.Delete(SavePath);
                File.Move(SavePath + ".tmp",SavePath);
            }
            catch (Exception ex) { notice = "Save failed: " + ex.Message; Debug.LogError(notice); }
        }
        private Text Label(string value, Vector2 position, Vector2 size, int fontSize = 24)
        {
            var obj = new GameObject("Label",typeof(RectTransform),typeof(Text)); obj.transform.SetParent(panel,false);
            var rt = obj.GetComponent<RectTransform>(); rt.anchorMin = rt.anchorMax = new Vector2(0,1); rt.pivot = new Vector2(0,1); rt.anchoredPosition = position; rt.sizeDelta = size;
            var text = obj.GetComponent<Text>(); text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); text.text = value; text.fontSize = fontSize; text.color = Color.white; return text;
        }
        private GameObject Button(string title, Vector2 pos, Action click)
        {
            var obj = new GameObject(title,typeof(RectTransform),typeof(Image),typeof(Button)); obj.transform.SetParent(panel,false);
            var rt = obj.GetComponent<RectTransform>(); rt.anchorMin = rt.anchorMax = new Vector2(0,1); rt.pivot = new Vector2(0,1); rt.anchoredPosition = pos; rt.sizeDelta = new Vector2(260,50);
            obj.GetComponent<Image>().color = new Color(.08f,.35f,.4f,.95f); obj.GetComponent<Button>().onClick.AddListener(()=>click());
            var text = Label(title, Vector2.zero,new Vector2(260,50),20); text.transform.SetParent(obj.transform,false); text.alignment = TextAnchor.MiddleCenter;
            return obj;
        }
        private void Hold(string title, Vector2 pos, Action<float> setter, float value)
        {
            var obj = Button(title,pos,()=>{}); var trigger = obj.AddComponent<EventTrigger>();
            Add(EventTriggerType.PointerDown,()=>setter(value)); Add(EventTriggerType.PointerUp,()=>setter(0)); Add(EventTriggerType.PointerExit,()=>setter(0));
            void Add(EventTriggerType type, Action action) { var entry = new EventTrigger.Entry { eventID = type }; entry.callback.AddListener(_=>action()); trigger.triggers.Add(entry); }
        }
        private void Show(string next)
        {
            page = next; forward = turn = vertical = pitch = 0;
            foreach (Transform child in panel) { child.gameObject.SetActive(false); Destroy(child.gameObject); }
            panel.GetComponent<Image>().color = new Color(.02f,.06f,.09f,page == "Dive" ? .05f : .94f);
            Label("PALEOZOIC BEACH  /  " + page.ToUpperInvariant(),new Vector2(20,-12),new Vector2(1100,50),32);
            status = Label("",new Vector2(20,-70),new Vector2(1140,260),23);
            if (page == "Menu")
            {
                status.text = "An Illinois research station. A window into deep time.\nExplore → Discover → Return → Analyze → Build → Earn\n\nPrototype: generated geometry; scientific reconstruction in progress.\n" + notice;
                Button("Enter research facility",new Vector2(20,-360),()=>Show("Facility"));
            }
            if (page == "Facility")
            {
                status.text = $"Research funding: ${progress.funds}   |   Completed expeditions: {progress.expeditions}\nPennsylvanian • approximately 309 Ma • Illinois coastal estuary\nFixed geographic coordinates; reconstructed habitat, no spatial time travel.\n8-minute expedition. Photograph Bandringa and survive the scheduled return.\n{(progress.tutorialComplete ? "Expedition cost: $250 (engine 100, maintenance 75, supplies 75)." : "First successful expedition is grant funded.")}\n" + notice;
                Button("Launch expedition",new Vector2(20,-370),Launch);
                Button("Research park",new Vector2(300,-370),()=>Show("Park"));
                Button("Main menu",new Vector2(580,-370),()=>Show("Menu"));
            }
            if (page == "Park")
            {
                string entries = ""; foreach (var d in progress.discoveries) entries += $"\n{animalSpecies.name}: quality {d.quality:P0}, {d.photographs} photographs";
                status.text = $"Funding ${progress.funds}  |  Educational exhibits {progress.exhibits}\nAnalyzed discoveries: {progress.ResearchCount}{entries}\n\nBuild an exhibit from a discovery ($300). Run an open day: $150 revenue\nand $50 operating costs per exhibit. No real-time waiting.\nExhibits use research and models; no live prehistoric animal capture.\n" + notice;
                Button("Build exhibit · $300",new Vector2(20,-390),()=>{ notice = progress.BuildExhibit() ? "Exhibit opened." : "Requires an unexhibited discovery and $300."; Save(); Show("Park"); });
                Button("Operate park day",new Vector2(300,-390),()=>{ notice = "Net park income: $" + progress.OperatePark(); Save(); Show("Park"); });
                Button("Research facility",new Vector2(580,-390),()=>Show("Facility"));
                Label(animalSpecies.uncertainty,new Vector2(20,-470),new Vector2(1140,120),19);
            }
            if (page == "Results")
            {
                status.text = (expedition.Survived ? "Scheduled return complete. Research analyzed and saved." : "Emergency recovery. Research lost; return to the facility.") + $"\nPhotographed species: {expedition.Research.Count}\nElapsed {expedition.Elapsed:F0} seconds / 480\nFunding ${progress.funds}\n" + notice;
                Button("Open research park",new Vector2(20,-380),()=>Show("Park"));
                Button("Research facility",new Vector2(300,-380),()=>Show("Facility"));
            }
            if (page == "Dive")
            {
                Hold("Forward",new Vector2(20,-440),v=>forward=v,1); Hold("Reverse",new Vector2(20,-500),v=>forward=v,-1);
                Hold("Turn left",new Vector2(300,-440),v=>turn=v,-1); Hold("Turn right",new Vector2(300,-500),v=>turn=v,1);
                Hold("Ascend",new Vector2(580,-440),v=>vertical=v,1); Hold("Descend",new Vector2(580,-500),v=>vertical=v,-1);
                Hold("Look up",new Vector2(860,-440),v=>pitch=v,-1); Hold("Look down",new Vector2(860,-500),v=>pitch=v,1);
                Button("Photograph",new Vector2(860,-360),Photograph);
                Button("Pause",new Vector2(860,-570),()=>{ paused = !paused; forward=turn=vertical=pitch=0; });
                Label("+",new Vector2(575,-300),new Vector2(50,50),32);
            }
        }
        private void Launch()
        {
            expedition = Expedition.Launch(progress);
            if (expedition == null) { notice = "Insufficient funds. Operate the park to finance the expedition."; Show("Facility"); return; }
            Save(); paused = false; swimTime = 0; photoCooldown = 0; notice = "Use controls to approach the animal; center it in the crosshair. Photos must be within 35 m.";
            BuildWorld(); Show("Dive");
        }
        private GameObject Primitive(string name, PrimitiveType shape, Vector3 position, Vector3 scale, Color color, Transform parent)
        {
            var obj = GameObject.CreatePrimitive(shape); obj.name = name; obj.transform.SetParent(parent,false); obj.transform.localPosition = position; obj.transform.localScale = scale;
            var material = new Material(Shader.Find("Standard")); material.color = color; material.SetFloat("_Glossiness",.15f); obj.GetComponent<Renderer>().material = material;
            return obj;
        }
        private void BuildWorld()
        {
            if (world != null) Destroy(world);
            world = new GameObject("Pennsylvanian Illinois estuary");
            RenderSettings.fog = true; RenderSettings.fogMode = FogMode.ExponentialSquared; RenderSettings.fogDensity = .035f; RenderSettings.fogColor = new Color(.08f,.22f,.19f); RenderSettings.ambientLight = new Color(.25f,.4f,.35f);
            var light = new GameObject("Filtered sunlight").AddComponent<Light>(); light.transform.SetParent(world.transform); light.type = LightType.Directional; light.intensity = 1.1f; light.transform.rotation = Quaternion.Euler(60,20,0);
            Primitive("Estuary sediment",PrimitiveType.Cube,new Vector3(0,-1,0),new Vector3(180,2,180),new Color(.25f,.23f,.16f),world.transform);
            var random = new System.Random(309);
            for (int i=0;i<45;i++) Primitive("Sediment rock",PrimitiveType.Sphere,new Vector3(random.Next(-65,66),0,random.Next(-65,66)),new Vector3(2,1,3),new Color(.22f,.25f,.21f),world.transform);
            submarine = new GameObject("Experimental submarine").transform; submarine.SetParent(world.transform); submarine.position = new Vector3(0,6,-15);
            Primitive("Pressure hull",PrimitiveType.Capsule,Vector3.zero,new Vector3(2,3,2),Color.gray,submarine).transform.localRotation = Quaternion.Euler(90,0,0);
            Primitive("Port cockpit frame",PrimitiveType.Cube,new Vector3(-.8f,.1f,3.4f),new Vector3(.12f,1.5f,.1f),Color.gray,submarine);
            Primitive("Starboard cockpit frame",PrimitiveType.Cube,new Vector3(.8f,.1f,3.4f),new Vector3(.12f,1.5f,.1f),Color.gray,submarine);
            view.transform.SetParent(submarine,false); view.transform.localPosition = new Vector3(0,.5f,3.2f); view.transform.localRotation = Quaternion.identity;
            animal = new GameObject("Bandringa prototype").transform; animal.SetParent(world.transform);
            Primitive("Body",PrimitiveType.Sphere,Vector3.zero,new Vector3(.7f,.6f,2.2f),new Color(.29f,.32f,.25f),animal);
            Primitive("Elongate rostrum",PrimitiveType.Cube,new Vector3(0,0,1.9f),new Vector3(.15f,.1f,2),new Color(.3f,.33f,.26f),animal);
            Primitive("Tail",PrimitiveType.Cube,new Vector3(0,.25f,-1.35f),new Vector3(.08f,.7f,.5f),new Color(.3f,.33f,.26f),animal);
            Primitive("Pectoral fins",PrimitiveType.Cube,new Vector3(0,-.15f,.2f),new Vector3(1.6f,.07f,.7f),new Color(.3f,.33f,.26f),animal);
            animal.position = new Vector3(0,4,0); previousPosition = submarine.position;
        }
        private void Photograph()
        {
            if (page != "Dive" || paused || photoCooldown > 0) return;
            photoCooldown = 1;
            var delta = animal.position - view.transform.position;
            bool visible = !Physics.Linecast(view.transform.position,animal.position,out var hit) || hit.transform.IsChildOf(animal);
            bool recorded = expedition.Photograph(animalSpecies,309,"Illinois estuary",delta.magnitude,Vector3.Dot(view.transform.forward,delta.normalized),visible);
            notice = recorded ? "Bandringa documented. Survive until the scheduled return to retain research." : "No usable photograph. Center the animal, approach within 35 m, and clear obstructions.";
        }
        private void Update()
        {
            if (page != "Dive" || paused) return;
            float dt = Time.deltaTime; photoCooldown = Mathf.Max(0,photoCooldown-dt);
            // UI hold controls work on iOS/Android; keyboard uses the legacy Input Manager.
            float move = Mathf.Clamp(forward + Input.GetAxisRaw("Vertical"),-1,1);
            float yaw = Mathf.Clamp(turn + Input.GetAxisRaw("Horizontal"),-1,1);
            float lift = vertical + (Input.GetKey(KeyCode.E) ? 1 : 0) - (Input.GetKey(KeyCode.Q) ? 1 : 0);
            submarine.Rotate(0,yaw*45*dt,0);
            float angle = view.transform.localEulerAngles.x; if (angle > 180) angle -= 360;
            view.transform.localRotation = Quaternion.Euler(Mathf.Clamp(angle + pitch*30*dt,-50,50),0,0);
            Vector3 desired = submarine.position + (submarine.forward*move*4 + Vector3.up*lift*2)*dt;
            desired = new Vector3(Mathf.Clamp(desired.x,-75,75),Mathf.Clamp(desired.y,2,22),Mathf.Clamp(desired.z,-75,75));
            if (Physics.Linecast(submarine.position,desired,out var obstacle) && !obstacle.transform.IsChildOf(submarine)) expedition.Damage(5*dt);
            else submarine.position = desired;
            if (Vector3.Distance(previousPosition,submarine.position) < .001f && Mathf.Abs(move) > .1f) expedition.Damage(dt);
            previousPosition = submarine.position;
            swimTime += dt; float t = swimTime*.12f;
            Vector3 target = new Vector3(Mathf.Sin(t)*9,4+Mathf.Sin(t*.6f),Mathf.Sin(t*.7f)*9);
            Vector3 direction = target - animal.position;
            if (direction.sqrMagnitude > .01f) animal.rotation = Quaternion.Slerp(animal.rotation,Quaternion.LookRotation(direction),dt*2);
            animal.position = Vector3.MoveTowards(animal.position,target,dt*1.2f);
            expedition.Tick(dt,Mathf.Abs(move)+Mathf.Abs(lift) > .1f);
            if (Input.GetKeyDown(KeyCode.Space)) Photograph();
            float remaining = Mathf.Max(0,480-expedition.Elapsed);
            status.text = $"RETURN IN {Mathf.FloorToInt(remaining/60):00}:{Mathf.FloorToInt(remaining%60):00}  |  Battery {expedition.Battery:F0}%  Oxygen {expedition.Oxygen:F0}%  Hull {expedition.Hull:F0}%\nSpecies documented: {expedition.Research.Count}\n{notice}\nW/S move · A/D turn · Q/E depth · Space photo";
            if (expedition.Finished)
            {
                expedition.Settle(progress); Save(); view.transform.SetParent(null); Destroy(world); RenderSettings.fog = false; Show("Results");
            }
        }
        private void OnApplicationPause(bool value) { if (value) { paused = true; forward=turn=vertical=pitch=0; Save(); } }
        private void OnApplicationQuit() { Save(); }
    }
}
