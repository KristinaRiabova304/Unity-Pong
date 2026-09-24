using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UnityLab.EditorTools
{
    /// <summary>
    /// Shared scene-building helpers used by the Unity Lab editor scene builders
    /// (Pong arenas, main menu, etc.) so generated assets and UI stay consistent.
    /// </summary>
    public static class LabBuilderCommon
    {
        public static string ProjectPath(string assetRelativePath)
        {
            string projectRoot = Directory.GetParent(Application.dataPath)!.FullName;
            return Path.Combine(projectRoot, assetRelativePath);
        }

        public static void SetField(Object target, string fieldName, object value)
        {
            SerializedObject so = new SerializedObject(target);
            SerializedProperty prop = so.FindProperty(fieldName);
            if (prop == null)
            {
                Debug.LogError($"Field '{fieldName}' not found on {target.GetType()}");
                return;
            }

            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    prop.intValue = (int)value;
                    break;
                case SerializedPropertyType.Boolean:
                    prop.boolValue = (bool)value;
                    break;
                case SerializedPropertyType.Float:
                    prop.floatValue = (float)value;
                    break;
                case SerializedPropertyType.String:
                    prop.stringValue = (string)value;
                    break;
                case SerializedPropertyType.Enum:
                    prop.enumValueIndex = (int)value;
                    break;
                case SerializedPropertyType.ObjectReference:
                    prop.objectReferenceValue = (Object)value;
                    break;
                case SerializedPropertyType.LayerMask:
                    prop.intValue = (int)value;
                    break;
                default:
                    Debug.LogError($"Unsupported property type {prop.propertyType} for field '{fieldName}'");
                    break;
            }

            so.ApplyModifiedProperties();
        }

        public static void EnsureTmpEssentials()
        {
            string check = ProjectPath("Assets/TextMesh Pro/Resources/TMP Settings.asset");
            if (File.Exists(check))
            {
                return;
            }

            string[] candidates = Directory.GetFiles(ProjectPath("Library/PackageCache"), "TMP Essential Resources.unitypackage", SearchOption.AllDirectories);
            if (candidates.Length == 0)
            {
                Debug.LogWarning("TMP Essential Resources package not found; text may not render with a font.");
                return;
            }

            AssetDatabase.ImportPackage(candidates[0], false);
            AssetDatabase.Refresh();
        }

        public static Sprite CreateSquareSprite()
        {
            return CreateOrLoadSprite("Assets/UnityLab/Generated/Sprites/Square.png", size =>
            {
                Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
                Color32[] pixels = new Color32[size * size];
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = new Color32(255, 255, 255, 255);
                }

                tex.SetPixels32(pixels);
                tex.Apply();
                return tex;
            });
        }

        /// <summary>
        /// A white, anti-aliased rounded-rect sprite, 9-sliced so it scales to any button or
        /// panel size without distorting the corners. Used for every UI surface so the game
        /// reads as one consistent style instead of flat default-Unity rectangles.
        /// </summary>
        public static Sprite CreateRoundedSprite()
        {
            const int size = 64;
            const float radius = 20f;
            return CreateOrLoadSprite("Assets/UnityLab/Generated/Sprites/Rounded.png", s =>
            {
                Texture2D tex = new Texture2D(s, s, TextureFormat.RGBA32, false);
                Color32[] pixels = new Color32[s * s];
                float half = s / 2f;
                for (int y = 0; y < s; y++)
                {
                    for (int x = 0; x < s; x++)
                    {
                        float px = x + 0.5f;
                        float py = y + 0.5f;
                        float cx = Mathf.Abs(px - half) - (half - radius);
                        float cy = Mathf.Abs(py - half) - (half - radius);
                        float dist = (cx > 0f && cy > 0f) ? Mathf.Sqrt(cx * cx + cy * cy) - radius : Mathf.Max(cx, cy) - radius;
                        float alpha = Mathf.Clamp01(0.5f - dist);
                        pixels[y * s + x] = new Color32(255, 255, 255, (byte)(alpha * 255));
                    }
                }

                tex.SetPixels32(pixels);
                tex.Apply();
                return tex;
            }, new Vector4(radius + 2f, radius + 2f, radius + 2f, radius + 2f));
        }

        public static Sprite CreateOrLoadSprite(string assetPath, Func<int, Texture2D> makeTexture, Vector4 border = default)
        {
            const int size = 64;
            string fsPath = ProjectPath(assetPath);
            bool isNew = !File.Exists(fsPath);
            if (isNew)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fsPath) ?? string.Empty);
                Texture2D tex = makeTexture(size);
                File.WriteAllBytes(fsPath, tex.EncodeToPNG());
                Object.DestroyImmediate(tex);
                AssetDatabase.ImportAsset(assetPath);
            }

            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
            if (isNew || importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = size;
                importer.filterMode = FilterMode.Bilinear;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.spriteBorder = border;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        }

        public static GameObject CreateCanvas()
        {
            GameObject go = new GameObject("Canvas", typeof(RectTransform));
            Canvas canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            go.AddComponent<GraphicRaycaster>();
            EnsureEventSystem();
            return go;
        }

        /// <summary>
        /// UI Buttons receive no clicks without an EventSystem in the scene to route input to them.
        /// </summary>
        private static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            GameObject go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            go.AddComponent<StandaloneInputModule>();
        }

        public static GameObject CreatePanel(Transform parent, string name, Color color)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            Image img = go.AddComponent<Image>();
            img.color = color;
            return go;
        }

        /// <summary>
        /// A centered rounded "dialog" surface with a soft drop shadow, used for win/pause
        /// screens and the main menu instead of bare full-screen text on a flat overlay.
        /// </summary>
        public static GameObject CreateCard(Transform parent, string name, Vector2 size, Color color)
        {
            return CreateCard(parent, name, size, new Vector2(0.5f, 0.5f), Vector2.zero, color);
        }

        public static GameObject CreateCard(Transform parent, string name, Vector2 size, Vector2 anchor, Vector2 anchoredPos, Color color)
        {
            Sprite rounded = CreateRoundedSprite();

            GameObject shadowGO = new GameObject(name + "Shadow", typeof(RectTransform));
            shadowGO.transform.SetParent(parent, false);
            RectTransform shadowRect = shadowGO.GetComponent<RectTransform>();
            shadowRect.anchorMin = anchor;
            shadowRect.anchorMax = anchor;
            shadowRect.pivot = anchor;
            shadowRect.sizeDelta = size + new Vector2(12f, 12f);
            shadowRect.anchoredPosition = anchoredPos + new Vector2(0f, -6f);
            Image shadowImg = shadowGO.AddComponent<Image>();
            shadowImg.sprite = rounded;
            shadowImg.type = Image.Type.Sliced;
            shadowImg.color = new Color(0f, 0f, 0f, 0.35f);

            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPos;
            Image img = go.AddComponent<Image>();
            img.sprite = rounded;
            img.type = Image.Type.Sliced;
            img.color = color;
            return go;
        }

        public static TextMeshProUGUI CreateUiText(Transform parent, string name, string content, Vector2 anchoredPos, Vector2 sizeDelta, int fontSize, Vector2 anchor, TextAlignmentOptions alignment)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = sizeDelta;
            TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
            text.text = content;
            text.fontSize = fontSize;
            text.color = Color.white;
            text.alignment = alignment;
            return text;
        }

        /// <summary>
        /// Title-style text with a soft drop shadow (a duplicate dark copy offset behind it),
        /// since the default TMP material does not expose an outline we can set without touching
        /// the shared font asset.
        /// </summary>
        public static TextMeshProUGUI CreateShadowedTitle(Transform parent, string name, string content, Vector2 anchoredPos, Vector2 sizeDelta, int fontSize, Color color)
        {
            TextMeshProUGUI shadow = CreateUiText(parent, name + "Shadow", content, anchoredPos + new Vector2(3f, -4f), sizeDelta, fontSize, new Vector2(0.5f, 0.5f), TextAlignmentOptions.Center);
            shadow.color = new Color(0f, 0f, 0f, 0.45f);
            shadow.fontStyle = FontStyles.Bold;

            TextMeshProUGUI title = CreateUiText(parent, name, content, anchoredPos, sizeDelta, fontSize, new Vector2(0.5f, 0.5f), TextAlignmentOptions.Center);
            title.color = color;
            title.fontStyle = FontStyles.Bold;
            return title;
        }

        public static Button CreateButton(Transform parent, string name, string label, Vector2 anchoredPos, Vector2 size, Color color)
        {
            Sprite rounded = CreateRoundedSprite();

            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = size;

            Image img = go.AddComponent<Image>();
            img.sprite = rounded;
            img.type = Image.Type.Sliced;
            img.color = color;

            Button button = go.AddComponent<Button>();
            button.targetGraphic = img;
            button.transition = Selectable.Transition.ColorTint;
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.12f, 1.12f, 1.12f, 1f);
            colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            colors.selectedColor = Color.white;
            colors.disabledColor = new Color(1f, 1f, 1f, 0.5f);
            colors.fadeDuration = 0.08f;
            button.colors = colors;

            TextMeshProUGUI text = CreateUiText(go.transform, name + "Label", label, Vector2.zero, size, 30, new Vector2(0.5f, 0.5f), TextAlignmentOptions.Center);
            text.fontStyle = FontStyles.Bold;
            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            return button;
        }

        /// <summary>
        /// Registers whichever Unity Lab scenes currently exist on disk in Build Settings,
        /// always in the same order: main menu first, then the Pong arenas.
        /// </summary>
        public static void SyncBuildSettings()
        {
            string[] order =
            {
                "Assets/Scenes/MainMenu.unity",
                "Assets/Scenes/Pong/PongArena1.unity",
                "Assets/Scenes/Pong/PongArena2.unity",
            };

            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
            foreach (string path in order)
            {
                if (File.Exists(ProjectPath(path)))
                {
                    scenes.Add(new EditorBuildSettingsScene(path, true));
                }
            }

            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
