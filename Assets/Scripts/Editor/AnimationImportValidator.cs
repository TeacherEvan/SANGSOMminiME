using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace SangsomMiniMe.Tests
{
    /// <summary>
    /// Editor utility to test and validate Unity-native animation asset imports.
    /// Tests FBX import settings, rig configuration, and animation clip generation.
    /// </summary>
    public class AnimationImportValidator : EditorWindow
    {
        private GameObject testModel;
        private string validationResults = "";
        private Vector2 scrollPosition;
        
        [MenuItem("Sangsom Mini-Me/Animation/Validate Import Settings")]
        public static void ShowWindow()
        {
            var window = GetWindow<AnimationImportValidator>("Animation Import Validator");
            window.minSize = new Vector2(400, 500);
            window.Show();
        }
        
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Unity-Native Animation Import Validator", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "This tool validates that FBX models and animations are imported correctly " +
                "for use with UMotion Pro, Animancer, and Final IK.",
                MessageType.Info
            );
            
            EditorGUILayout.Space();
            
            testModel = (GameObject)EditorGUILayout.ObjectField(
                "Test Model/Animation", 
                testModel, 
                typeof(GameObject), 
                false
            );
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Validate Import Settings", GUILayout.Height(30)))
            {
                ValidateModel();
            }
            
            EditorGUILayout.Space();
            
            if (!string.IsNullOrEmpty(validationResults))
            {
                EditorGUILayout.LabelField("Validation Results:", EditorStyles.boldLabel);
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));
                EditorGUILayout.TextArea(validationResults, GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();
            }
        }
        
        private void ValidateModel()
        {
            if (testModel == null)
            {
                validationResults = "ERROR: No model selected. Please assign a model to test.";
                return;
            }
            
            var results = new System.Text.StringBuilder();
            results.AppendLine("=== ANIMATION IMPORT VALIDATION ===");
            results.AppendLine($"Model: {testModel.name}");
            results.AppendLine($"Time: {System.DateTime.Now}");
            results.AppendLine();
            
            string assetPath = AssetDatabase.GetAssetPath(testModel);
            
            if (string.IsNullOrEmpty(assetPath))
            {
                results.AppendLine("ERROR: Cannot determine asset path.");
                validationResults = results.ToString();
                return;
            }
            
            results.AppendLine($"Asset Path: {assetPath}");
            results.AppendLine();
            
            // Test 1: Check if it's an FBX or model file
            bool isFBX = assetPath.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase);
            results.AppendLine($"[Test 1] FBX File: {(isFBX ? "✓ PASS" : "✗ FAIL - Not an FBX file")}");
            
            if (!isFBX)
            {
                results.AppendLine("  INFO: Animation import validation works best with FBX files.");
            }
            
            // Test 2: Check Model Import Settings
            ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            
            if (importer == null)
            {
                results.AppendLine("[Test 2] Model Importer: ✗ FAIL - Not a model file");
                validationResults = results.ToString();
                return;
            }
            
            results.AppendLine("[Test 2] Model Importer: ✓ FOUND");
            results.AppendLine();
            
            // Test 3: Rig Configuration
            results.AppendLine("=== RIG CONFIGURATION ===");
            results.AppendLine($"Animation Type: {importer.animationType}");
            
            bool isHumanoid = importer.animationType == ModelImporterAnimationType.Human;
            bool isGeneric = importer.animationType == ModelImporterAnimationType.Generic;
            
            if (isHumanoid)
            {
                results.AppendLine("[Test 3] Rig Type: ✓ PASS - Humanoid (Recommended)");
                results.AppendLine("  INFO: Humanoid rigs support animation retargeting.");
            }
            else if (isGeneric)
            {
                results.AppendLine("[Test 3] Rig Type: ⚠ WARN - Generic (Limited retargeting)");
                results.AppendLine("  SUGGESTION: Change to Humanoid if this is a bipedal character.");
            }
            else
            {
                results.AppendLine("[Test 3] Rig Type: ✗ FAIL - None or Legacy");
                results.AppendLine("  FIX: Select asset > Inspector > Rig > Animation Type: Humanoid");
            }
            
            // Test 4: Avatar Configuration (for Humanoid)
            if (isHumanoid)
            {
                results.AppendLine();
                results.AppendLine("=== AVATAR CONFIGURATION ===");
                
                var avatar = importer.sourceAvatar;
                if (avatar != null && avatar.isHuman)
                {
                    results.AppendLine("[Test 4] Avatar: ✓ PASS - Valid humanoid avatar");
                    
                    if (avatar.isValid)
                    {
                        results.AppendLine("  Bone Mapping: ✓ Valid");
                    }
                    else
                    {
                        results.AppendLine("  Bone Mapping: ✗ Invalid - Reconfigure in Inspector");
                    }
                }
                else
                {
                    results.AppendLine("[Test 4] Avatar: ✗ FAIL - No valid avatar");
                    results.AppendLine("  FIX: Select asset > Inspector > Rig > Configure > Fix bone mapping");
                }
            }
            
            // Test 5: Animation Import Settings
            results.AppendLine();
            results.AppendLine("=== ANIMATION IMPORT ===");
            results.AppendLine($"Import Animation: {(importer.importAnimation ? "✓ ENABLED" : "✗ DISABLED")}");
            
            if (!importer.importAnimation)
            {
                results.AppendLine("  FIX: Enable 'Import Animation' in Inspector > Animation tab");
            }
            
            results.AppendLine($"Animation Compression: {importer.animationCompression}");
            results.AppendLine($"Resample Curves: {(importer.resampleCurves ? "Enabled" : "Disabled")}");
            
            // Test 6: Animation Clips
            results.AppendLine();
            results.AppendLine("=== ANIMATION CLIPS ===");
            
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            List<AnimationClip> clips = new List<AnimationClip>();
            
            foreach (var asset in assets)
            {
                if (asset is AnimationClip clip && !clip.name.StartsWith("__"))
                {
                    clips.Add(clip);
                }
            }
            
            if (clips.Count > 0)
            {
                results.AppendLine($"[Test 6] Animation Clips: ✓ FOUND ({clips.Count} clips)");
                results.AppendLine();
                
                foreach (var clip in clips)
                {
                    results.AppendLine($"  Clip: {clip.name}");
                    results.AppendLine($"    Length: {clip.length:F2} seconds");
                    results.AppendLine($"    Frame Rate: {clip.frameRate} fps");
                    results.AppendLine($"    Looping: {(clip.isLooping ? "Yes" : "No")}");
                    results.AppendLine($"    Legacy: {(clip.legacy ? "Yes (old format)" : "No")}");
                    
                    if (clip.legacy)
                    {
                        results.AppendLine("      ⚠ WARNING: Legacy format not compatible with Mecanim/Animancer");
                    }
                    
                    results.AppendLine();
                }
            }
            else
            {
                results.AppendLine("[Test 6] Animation Clips: ⚠ WARN - No clips found");
                results.AppendLine("  INFO: This might be a model-only file (no animations).");
            }
            
            // Test 7: Compatibility Checks
            results.AppendLine("=== COMPATIBILITY ===");
            
            bool umotionCompatible = isHumanoid || isGeneric;
            results.AppendLine($"[Test 7a] UMotion Pro: {(umotionCompatible ? "✓ Compatible" : "✗ Incompatible")}");
            
            bool animancerCompatible = clips.Count > 0;
            results.AppendLine($"[Test 7b] Animancer: {(animancerCompatible ? "✓ Compatible" : "⚠ No clips to play")}");
            
            bool finalIKCompatible = isHumanoid;
            results.AppendLine($"[Test 7c] Final IK: {(finalIKCompatible ? "✓ Compatible (Humanoid)" : "⚠ Limited (Non-humanoid)")}");
            
            // Test 8: File Organization
            results.AppendLine();
            results.AppendLine("=== FILE ORGANIZATION ===");
            
            bool inCharacterFolder = assetPath.Contains("Assets/Characters/");
            results.AppendLine($"[Test 8] Location: {(inCharacterFolder ? "✓ Organized (in Assets/Characters/)" : "⚠ Consider moving to Assets/Characters/")}");
            
            if (!inCharacterFolder)
            {
                results.AppendLine("  SUGGESTION: Organize as Assets/Characters/[CharacterName]/Models/ or /Animations/");
            }
            
            // Summary
            results.AppendLine();
            results.AppendLine("=== VALIDATION SUMMARY ===");
            
            int passCount = 0;
            int warnCount = 0;
            int failCount = 0;
            
            string fullResults = results.ToString();
            passCount = CountOccurrences(fullResults, "✓ PASS");
            warnCount = CountOccurrences(fullResults, "⚠ WARN");
            failCount = CountOccurrences(fullResults, "✗ FAIL");
            
            results.AppendLine($"Passed: {passCount}");
            results.AppendLine($"Warnings: {warnCount}");
            results.AppendLine($"Failed: {failCount}");
            results.AppendLine();
            
            if (failCount == 0 && warnCount == 0)
            {
                results.AppendLine("✓ ALL CHECKS PASSED - Model is ready for Unity-native animation workflow!");
            }
            else if (failCount == 0)
            {
                results.AppendLine("⚠ VALIDATION PASSED WITH WARNINGS - Review suggestions above.");
            }
            else
            {
                results.AppendLine("✗ VALIDATION FAILED - Fix issues above before using this model.");
            }
            
            validationResults = results.ToString();
            Debug.Log(validationResults);
        }
        
        private int CountOccurrences(string text, string substring)
        {
            int count = 0;
            int index = 0;
            while ((index = text.IndexOf(substring, index)) != -1)
            {
                count++;
                index += substring.Length;
            }
            return count;
        }
    }
}
