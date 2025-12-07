# Unity-Native Animation Pipeline Guide

## Overview

This guide documents the Unity-native animation asset pipeline as an alternative/complement to the Blender-centric workflow. The Unity-native approach uses professional Asset Store tools to create, edit, and manage character animations directly within Unity, minimizing external tool dependencies and streamlining the animation workflow for agents and developers.

## Table of Contents

- [Philosophy](#philosophy)
- [Recommended Tools](#recommended-tools)
- [Workflow Comparison](#workflow-comparison)
- [Step-by-Step Setup](#step-by-step-setup)
- [Test Asset Import](#test-asset-import)
- [UITransitionManager Compatibility](#uitransitionmanager-compatibility)
- [Best Practices](#best-practices)
- [Troubleshooting](#troubleshooting)

---

## Philosophy

### Why Unity-Native Animation?

**Advantages:**
- ✅ **Reduced Context Switching**: Animate directly in Unity Editor without jumping to Blender
- ✅ **Real-time Preview**: See animations in the actual game environment immediately
- ✅ **Agent-Friendly**: Simpler workflow with fewer tool dependencies for AI agents
- ✅ **Faster Iteration**: Direct integration with Unity's Animator and Timeline systems
- ✅ **Code-Driven Control**: Better programmatic animation control with tools like Animancer
- ✅ **Asset Store Integration**: Leverage pre-made animations from Mixamo, Unity Asset Store

**When to Use:**
- Quick prototyping and iteration on character animations
- Creating UI-driven character interactions (compatible with UITransitionManager.cs)
- Agents working on animation logic without Blender expertise
- Projects requiring rapid animation retargeting and modification
- Educational projects where simplicity is valued over custom character design

**When to Keep Blender:**
- Custom 3D character modeling from photos (AI generation)
- Complex rigging requirements beyond Unity's capabilities
- High-fidelity facial animation and blend shapes
- Custom mesh deformations and sculpting
- Asset pipeline automation with Python scripts

**Recommended Hybrid Approach:**
- Use **Blender** for: Character modeling, rigging, and base animations export
- Use **Unity-native tools** for: Animation editing, retargeting, and runtime control

---

## Recommended Tools

### 1. UMotion Pro ($90 USD)

**Purpose:** Professional animation editor for Unity

**Key Features:**
- Animate any 3D model directly in Unity Editor
- Advanced Inverse Kinematics (IK) with pinning and FK/IK blending
- Animation layers (additive/override) for non-destructive editing
- Child-Of constraint for dynamic parenting (pickup/drop objects)
- Import and retarget mocap animations (Mixamo, Asset Store)
- FBX export capability for external tools
- No runtime overhead (produces standard Unity .anim files)
- Animation project system for version control
- Full undo/redo support

**Best For:**
- Professional animation editing
- Complex character animation workflows
- Retargeting animations between different character rigs
- Creating animations from scratch in Unity

**Asset Store:** [UMotion Pro](https://assetstore.unity.com/packages/tools/animation/umotion-pro-animation-editor-95991)

**Alternative:** UMotion Community (free, limited features)

### 2. Animancer ($65 USD)

**Purpose:** Code-based animation system (Mecanim replacement)

**Key Features:**
- Control animations directly from C# code (no Animator Controllers needed)
- Play any animation clip at any time programmatically
- Smooth blending between animations
- Animation events and callbacks in code
- Better debugging (logic in scripts, not assets)
- Slightly better performance than Mecanim
- Perfect for procedural animation control
- Ideal for complex animation state management

**Best For:**
- Programmers who prefer code-driven animation control
- Projects with complex animation logic
- Integration with existing CharacterController.cs
- Educational games with dynamic animation systems

**Asset Store:** [Animancer Pro](https://assetstore.unity.com/packages/tools/animation/animancer-pro-116514)

**Why Animancer vs Mecanim?**
- **Animancer:** Logic in C# scripts, easier debugging, more flexible
- **Mecanim:** Visual state machines, better for artist-driven workflows

### 3. Final IK ($90 USD)

**Purpose:** Advanced Inverse Kinematics system

**Key Features:**
- Full Body IK for bipeds
- VRIK (optimized for VR avatars)
- Grounder for automatic foot placement
- Interaction System for character-object interactions
- Various IK solvers: FABRIK, CCD, Aim IK, LookAt IK, Limb IK
- Baker tool to bake procedural IK into animation clips
- Performance optimized for real-time use

**Best For:**
- Realistic character interactions with environment
- Foot placement on uneven terrain
- Character aiming and looking systems
- Object grabbing and manipulation
- VR character systems

**Asset Store:** [Final IK](https://assetstore.unity.com/packages/tools/animation/final-ik-14290)

**Alternative:** Unity's Animation Rigging Package (free, but less features)

### 4. Free Alternatives

**Unity Animation Rigging Package (FREE):**
- Built into Unity (install via Package Manager)
- Basic IK constraints (Two Bone IK, Multi-Parent, etc.)
- Good for simple IK setups
- Lighter weight than Final IK
- Official Unity support

**Unity Animator (FREE - Built-in):**
- Mecanim state machine system
- Blend trees for animation blending
- Animation layers and masking
- Retargeting for humanoid rigs
- Adequate for most basic needs

---

## Workflow Comparison

### Traditional Blender Workflow

```
1. Create character in Blender (modeling + rigging)
2. Animate in Blender (keyframe animation)
3. Export as FBX to Unity
4. Import FBX into Unity
5. Set up Animator Controller in Unity
6. Test in Play mode
7. If changes needed → back to Blender (steps 2-6 repeat)
```

**Time per iteration:** ~10-30 minutes (Blender ↔ Unity context switching)

### Unity-Native Workflow

```
1. Import character model (from Blender, Asset Store, or Mixamo)
2. Set rig type to Humanoid in Unity
3. Open UMotion Pro and create animation project
4. Animate directly in Unity with real-time preview
5. Save as Unity .anim clip
6. Drag into Animator Controller or use with Animancer
7. Test in Play mode immediately
8. If changes needed → edit in UMotion (no export/import)
```

**Time per iteration:** ~2-5 minutes (everything in Unity Editor)

### Hybrid Workflow (RECOMMENDED)

```
1. Create character model in Blender (one-time setup)
2. Export base rig to Unity as FBX
3. Import starter animations from Mixamo or Asset Store
4. Use UMotion Pro to customize/modify animations in Unity
5. Use Animancer for code-driven animation control
6. Use Final IK for runtime IK (foot placement, looking, etc.)
7. Iterate entirely within Unity
```

**Benefits:** Best of both worlds - custom characters + fast iteration

---

## Step-by-Step Setup

### Phase 1: Install Required Tools

#### Option A: Full Professional Setup ($245 USD)

```bash
# Install via Unity Asset Store:
1. UMotion Pro ($90)
2. Animancer Pro ($65)
3. Final IK ($90)

# Total: $245 USD (one-time purchase)
```

#### Option B: Budget-Friendly Setup (FREE)

```bash
# Install via Unity Package Manager:
1. Animation Rigging (FREE - official Unity package)
2. Use Unity's built-in Animator/Mecanim

# Note: More limited features, but functional
```

#### Option C: Recommended Starter ($90 USD)

```bash
# Start with the most impactful tool:
1. UMotion Pro ($90)
   - Covers 80% of animation needs
   - Add Animancer/Final IK later if needed
```

### Phase 2: Import Test Character

#### Method 1: Use Existing Leandi Character

```csharp
// Assets/Characters/Leandi/ structure:
Leandi/
├── Photos/          # Reference photos (already exist)
├── Models/          # Blender exports (FBX files)
│   └── Leandi_Rig.fbx
└── Animations/      # Unity animation clips (.anim)
    ├── Leandi_Idle.anim
    ├── Leandi_Dance.anim
    ├── Leandi_Wave.anim
    ├── Leandi_Wai.anim
    ├── Leandi_Curtsy.anim
    └── Leandi_Bow.anim
```

**Steps:**
1. Export Leandi character from Blender (if not already done)
2. Place FBX in `Assets/Characters/Leandi/Models/`
3. In Unity Inspector, set Rig to "Humanoid"
4. Click "Configure" to verify bone mapping
5. Click "Apply"

#### Method 2: Import Mixamo Character (Quick Test)

**For rapid prototyping without Blender:**

1. Go to [Mixamo.com](https://www.mixamo.com/)
2. Select a character (e.g., "Megan" or "Amy")
3. Download as FBX for Unity:
   - Format: FBX Binary
   - Pose: T-Pose
   - Include animations: No (we'll get these separately)
4. Place in `Assets/Characters/TestCharacter/Models/`
5. In Unity Inspector:
   - Rig Type: Humanoid
   - Animation Type: Generic (for model file)
6. Download animations separately from Mixamo:
   - Idle, Dance, Wave, etc.
   - Format: FBX, 30fps, "In Place" option checked
7. Place animations in `Assets/Characters/TestCharacter/Animations/`

### Phase 3: Set Up UMotion Pro

**Create Animation Project:**

1. Window > UMotion Editor > Clip Editor
2. Click "Create New Project"
   - Name: "Leandi_Animations"
   - Location: `Assets/Characters/Leandi/`
3. Assign character model to project
4. Set pose to T-Pose or Bind Pose

**Create First Animation:**

1. In Clip Editor, click "Create Clip"
   - Name: "Idle"
   - Duration: 2 seconds (60 frames at 30fps)
2. Select bones in hierarchy
3. Use Pose Editor to adjust positions
4. Add keyframes (K key)
5. Scrub timeline to preview
6. Export as Unity .anim file

**Import Existing Animation for Editing:**

1. Drag existing .anim or FBX animation to Clip Editor
2. UMotion imports keyframe data
3. Edit as needed
4. Export back to .anim

### Phase 4: Set Up Animancer (Optional but Recommended)

**Create AnimancerComponent:**

1. Add `AnimancerComponent` to character GameObject
2. Remove standard `Animator` component (Animancer replaces it)
3. Assign `Animator` (for rig structure)

**Example C# Integration with CharacterController:**

```csharp
using UnityEngine;
using Animancer;

namespace SangsomMiniMe.Character
{
    public class CharacterController : MonoBehaviour
    {
        [Header("Animancer Setup")]
        [SerializeField] private AnimancerComponent animancer;
        
        [Header("Animation Clips")]
        [SerializeField] private AnimationClip idleClip;
        [SerializeField] private AnimationClip danceClip;
        [SerializeField] private AnimationClip waveClip;
        [SerializeField] private AnimationClip waiClip;
        [SerializeField] private AnimationClip curtsyClip;
        [SerializeField] private AnimationClip bowClip;

        private void Start()
        {
            // Play default idle animation
            PlayIdle();
        }

        public void PlayIdle()
        {
            animancer.Play(idleClip);
        }

        public void PlayDance()
        {
            animancer.Play(danceClip);
        }

        public void PlayWave()
        {
            var state = animancer.Play(waveClip);
            state.Events.OnEnd = PlayIdle; // Return to idle when done
        }

        public void PlayWai()
        {
            var state = animancer.Play(waiClip);
            state.Events.OnEnd = PlayIdle;
        }

        public void PlayCurtsy()
        {
            var state = animancer.Play(curtsyClip);
            state.Events.OnEnd = PlayIdle;
        }

        public void PlayBow()
        {
            var state = animancer.Play(bowClip);
            state.Events.OnEnd = PlayIdle;
        }

        // Cross-fade for smooth transitions
        public void PlayDanceSmooth(float fadeDuration = 0.3f)
        {
            animancer.Play(danceClip, fadeDuration);
        }
    }
}
```

### Phase 5: Set Up Final IK (Optional)

**Add Grounder for Foot Placement:**

1. Add `GrounderBipedIK` component to character
2. Assign Spine, Left/Right Thigh, Foot, and Toe bones
3. Adjust "Foot Length" and "Height" parameters
4. Character feet now adapt to terrain automatically

**Add LookAt IK:**

1. Add `LookAtIK` component
2. Create empty GameObject as look target
3. Assign head, spine, and eye bones
4. Character now looks at target smoothly

---

## Test Asset Import

### Quick Validation Test

**Goal:** Verify Unity-native pipeline works end-to-end

#### Test 1: Import and Retarget Mixamo Animation

```
1. Download Mixamo character "Megan" with T-Pose
2. Download "Idle" animation for Megan
3. Import both into Unity (Assets/Tests/MixamoTest/)
4. Set Megan rig to Humanoid
5. Verify Unity recognizes all bones (green checkmarks)
6. Create Animator Controller
7. Add Idle animation to controller
8. Drop character into scene and press Play
9. ✅ Character should animate smoothly

Result: Proves Unity can import and play animations natively
```

#### Test 2: Edit Animation with UMotion Pro

```
1. Open UMotion Clip Editor
2. Create new project: "TestProject"
3. Assign Megan character
4. Create new clip: "CustomWave"
5. Pose right arm raised (frame 0)
6. Pose right arm lowered (frame 30)
7. Scrub timeline → should see smooth wave
8. Export as "CustomWave.anim"
9. Add to Animator Controller
10. ✅ Play in scene → custom animation works

Result: Proves we can create/edit animations in Unity
```

#### Test 3: Code-Driven Animation with Animancer

```csharp
// Add to test character:
using Animancer;
using UnityEngine;

public class AnimancerTest : MonoBehaviour
{
    [SerializeField] private AnimancerComponent animancer;
    [SerializeField] private AnimationClip clip1;
    [SerializeField] private AnimationClip clip2;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            animancer.Play(clip1);
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
            animancer.Play(clip2);
    }
}

// Press Play, hit 1 and 2 keys
// ✅ Animations switch smoothly via code
```

#### Test 4: Runtime IK with Final IK

```
1. Add GrounderBipedIK to character
2. Create sloped terrain in scene
3. Walk character across terrain
4. ✅ Feet should plant correctly on slopes

Result: Proves runtime IK works for dynamic interactions
```

### Validation Checklist

```
✅ Can import FBX characters from Blender
✅ Can import animations from Mixamo/Asset Store
✅ Can edit animations with UMotion Pro
✅ Can create new animations from scratch
✅ Can control animations via C# code (Animancer)
✅ Can use runtime IK for foot placement (Final IK)
✅ Animations work with existing CharacterController.cs
✅ No runtime performance issues (60fps maintained)
✅ UITransitionManager.cs compatible with animation transitions
```

---

## UITransitionManager Compatibility

### Integration Strategy

The existing `UITransitionManager.cs` handles UI panel transitions. Character animations should run independently but can be synchronized:

#### Approach 1: Parallel Transitions (RECOMMENDED)

```csharp
// In GameUI.cs or similar
public void ShowCharacterCustomizationPanel()
{
    // Transition UI panels
    UITransitionManager.Instance.TransitionPanels(
        mainPanel, 
        customizationPanel, 
        UITransitionManager.TransitionType.SlideLeft
    );
    
    // Play character animation simultaneously
    characterController.PlayWave(); // Greet the customization screen
}
```

#### Approach 2: Sequenced Transitions

```csharp
public void ShowRewardPanel()
{
    // Play character animation first
    characterController.PlayDance();
    
    // Wait for animation, then show panel
    StartCoroutine(ShowPanelAfterAnimation(1.5f));
}

private IEnumerator ShowPanelAfterAnimation(float delay)
{
    yield return new WaitForSeconds(delay);
    
    UITransitionManager.Instance.ShowPanel(
        rewardPanel,
        UITransitionManager.TransitionType.ScaleUp
    );
}
```

#### Approach 3: Event-Driven Synchronization

```csharp
// Add to CharacterController.cs
public event System.Action OnAnimationComplete;

public void PlayDance()
{
    var state = animancer.Play(danceClip);
    state.Events.OnEnd = () => OnAnimationComplete?.Invoke();
}

// In UI code:
private void Start()
{
    characterController.OnAnimationComplete += ShowRewardPanel;
}

private void ShowRewardPanel()
{
    UITransitionManager.Instance.ShowPanel(
        rewardPanel,
        UITransitionManager.TransitionType.FadeAndSlide
    );
}
```

### Performance Considerations

**UITransitionManager runs coroutines for smooth UI fades/slides.**
**Character animations run via Unity's Animator/Animancer.**

**Both systems are independent and won't conflict because:**
- UI transitions affect `CanvasGroup.alpha` and `RectTransform.position`
- Character animations affect `Transform` hierarchy of character bones
- No shared state or resources

**Optimization tips:**
- Use UI transitions for UI GameObjects only
- Use character animations for 3D character GameObjects only
- Avoid triggering 10+ transitions simultaneously
- Profile with Unity Profiler if frame drops occur

---

## Best Practices

### General Workflow

1. **Use Humanoid Rig Type** for all characters
   - Enables animation retargeting between characters
   - Required for UMotion Pro and Final IK
   - Unity's avatar system handles differences automatically

2. **Organize Animation Clips by Character**
   ```
   Assets/Characters/
   ├── Leandi/
   │   └── Animations/
   │       ├── Leandi_Idle.anim
   │       ├── Leandi_Dance.anim
   │       └── ...
   └── Student_Template/
       └── Animations/
           ├── Student_Idle.anim
           └── ...
   ```

3. **Name Animations Consistently**
   - `CharacterName_ActionName.anim`
   - e.g., `Leandi_Wave.anim`, `Leandi_Bow.anim`
   - Makes scripting easier (find by pattern)

4. **Use Animation Events for Synchronization**
   - Add events to animation clips for footsteps, VFX triggers
   - Better than hardcoding delays in scripts

5. **Version Control Best Practices**
   - UMotion projects (.asset) are version-control friendly
   - .anim files are YAML text (easy to diff)
   - Commit frequently during animation work

### UMotion Pro Tips

- **Use Animation Layers** for non-destructive editing
  - Base layer: imported animation
  - Override layer: your modifications
- **Pin IK targets** when editing (prevents unwanted movement)
- **Animate in Play Mode** to see exact runtime behavior
- **Use Muscle Groups** for finger/facial animation
- **Export to FBX** if you need animations in other tools

### Animancer Tips

- **Cache animation references** in serialized fields
- **Use events** instead of checking animation time in Update()
- **Blend animations** with fade durations for smoothness
- **Avoid playing the same clip repeatedly** (check if already playing)
- **Use layers** for simultaneous animations (e.g., walk + wave)

### Final IK Tips

- **Start with Grounder** for instant foot placement wins
- **Adjust weight values** for subtle vs dramatic IK influence
- **Use LookAt IK** for engaging character eye contact
- **Bake IK to animations** if performance is critical
- **Profile regularly** (IK can be expensive on low-end devices)

---

## Troubleshooting

### Import Issues

**Problem:** FBX imports but animations are broken

**Solutions:**
- Check rig is set to Humanoid (not Generic)
- Verify bone mapping in Avatar Configuration
- Ensure T-pose or bind pose is correct
- Re-import with "Import Animation" checked

---

**Problem:** Mixamo animations slide/float

**Solutions:**
- Download with "In Place" option enabled
- Adjust root motion settings in animation import
- Add Grounder component for automatic foot correction

---

**Problem:** UMotion Pro doesn't recognize bones

**Solutions:**
- Ensure character has Humanoid rig type
- Verify Avatar is properly configured (green checkmarks)
- Check bone hierarchy matches Unity's humanoid template

---

### Animation Issues

**Problem:** Animations don't loop smoothly

**Solutions:**
- In animation import settings, enable "Loop Time"
- Match start and end poses in UMotion Pro
- Use animation curves to ease in/out

---

**Problem:** Transition between animations is jerky

**Solutions:**
- Use Animancer's `Play(clip, fadeDuration)` for cross-fading
- In Mecanim, adjust transition duration and offset
- Ensure "Interruption Source" is set appropriately

---

**Problem:** IK makes limbs stretch unrealistically

**Solutions:**
- Check "limb length" settings in Final IK
- Add pole targets for elbows/knees
- Reduce IK weight (don't use 100% always)
- Ensure end-effector targets aren't too far from character

---

### Performance Issues

**Problem:** Frame rate drops with animations

**Solutions:**
- Reduce IK solver updates (e.g., update every 2nd frame)
- Bake IK to animations for background characters
- Use Animator culling (disable when off-screen)
- Simplify character rigs (fewer bones)
- Profile with Unity Profiler to identify bottleneck

---

**Problem:** Too many animation clips increase build size

**Solutions:**
- Use animation compression in import settings
- Share animations between similar characters
- Remove unused animation clips before build
- Use addressables to load animations on-demand

---

## Conclusion

The Unity-native animation pipeline provides a streamlined, agent-friendly workflow for character animation with minimal external tool dependencies. By leveraging professional Asset Store tools (UMotion Pro, Animancer, Final IK), teams can achieve high-quality character animation entirely within Unity.

**Key Takeaways:**
- ✅ Faster iteration (no Blender round-trips)
- ✅ Better code integration (Animancer)
- ✅ Runtime IK for dynamic interactions (Final IK)
- ✅ Compatible with existing systems (UITransitionManager, CharacterController)
- ✅ Scalable for production (proven tools, good performance)

**Recommended Next Steps:**
1. Purchase UMotion Pro ($90) as minimum viable setup
2. Import Mixamo character for quick validation
3. Create test animations following this guide
4. Integrate with existing CharacterController.cs
5. Profile performance and iterate
6. Consider adding Animancer/Final IK as needed

For questions or issues, refer to the official documentation for each tool or consult the Unity community forums.

---

**Last Updated:** December 2024  
**Unity Version:** 2022.3.12f1 LTS  
**Author:** Sangsom Mini-Me Development Team
