using LitJson;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

public interface ISaveable
{
    string SaveID { get; }
    JsonData SavedData { get; }
    void LoadFromData(JsonData data);
}

public static class SavingService
{
    // These are the string ID's is the JSON dictionary
    private const string ACTIVE_SCENE_KEY = "activeScene";
    private const string SCENES_KEY = "scenes";
    private const string OBJECTS_KEY = "objects";
    private const string SAVEID_KEY = "$saveID";

    public static void SaveGame(string fileName)
    {
        Debug.Log("hi");
        var result = new JsonData();
        var allSaveableObjects = Object
            .FindObjectsOfType<MonoBehaviour>()
            .OfType<ISaveable>();

        if (allSaveableObjects.Count() > 0)
        {
            var savedObjects = new JsonData();
            foreach (var saveableObject in allSaveableObjects)
            {
                var data = saveableObject.SavedData;
                // Using LitJSON, checks if data is a JSON dictionary.
                if (data.IsObject)
                {
                    // https://litjson.net/api/LitJson/JsonData/
                    data[SAVEID_KEY] = saveableObject.SaveID;
                    savedObjects.Add(data);
                }
                else
                {
                    // Casts to MonoBehaviour;
                    var behaviour = saveableObject as MonoBehaviour;
                    if (!(behaviour is null))
                        Debug.LogWarningFormat(
                            behaviour,
                            "{0}'s save data is not a dictionary. The " +
                            "object was not saved.",
                            behaviour.name
                    );
                }
            }

            result[OBJECTS_KEY] = savedObjects;
        }
        else
        {
            Debug.LogWarningFormat("The scene did not include any saveable objects.");
        }

        // Gets Scene Data
        var openScenes = new JsonData();
        var sceneCount = SceneManager.sceneCount; // Currently loaded scenes.
        for (var i = 0; i < sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            openScenes.Add(scene.name);
        }

        result[SCENES_KEY] = openScenes;
        result[ACTIVE_SCENE_KEY] = SceneManager.GetActiveScene().name;

        //Finds where to put the file
        var outputPath = Path.Combine(Application.persistentDataPath, fileName);
        var writer = new JsonWriter();
        writer.PrettyPrint = true;
        result.ToJson(writer);
        File.WriteAllText(outputPath, writer.ToString());
        Debug.LogFormat("Wrote saved game to {0}", outputPath);
        // Run GC now.
        result = null;
        System.GC.Collect();
    }

    // Adds a delegate to sceneLoaded in order to be notified when the scene has loaded. 
    // See https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneLoaded.html
    // Basically, Unity passes in the Scene and LoadSceneMode when the function is added as a subscription to
    // SceneManager.sceneLoaded
    private static UnityEngine.Events.UnityAction<Scene, LoadSceneMode> LoadObjectsAfterSceneLoad;

    public static bool LoadGame(string fileName)
    {
        var dataPath = Path.Combine(Application.persistentDataPath, fileName);
        if (!File.Exists(dataPath))
        {
            Debug.LogErrorFormat("No file exists at {0}", dataPath);
            return false;
        }

        var text = File.ReadAllText(dataPath);
        var data = JsonMapper.ToObject(text);
        if (data == null || data.IsObject == false)
        {
            Debug.LogErrorFormat("Data at {0} is not a JSON Object", dataPath);
            return false;
        }

        var scenes = data[SCENES_KEY];
        var sceneCount = scenes.Count;
        if (sceneCount == 0)
        {
            Debug.LogWarningFormat("Data at {0} doesn't specify any scenes to load.", dataPath);
            return false;
        }

        for (var i = 0; i < sceneCount; i++)
        {
            var scene = (string) scenes[i];
            if (i == 0) // The First scene is the active scene.
            {
                SceneManager.LoadScene(scene, LoadSceneMode.Single);
            }
            else
            {
                SceneManager.LoadScene(scene, LoadSceneMode.Additive);
            }
        }

        if (data.ContainsKey(ACTIVE_SCENE_KEY))
        {
            var activeSceneName = (string) data[ACTIVE_SCENE_KEY];
            var activeScene = SceneManager.GetSceneByName(activeSceneName);
            if (activeScene.IsValid() == false)
            {
                Debug.LogErrorFormat(
                    "Data at {0} specifies an active scene that " +
                    "doesn't exist. Stopping loading here.",
                    dataPath
                );
                return false;
            }

            SceneManager.SetActiveScene(activeScene);
        }
        else
        {
            Debug.LogWarningFormat(
                "Data at {0} does not specify an active scene. " +
                "(But the first scene in the list will be treated as active)",
                dataPath
            );
        }

        if (data.ContainsKey(OBJECTS_KEY))
        {
            var objects = data[OBJECTS_KEY];
            // We need to wait until the scene loads, or else objects will revert to their
            // original states- ignoring the changes we make here.
            LoadObjectsAfterSceneLoad = (scene, LoadSceneMode) =>
            {
                var allLoadableObjects = Object
                    .FindObjectsOfType<MonoBehaviour>()
                    .OfType<ISaveable>()
                    .ToDictionary(o => o.SaveID, o => o);
                var objectsCount = objects.Count;
                for (var i = 0; i < objectsCount; i++)
                {
                    var objectData = objects[i];
                    var saveID = (string) objectData[SAVEID_KEY];
                    if (allLoadableObjects.ContainsKey(saveID))
                    {
                        var loadableObject = allLoadableObjects[saveID];
                        loadableObject.LoadFromData(objectData); // TODO How does objectData look like- in order to parse it?
                    }
                }

                // Unsubscribes from SceneLoaded notifications.
                SceneManager.sceneLoaded -= LoadObjectsAfterSceneLoad;
                LoadObjectsAfterSceneLoad = null;
                System.GC.Collect();
            };

            // Registers the object-loading code to run after scene loads.
            SceneManager.sceneLoaded += LoadObjectsAfterSceneLoad;
        }

        return true;
    }
}