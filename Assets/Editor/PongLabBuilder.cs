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
    public static class PongLabBuilder
    {
        private const float HalfW = 8.5f;
        private const float HalfH = 4.5f;

        [MenuItem("Unity Lab/Run Pong Arena 1")]
        public static void RunPongArena1()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Pong/PongArena1.unity");
            EditorApplication.isPlaying = true;
        }

        [MenuItem("Unity Lab/Build Pong Lab")]
        public static void Build()
        {
            Sprite square = LabBuilderCommon.CreateSquareSprite();
            Sprite circle = CreateCircleSprite();
            PhysicsMaterial2D bounce = CreateBounceMaterial();
            LabBuilderCommon.EnsureTmpEssentials();

            Directory.CreateDirectory(LabBuilderCommon.ProjectPath("Assets/Scenes/Pong"));

            BuildArena("PongArena1", "Assets/Scenes/Pong/PongArena1.unity", square, circle, bounce, false, "PongArena2");
            BuildArena("PongArena2", "Assets/Scenes/Pong/PongArena2.unity", square, circle, bounce, true, null);

            LabBuilderCommon.SyncBuildSettings();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorSceneManager.OpenScene("Assets/Scenes/Pong/PongArena1.unity");
            Debug.Log("Pong lab build complete.");
        }

        private static void BuildArena(string sceneName, string path, Sprite square, Sprite circle, PhysicsMaterial2D bounce, bool addObstacles, string nextSceneName)
        {
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

            CreateStaticBox("WallTop", new Vector3(0f, HalfH + 0.25f, 0f), new Vector2(2f * HalfW + 1f, 0.5f), square, bounce, new Color(0.55f, 0.57f, 0.68f));
            CreateStaticBox("WallBottom", new Vector3(0f, -(HalfH + 0.25f), 0f), new Vector2(2f * HalfW + 1f, 0.5f), square, bounce, new Color(0.55f, 0.57f, 0.68f));

            GameObject ball = CreateBall(circle, bounce);

            CreateGoal("GoalLeft", new Vector3(-(HalfW + 0.5f), 0f, 0f), new Vector2(1f, 2f * HalfH + 2f), ScoreSide.PlayerTwo, ball, 1);
            CreateGoal("GoalRight", new Vector3(HalfW + 0.5f, 0f, 0f), new Vector2(1f, 2f * HalfH + 2f), ScoreSide.PlayerOne, ball, 1);

            // Safety net: if the ball ever escapes past the top/bottom walls (a fast paddle hit
            // pushing it outside a single physics step), catch it and relaunch instead of losing
            // it off-screen forever with no way for either player to score again.
            CreateGoal("OutOfBoundsTop", new Vector3(0f, HalfH + 1.5f, 0f), new Vector2(2f * HalfW + 3f, 1f), ScoreSide.PlayerOne, ball, 0);
            CreateGoal("OutOfBoundsBottom", new Vector3(0f, -(HalfH + 1.5f), 0f), new Vector2(2f * HalfW + 3f, 1f), ScoreSide.PlayerOne, ball, 0);

            CreatePaddle("PaddleLeft", new Vector3(-(HalfW - 1f), 0f, 0f), square, bounce, InputScheme.WASD, new Color(0.3f, 0.9f, 0.5f));
            CreatePaddle("PaddleRight", new Vector3(HalfW - 1f, 0f, 0f), square, bounce, InputScheme.Arrows, new Color(0.95f, 0.4f, 0.4f));

            if (addObstacles)
            {
                CreateStaticBox("ObstacleCenter", new Vector3(0f, 0f, 0f), new Vector2(1f, 1f), square, bounce, new Color(1f, 0.82f, 0.35f));
                CreateStaticBox("ObstacleUpper", new Vector3(-2.7f, 2.1f, 0f), new Vector2(1.3f, 0.6f), square, bounce, new Color(1f, 0.82f, 0.35f));
                CreateStaticCircle("ObstacleLower", new Vector3(2.7f, -2.1f, 0f), 1.1f, circle, bounce, new Color(1f, 0.82f, 0.35f));
            }

            GameObject canvasGO = LabBuilderCommon.CreateCanvas();

            GameObject p1Chip = LabBuilderCommon.CreateCard(canvasGO.transform, "P1Chip", new Vector2(260f, 110f), new Vector2(0f, 1f), new Vector2(30f, -30f), new Color(0.3f, 0.85f, 0.5f, 0.18f));
            LabBuilderCommon.CreateUiText(p1Chip.transform, "P1Label", "PLAYER 1", new Vector2(0f, 24f), new Vector2(240f, 36f), 22, new Vector2(0.5f, 0.5f), TextAlignmentOptions.Center).color = new Color(0.7f, 0.95f, 0.8f);
            TextMeshProUGUI p1Score = LabBuilderCommon.CreateUiText(p1Chip.transform, "P1Score", "0", new Vector2(0f, -22f), new Vector2(240f, 60f), 44, new Vector2(0.5f, 0.5f), TextAlignmentOptions.Center);
            p1Score.fontStyle = FontStyles.Bold;

            GameObject p2Chip = LabBuilderCommon.CreateCard(canvasGO.transform, "P2Chip", new Vector2(260f, 110f), new Vector2(1f, 1f), new Vector2(-30f, -30f), new Color(0.9f, 0.35f, 0.4f, 0.18f));
            LabBuilderCommon.CreateUiText(p2Chip.transform, "P2Label", "PLAYER 2", new Vector2(0f, 24f), new Vector2(240f, 36f), 22, new Vector2(0.5f, 0.5f), TextAlignmentOptions.Center).color = new Color(1f, 0.75f, 0.78f);
            TextMeshProUGUI p2Score = LabBuilderCommon.CreateUiText(p2Chip.transform, "P2Score", "0", new Vector2(0f, -22f), new Vector2(240f, 60f), 44, new Vector2(0.5f, 0.5f), TextAlignmentOptions.Center);
            p2Score.fontStyle = FontStyles.Bold;

            Button pauseToggleButton = LabBuilderCommon.CreateButton(canvasGO.transform, "PauseButton", "Menu", Vector2.zero, new Vector2(150f, 55f), new Color(0.32f, 0.33f, 0.42f));
            RectTransform pauseToggleRect = pauseToggleButton.GetComponent<RectTransform>();
            pauseToggleRect.anchorMin = new Vector2(0.5f, 1f);
            pauseToggleRect.anchorMax = new Vector2(0.5f, 1f);
            pauseToggleRect.pivot = new Vector2(0.5f, 1f);
            pauseToggleRect.anchoredPosition = new Vector2(0f, -20f);

            GameObject winPanel = LabBuilderCommon.CreatePanel(canvasGO.transform, "WinPanel", new Color(0f, 0f, 0f, 0.6f));
            GameObject winCard = LabBuilderCommon.CreateCard(winPanel.transform, "WinCard", new Vector2(620f, 520f), new Color(0.11f, 0.11f, 0.17f, 0.98f));
            TextMeshProUGUI resultText = LabBuilderCommon.CreateUiText(winCard.transform, "ResultText", "You win!", new Vector2(0f, 130f), new Vector2(560f, 220f), 56, new Vector2(0.5f, 0.5f), TextAlignmentOptions.Center);
            resultText.fontStyle = FontStyles.Bold;
            string continueLabel = string.IsNullOrEmpty(nextSceneName) ? "Play Again" : "Continue";
            Button continueButton = LabBuilderCommon.CreateButton(winCard.transform, "ContinueButton", continueLabel, new Vector2(0f, -60f), new Vector2(380f, 84f), new Color(0.30f, 0.85f, 0.5f));
            Button winMenuButton = LabBuilderCommon.CreateButton(winCard.transform, "WinMenuButton", "Main Menu", new Vector2(0f, -170f), new Vector2(380f, 84f), new Color(0.32f, 0.33f, 0.42f));
            winPanel.SetActive(false);

            GameObject continueLoaderGO = new GameObject("ContinueLoader");
            SceneLoader continueLoader = continueLoaderGO.AddComponent<SceneLoader>();
            LabBuilderCommon.SetField(continueLoader, "sceneName", string.IsNullOrEmpty(nextSceneName) ? "PongArena1" : nextSceneName);
            UnityEventTools.AddPersistentListener(continueButton.onClick, continueLoader.Load);

            GameObject pausePanel = LabBuilderCommon.CreatePanel(canvasGO.transform, "PausePanel", new Color(0f, 0f, 0f, 0.6f));
            GameObject pauseCard = LabBuilderCommon.CreateCard(pausePanel.transform, "PauseCard", new Vector2(560f, 460f), new Color(0.11f, 0.11f, 0.17f, 0.98f));
            LabBuilderCommon.CreateShadowedTitle(pauseCard.transform, "PausedTitle", "Paused", new Vector2(0f, 130f), new Vector2(500f, 120f), 52, new Color(1f, 0.82f, 0.35f));
            Button resumeButton = LabBuilderCommon.CreateButton(pauseCard.transform, "ResumeButton", "Resume", new Vector2(0f, -40f), new Vector2(380f, 84f), new Color(0.30f, 0.85f, 0.5f));
            Button pauseMenuButton = LabBuilderCommon.CreateButton(pauseCard.transform, "PauseMenuButton", "Main Menu", new Vector2(0f, -150f), new Vector2(380f, 84f), new Color(0.32f, 0.33f, 0.42f));
            pausePanel.SetActive(false);

            GameObject pauseGO = new GameObject("PauseMenu");
            PauseMenuController pauseMenu = pauseGO.AddComponent<PauseMenuController>();
            LabBuilderCommon.SetField(pauseMenu, "pausePanel", pausePanel);
            LabBuilderCommon.SetField(pauseMenu, "blockingPanel", winPanel);
            LabBuilderCommon.SetField(pauseMenu, "mainMenuSceneName", "MainMenu");

            UnityEventTools.AddPersistentListener(pauseToggleButton.onClick, pauseMenu.Pause);
            UnityEventTools.AddPersistentListener(resumeButton.onClick, pauseMenu.Resume);
            UnityEventTools.AddPersistentListener(pauseMenuButton.onClick, pauseMenu.GoToMainMenu);
            UnityEventTools.AddPersistentListener(winMenuButton.onClick, pauseMenu.GoToMainMenu);

            GameObject gm = new GameObject("GameManager");
            TwoPlayerScore twoPlayerScore = gm.AddComponent<TwoPlayerScore>();
            LabBuilderCommon.SetField(twoPlayerScore, "playerOneText", p1Score);
            LabBuilderCommon.SetField(twoPlayerScore, "playerTwoText", p2Score);
            LabBuilderCommon.SetField(twoPlayerScore, "winScore", 5);

            GameController controller = gm.AddComponent<GameController>();
            LabBuilderCommon.SetField(controller, "winPanel", winPanel);
            LabBuilderCommon.SetField(controller, "resultText", resultText);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, path);
        }

        private static GameObject CreateStaticBox(string name, Vector3 pos, Vector2 size, Sprite sprite, PhysicsMaterial2D bounce, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.position = pos;
            go.transform.localScale = new Vector3(size.x, size.y, 1f);
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = color;
            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            col.sharedMaterial = bounce;
            return go;
        }

        private static GameObject CreateStaticCircle(string name, Vector3 pos, float diameter, Sprite sprite, PhysicsMaterial2D bounce, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.position = pos;
            go.transform.localScale = new Vector3(diameter, diameter, 1f);
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = color;
            CircleCollider2D col = go.AddComponent<CircleCollider2D>();
            col.sharedMaterial = bounce;
            return go;
        }

        private static GameObject CreateBall(Sprite circle, PhysicsMaterial2D bounce)
        {
            GameObject go = new GameObject("Ball");
            go.transform.position = Vector3.zero;
            go.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = circle;
            sr.color = Color.white;
            Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.freezeRotation = true;
            CircleCollider2D col = go.AddComponent<CircleCollider2D>();
            col.sharedMaterial = bounce;
            BallStarter2D starter = go.AddComponent<BallStarter2D>();
            LabBuilderCommon.SetField(starter, "initialSpeed", 7f);
            LabBuilderCommon.SetField(starter, "randomizeDirection", true);
            return go;
        }

        private static GameObject CreateGoal(string name, Vector3 pos, Vector2 size, ScoreSide side, GameObject ball, int points)
        {
            GameObject go = new GameObject(name);
            go.transform.position = pos;
            go.transform.localScale = new Vector3(size.x, size.y, 1f);
            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            ScoreZone2D zone = go.AddComponent<ScoreZone2D>();
            LabBuilderCommon.SetField(zone, "scoreSide", (int)side);
            LabBuilderCommon.SetField(zone, "points", points);
            LabBuilderCommon.SetField(zone, "ballToReset", ball.GetComponent<BallStarter2D>());
            return go;
        }

        private static GameObject CreatePaddle(string name, Vector3 pos, Sprite sprite, PhysicsMaterial2D bounce, InputScheme scheme, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.position = pos;
            go.transform.localScale = new Vector3(0.6f, 3.2f, 1f);
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = color;
            Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            col.sharedMaterial = bounce;
            VerticalMove2D move = go.AddComponent<VerticalMove2D>();
            LabBuilderCommon.SetField(move, "speed", 7f);
            LabBuilderCommon.SetField(move, "inputScheme", (int)scheme);
            return go;
        }

        private static Sprite CreateCircleSprite()
        {
            return LabBuilderCommon.CreateOrLoadSprite("Assets/UnityLab/Generated/Sprites/Circle.png", size =>
            {
                Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
                Color32[] pixels = new Color32[size * size];
                float r = size / 2f;
                Vector2 center = new Vector2(r, r);
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                        pixels[y * size + x] = d <= r ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 0);
                    }
                }

                tex.SetPixels32(pixels);
                tex.Apply();
                return tex;
            });
        }

        private static PhysicsMaterial2D CreateBounceMaterial()
        {
            const string path = "Assets/UnityLab/Generated/Physics/Bouncy.physicsMaterial2D";
            PhysicsMaterial2D existing = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(path);
            if (existing != null)
            {
                return existing;
            }

            Directory.CreateDirectory(LabBuilderCommon.ProjectPath("Assets/UnityLab/Generated/Physics"));
            AssetDatabase.Refresh();
            PhysicsMaterial2D mat = new PhysicsMaterial2D("Bouncy") { bounciness = 1f, friction = 0f };
            AssetDatabase.CreateAsset(mat, path);
            AssetDatabase.SaveAssets();
            return mat;
        }
    }
}
