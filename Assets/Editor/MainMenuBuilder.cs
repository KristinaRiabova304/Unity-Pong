using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityLab.Components;

namespace UnityLab.EditorTools
{
    public static class MainMenuBuilder
    {
        [MenuItem("Unity Lab/Run Main Menu")]
        public static void RunMainMenu()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
            EditorApplication.isPlaying = true;
        }

        [MenuItem("Unity Lab/Build Main Menu")]
        public static void Build()
        {
            LabBuilderCommon.EnsureTmpEssentials();

            Directory.CreateDirectory(LabBuilderCommon.ProjectPath("Assets/Scenes"));

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject camGO = new GameObject("Main Camera");
            camGO.tag = "MainCamera";
            camGO.AddComponent<AudioListener>();
            Camera cam = camGO.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5.2f;
            cam.transform.position = new Vector3(0f, 0f, -10f);
            cam.backgroundColor = new Color(0.05f, 0.05f, 0.08f);
            cam.clearFlags = CameraClearFlags.SolidColor;

            GameObject canvasGO = LabBuilderCommon.CreateCanvas();

            GameObject menuCard = LabBuilderCommon.CreateCard(canvasGO.transform, "MenuCard", new Vector2(620f, 620f), new Color(0.11f, 0.11f, 0.17f, 0.97f));

            LabBuilderCommon.CreateShadowedTitle(menuCard.transform, "Title", "PONG", new Vector2(0f, 210f), new Vector2(560f, 140f), 84, new Color(1f, 0.82f, 0.35f));
            TextMeshProUGUI subtitle = LabBuilderCommon.CreateUiText(menuCard.transform, "Subtitle", "Two-player physics pong", new Vector2(0f, 130f), new Vector2(560f, 50f), 24, new Vector2(0.5f, 0.5f), TextAlignmentOptions.Center);
            subtitle.color = new Color(0.75f, 0.78f, 0.85f);

            GameObject divider = new GameObject("Divider", typeof(RectTransform));
            divider.transform.SetParent(menuCard.transform, false);
            RectTransform dividerRect = divider.GetComponent<RectTransform>();
            dividerRect.anchorMin = new Vector2(0.5f, 0.5f);
            dividerRect.anchorMax = new Vector2(0.5f, 0.5f);
            dividerRect.pivot = new Vector2(0.5f, 0.5f);
            dividerRect.sizeDelta = new Vector2(420f, 4f);
            dividerRect.anchoredPosition = new Vector2(0f, 85f);
            Image dividerImg = divider.AddComponent<Image>();
            dividerImg.color = new Color(1f, 0.82f, 0.35f, 0.5f);

            GameObject menuGO = new GameObject("MainMenu");
            MainMenuController controller = menuGO.AddComponent<MainMenuController>();
            LabBuilderCommon.SetField(controller, "newGameSceneName", "PongArena1");

            Button newGameButton = LabBuilderCommon.CreateButton(menuCard.transform, "NewGameButton", "New Game", new Vector2(0f, -30f), new Vector2(380f, 84f), new Color(0.30f, 0.85f, 0.5f));
            Button exitButton = LabBuilderCommon.CreateButton(menuCard.transform, "ExitButton", "Exit", new Vector2(0f, -145f), new Vector2(380f, 84f), new Color(0.9f, 0.35f, 0.4f));

            UnityEventTools.AddPersistentListener(newGameButton.onClick, controller.StartGame);
            UnityEventTools.AddPersistentListener(exitButton.onClick, controller.QuitGame);

            const string path = "Assets/Scenes/MainMenu.unity";
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, path);

            LabBuilderCommon.SyncBuildSettings();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorSceneManager.OpenScene(path);
            Debug.Log("Main menu build complete.");
        }
    }
}
