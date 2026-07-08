// ---------------------------------------------------------------------------- 
// Author: Ryan Hipple
// Date:   05/07/2018
// Source: https://github.com/roboryantron/UnityEditorJunkie
// ----------------------------------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyBox {
    /// <summary>
    /// Class used to serialize a reference to a scene asset that can be used
    /// at runtime in a build, when the asset can no longer be directly
    /// referenced. This caches the scene name based on the SceneAsset to use
    /// at runtime to load.
    /// </summary>
    [Serializable]
    public class SceneReference : ISerializationCallbackReceiver {
        /// <summary>
        /// Exception that is raised when there is an issue resolving and
        /// loading a scene reference.
        /// </summary>
        public class SceneLoadException : Exception {
            public SceneLoadException(string message) : base(message) { }
        }

#if UNITY_EDITOR
        public UnityEditor.SceneAsset Scene;
#endif

        [Tooltip("The name of the referenced scene. This may be used at runtime to load the scene.")]
        public string SceneName;

        [SerializeField] private int sceneIndex = -1;

        [SerializeField] private bool sceneEnabled;

        public bool IsAssigned => !string.IsNullOrEmpty(this.SceneName);


        public void LoadScene(LoadSceneMode mode = LoadSceneMode.Single) {
            this.ValidateScene();
            SceneManager.LoadScene(this.SceneName, mode);
        }

        public AsyncOperation LoadSceneAsync(LoadSceneMode mode = LoadSceneMode.Single) {
            this.ValidateScene();
            return SceneManager.LoadSceneAsync(this.SceneName, mode);
        }

        public AsyncOperation UnloadSceneAsync() {
            this.ValidateScene();
            return SceneManager.UnloadSceneAsync(this.SceneName);
        }

        public bool SetActive() {
            return SceneManager.SetActiveScene(SceneManager.GetSceneByName(this.SceneName));
        }


        private void ValidateScene() {
            if (string.IsNullOrEmpty(this.SceneName))
                throw new SceneLoadException("No scene specified.");

            if (this.sceneIndex < 0)
                throw new SceneLoadException("Scene " + this.SceneName + " is not in the build settings");

            if (!this.sceneEnabled)
                throw new SceneLoadException("Scene " + this.SceneName + " is not enabled in the build settings");
        }

        public void OnBeforeSerialize() {
#if UNITY_EDITOR
            if (this.Scene != null) {
                string sceneAssetPath = UnityEditor.AssetDatabase.GetAssetPath(this.Scene);
                string sceneAssetGUID = UnityEditor.AssetDatabase.AssetPathToGUID(sceneAssetPath);

                var sceneInBuild = Internal.SceneReferenceUtils.GetSceneInBuildState(sceneAssetGUID);
                if (sceneInBuild.Enabled)
                    this.SceneName = this.Scene.name;
            } else {
                this.SceneName = "";
            }
#endif
        }

        public void OnAfterDeserialize() { }
    }
}

#if UNITY_EDITOR

namespace MyBox.Internal {
    using System;
    using UnityEditor;
    using UnityEngine;

    public static class SceneReferenceUtils {
        public static (bool Present, bool Enabled, int Index) GetSceneInBuildState(string sceneGuid) {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

            for (int i = 0; i < scenes.Length; i++) {
                if (scenes[i].guid.ToString() == sceneGuid) return (true, scenes[i].enabled, i);
            }

            return (false, false, -1);
        }
    }

    /// <summary>
    /// Editor for a scene reference that can display error prompts and offer
    /// solutions when the scene is not valid.
    /// </summary>
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferenceEditor : PropertyDrawer {
        #region -- Constants --------------------------------------------------
        private const string TOOLTIP_SCENE_MISSING = "Scene is not in build settings.";

        private const string ERROR_SCENE_MISSING =
            "You are refencing a scene that is not added to the build. Add it to the editor build settings now?";

        private const string TOOLTIP_SCENE_DISABLED = "Scene is not enebled in build settings.";

        private const string ERROR_SCENE_DISABLED =
            "You are refencing a scene that is not active the build. Enable it in the build settings now?";
        #endregion -- Constants -----------------------------------------------

        #region -- Private Variables ------------------------------------------
        private SerializedProperty scene;
        private SerializedProperty sceneName;
        private SerializedProperty sceneIndex;
        private SerializedProperty sceneEnabled;
        private SceneAsset sceneAsset;
        private string sceneAssetPath;
        private string sceneAssetGUID;

        private GUIContent errorTooltip;
        private GUIStyle errorStyle;
        #endregion -- Private Variables ---------------------------------------

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            this.CacheProperties(property);
            this.UpdateSceneState();

            position = this.DisplayErrorsIfNecessary(position);

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, this.scene, GUIContent.none, false);
            if (EditorGUI.EndChangeCheck()) {
                property.serializedObject.ApplyModifiedProperties();
                this.CacheProperties(property);
                this.UpdateSceneState();
                this.Validate();
            }

            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Cache all used properties as local variables so that they can be
        /// used by other methods. This needs to be called every frame since a
        /// PropertyDrawer can be reused on different properties.
        /// </summary>
        /// <param name="property">Property to search through.</param>
        private void CacheProperties(SerializedProperty property) {
            this.scene = property.FindPropertyRelative("Scene");
            this.sceneName = property.FindPropertyRelative("SceneName");
            this.sceneIndex = property.FindPropertyRelative("sceneIndex");
            this.sceneEnabled = property.FindPropertyRelative("sceneEnabled");
            this.sceneAsset = this.scene.objectReferenceValue as SceneAsset;

            if (this.sceneAsset != null) {
                this.sceneAssetPath = AssetDatabase.GetAssetPath(this.scene.objectReferenceValue);
                this.sceneAssetGUID = AssetDatabase.AssetPathToGUID(this.sceneAssetPath);
            } else {
                this.sceneAssetPath = null;
                this.sceneAssetGUID = null;
            }
        }

        /// <summary>
        /// Updates the scene index and enabled flags of a scene property by
        /// scanning through the scenes in EditorBuildSettings.
        /// </summary>
        private void UpdateSceneState() {
            if (this.sceneAsset != null) {
                var sceneInBuild = SceneReferenceUtils.GetSceneInBuildState(this.sceneAssetGUID);

                this.sceneIndex.intValue = sceneInBuild.Index;
                if (sceneInBuild.Present) {
                    this.sceneEnabled.boolValue = sceneInBuild.Enabled;
                    if (sceneInBuild.Enabled && this.sceneName.stringValue != this.sceneAsset.name)
                        this.sceneName.stringValue = this.sceneAsset.name;
                }
            } else {
                this.sceneName.stringValue = "";
            }
        }

        /// <summary>
        /// Display a popup error message about the selected scene and respond
        /// to the user choice by either fixing the issue in the build
        /// settings, doing nothing, or opening the build settings.
        /// </summary>
        /// <param name="message">Message to display.</param>
        private void DisplaySceneErrorPrompt(string message) {
            int choice = EditorUtility.DisplayDialogComplex("Scene Not In Build", message, "Yes", "No", "Open Build Settings");

            if (choice == 0) {
                var scenes = EditorBuildSettings.scenes;
                int newCount = this.sceneIndex.intValue < 0
                    ? scenes.Length + 1
                    : scenes.Length;
                EditorBuildSettingsScene[] newScenes = new EditorBuildSettingsScene[newCount];
                Array.Copy(scenes, newScenes, scenes.Length);

                if (this.sceneIndex.intValue < 0) {
                    newScenes[scenes.Length] = new EditorBuildSettingsScene(this.sceneAssetPath, true);
                    this.sceneIndex.intValue = scenes.Length;
                }

                newScenes[this.sceneIndex.intValue].enabled = true;

                EditorBuildSettings.scenes = newScenes;
            } else if (choice == 2) {
                EditorApplication.ExecuteMenuItem("File/Build Settings...");
            }
        }

        /// <summary>
        /// If there is anything wrong with the selected scene, this will
        /// display an error icon that the user can click on for more info.
        /// </summary>
        /// <param name="position">
        /// Full rect that will be used to draw the property.
        /// </param>
        /// <returns>
        /// The rect that should be used to draw the rest of the property. If
        /// there are no errors, this is the same as the input position Rect.
        /// Otherwise, it will be the input rect adjusted to fit the error.
        /// </returns>
        private Rect DisplayErrorsIfNecessary(Rect position) {
            if (this.errorStyle == null) {
                this.errorStyle = "CN EntryErrorIconSmall";
                this.errorTooltip = new GUIContent("", "error");
            }

            if (this.sceneAsset == null)
                return position;

            Rect warningRect = new Rect(position);
            warningRect.width = this.errorStyle.fixedWidth + 4;

            if (this.sceneIndex.intValue < 0) {
                this.errorTooltip.tooltip = TOOLTIP_SCENE_MISSING;
                position.xMin = warningRect.xMax;
                if (GUI.Button(warningRect, this.errorTooltip, this.errorStyle)) {
                    this.DisplaySceneErrorPrompt(ERROR_SCENE_MISSING);
                }
            } else if (!this.sceneEnabled.boolValue) {
                this.errorTooltip.tooltip = TOOLTIP_SCENE_DISABLED;
                position.xMin = warningRect.xMax;
                if (GUI.Button(warningRect, this.errorTooltip, this.errorStyle)) {
                    this.DisplaySceneErrorPrompt(ERROR_SCENE_DISABLED);
                }
            }

            return position;
        }

        /// <summary>
        /// Validate any new values in the scene property. This will display
        /// popup errors if there are issues with the current value.
        /// </summary>
        private void Validate() {
            if (this.sceneAsset != null) {
                var buildScene = SceneReferenceUtils.GetSceneInBuildState(this.sceneAssetGUID);

                this.sceneIndex.intValue = buildScene.Index;
                if (buildScene.Enabled) {
                    if (this.sceneName.stringValue != this.sceneAsset.name)
                        this.sceneName.stringValue = this.sceneAsset.name;
                } else
                    this.DisplaySceneErrorPrompt(
                        !buildScene.Present
                            ? ERROR_SCENE_MISSING
                            : ERROR_SCENE_DISABLED
                    );
            } else {
                this.sceneName.stringValue = "";
            }
        }
    }
}
#endif
