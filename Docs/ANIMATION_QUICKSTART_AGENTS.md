# Animation Pipeline Quickstart for Agents

## Goal
Enable AI agents to quickly implement character animations in the Sangsom Mini-Me project with minimal friction and maximum error resistance.

## Overview

This project supports **TWO animation workflows**:

### Workflow A: Blender-Based (Original)
- **When:** Custom character modeling, complex rigging, facial animation
- **Tools:** Blender 5.0.0 + Python scripting
- **Export:** FBX → Unity
- **Best For:** Initial character creation, unique assets

### Workflow B: Unity-Native (NEW)
- **When:** Animation editing, retargeting, rapid iteration
- **Tools:** UMotion Pro, Animancer, Final IK (Unity Asset Store)
- **Export:** Native .anim files (no export needed)
- **Best For:** Animation work, agent tasks, quick prototyping

**Most agent tasks should use Workflow B (Unity-Native).**

---

## Quick Decision Tree

```
Do you need to MODEL a new character?
├─ YES → Use Blender (Workflow A)
└─ NO → Is it animation work?
    └─ YES → Use Unity-Native tools (Workflow B) ✅
```

---

## 5-Minute Agent Setup

### Prerequisites Check

```bash
# Verify Unity is installed
ls /path/to/Unity/Hub/Editor/2022.3.12f1/

# Clone project (if not done)
git clone https://github.com/TeacherEvan/SANGSOMminiME.git
cd SANGSOMminiME

# Open in Unity Hub
# File > Open Project > Select SANGSOMminiME folder
```

### Install Unity Tools (One-Time)

**Option 1: Asset Store (Requires Unity Account)**

```
1. Open Unity Editor
2. Window > Asset Store (or visit assetstore.unity.com)
3. Search and purchase:
   - "UMotion Pro" ($90) - ESSENTIAL for animation editing
   - "Animancer Pro" ($65) - OPTIONAL for code control
   - "Final IK" ($90) - OPTIONAL for IK features
4. Download and import into project
```

**Option 2: Quick Test (Free)**

```
1. Window > Package Manager
2. Search "Animation Rigging"
3. Click Install
4. Use Unity's built-in Animator
```

---

## Common Agent Tasks

### Task 1: Add New Animation to Character

**Scenario:** Add "Jump" animation to Leandi character

**Steps:**

```csharp
// 1. Download animation from Mixamo (free)
//    - Go to mixamo.com
//    - Search "Jump"
//    - Download as FBX (30fps, In Place)

// 2. Import into Unity
//    - Drag FBX to Assets/Characters/Leandi/Animations/

// 3. Configure import settings
//    - Select FBX in Project window
//    - Inspector > Rig > Animation Type: "Humanoid"
//    - Inspector > Animation > Import Animation: ✅
//    - Click Apply

// 4. Add to CharacterController.cs
public class CharacterController : MonoBehaviour
{
    [SerializeField] private AnimationClip jumpClip;
    
    public void PlayJump()
    {
        // If using Animancer:
        animancer.Play(jumpClip);
        
        // If using Animator:
        animator.SetTrigger("Jump");
    }
}

// 5. Test in Unity
//    - Drag character to scene
//    - Press Play
//    - Call PlayJump() method (e.g., from UI button)
```

**Time:** ~10 minutes

---

### Task 2: Edit Existing Animation

**Scenario:** Make "Wave" animation more enthusiastic

**Steps:**

```
1. Install UMotion Pro (if not done)
2. Window > UMotion Editor > Clip Editor
3. Create or Open UMotion Project
   - Create New: Assets/Characters/Leandi/Leandi_Animations.asset
   - Assign Leandi character model
4. Import existing animation
   - Click "+" button in Clip Editor
   - Select "Leandi_Wave.anim"
   - UMotion loads keyframe data
5. Edit animation
   - Select right arm bone
   - Move to frame 15 (peak of wave)
   - In Pose Editor, rotate arm higher (+30 degrees)
   - Press K to set keyframe
6. Preview
   - Scrub timeline → see updated wave
7. Export
   - Click Export > Replace "Leandi_Wave.anim"
8. Test
   - Return to scene, press Play
   - Wave should be more enthusiastic
```

**Time:** ~15 minutes

---

### Task 3: Create Animation from Scratch

**Scenario:** Create custom "Thinking" idle animation

**Steps:**

```
1. Open UMotion Clip Editor
2. Create new clip: "Leandi_Thinking"
   - Duration: 4 seconds (120 frames @ 30fps)
3. Pose character at keyframes:
   Frame 0:   Normal idle pose
   Frame 30:  Right hand raised to chin
   Frame 90:  Hand on chin (hold)
   Frame 120: Return to idle
4. Add subtle head rotation:
   Frame 0:   Head straight
   Frame 60:  Head tilted 15° right
   Frame 120: Head straight
5. Preview → smooth interpolation
6. Export as "Leandi_Thinking.anim"
7. Add to CharacterController:
   [SerializeField] private AnimationClip thinkingClip;
   public void PlayThinking() { animancer.Play(thinkingClip); }
```

**Time:** ~30 minutes (first time), ~15 minutes (experienced)

---

### Task 4: Retarget Animation to Different Character

**Scenario:** Copy Leandi's animations to new student character

**Steps:**

```
1. Ensure both characters have Humanoid rig type
2. Verify Avatar configuration matches (T-pose, bone mapping)
3. Method A: Direct Copy (if rigs are similar)
   - Copy .anim files from Leandi/Animations/
   - Paste into Student/Animations/
   - Rename files (Student_Wave.anim, etc.)
   - Drag onto new character → should work
   
4. Method B: UMotion Retarget (for different proportions)
   - Open animation in UMotion
   - Change character reference to new student
   - UMotion auto-retargets to new rig
   - Export as new animation clip
```

**Time:** ~5 minutes per animation

---

### Task 5: Add Runtime IK (Foot Placement)

**Scenario:** Make character feet adapt to terrain

**Steps:**

```
1. Install Final IK (or Animation Rigging)
2. Select character in Hierarchy
3. Add Component > Final IK > Grounder > Grounder Biped IK
4. Assign bones in Inspector:
   - Spine: Spine bone
   - Left Thigh: LeftUpperLeg
   - Left Calf: LeftLowerLeg  
   - Left Foot: LeftFoot
   - Left Toe: LeftToes (if exists)
   - Repeat for right side
5. Adjust parameters:
   - Foot Length: ~0.2 (measure in scene)
   - Height: 0.1 (feet underground offset)
   - Weight: 1.0 (full effect)
6. Test:
   - Create sloped terrain in scene
   - Walk character across it
   - Feet should plant correctly
```

**Time:** ~20 minutes

---

## Integration with Existing Code

### CharacterController.cs

**Current structure:**
```csharp
namespace SangsomMiniMe.Character
{
    public class CharacterController : MonoBehaviour
    {
        // Existing fields...
        private Animator animator;
        
        // Existing methods...
        public void PlayDance() { /* ... */ }
        public void PlayWave() { /* ... */ }
    }
}
```

**Adding Animancer (OPTIONAL upgrade):**

```csharp
using Animancer; // Add this

namespace SangsomMiniMe.Character
{
    public class CharacterController : MonoBehaviour
    {
        // Replace Animator with AnimancerComponent
        [SerializeField] private AnimancerComponent animancer;
        
        // Animation clip references
        [SerializeField] private AnimationClip idleClip;
        [SerializeField] private AnimationClip danceClip;
        [SerializeField] private AnimationClip waveClip;
        // ... etc
        
        public void PlayDance()
        {
            var state = animancer.Play(danceClip);
            // Optional: Add callback when animation finishes
            state.Events.OnEnd = () => PlayIdle();
        }
        
        public void PlayWave()
        {
            // Cross-fade for smooth transition (0.3 second blend)
            animancer.Play(waveClip, fadeDuration: 0.3f);
        }
        
        private void PlayIdle()
        {
            animancer.Play(idleClip);
        }
    }
}
```

**Benefits:**
- ✅ No Animator Controller needed (less asset clutter)
- ✅ All logic in C# code (easier debugging)
- ✅ Events and callbacks built-in
- ✅ Dynamic blending with code control

---

### UITransitionManager.cs

**No changes needed!** UI transitions and character animations are independent.

**Safe to do:**
```csharp
// Play animation and show panel simultaneously
characterController.PlayDance();
UITransitionManager.Instance.ShowPanel(rewardPanel);
```

**Also safe to sequence:**
```csharp
// Animation first, then panel
characterController.PlayDance();
yield return new WaitForSeconds(1.5f);
UITransitionManager.Instance.ShowPanel(rewardPanel);
```

---

## Error Prevention

### Common Mistakes (and how to avoid)

#### ❌ Mistake 1: Importing with Generic Rig

```
Problem: Animation doesn't work on character
Solution: Always use "Humanoid" rig type for characters
          Check: Select FBX > Inspector > Rig > Animation Type: Humanoid
```

#### ❌ Mistake 2: Missing Avatar Configuration

```
Problem: UMotion can't find bones
Solution: Configure Avatar after import
          Steps:  Select FBX > Inspector > Rig > Configure
                  Verify all bones mapped (green checkmarks)
                  Click Done
```

#### ❌ Mistake 3: Animations Sliding/Floating

```
Problem: Character feet slide across ground
Solution: Download Mixamo with "In Place" option enabled
          OR: Adjust root motion in animation import settings
          OR: Use Final IK Grounder component
```

#### ❌ Mistake 4: Animation Doesn't Loop

```
Problem: Animation plays once and stops
Solution: Enable Loop Time in animation import settings
          Select animation clip > Inspector > Loop Time ✅
```

#### ❌ Mistake 5: Jerky Transitions

```
Problem: Animation switches look choppy
Solution: Use cross-fading:
          - Animancer: animancer.Play(clip, fadeDuration: 0.3f)
          - Animator: Increase transition duration in Animator Controller
```

---

## Testing Checklist

Before committing animation work, verify:

```
✅ Animation plays in Unity Play mode
✅ Animation loops smoothly (if intended to loop)
✅ Transitions between animations are smooth
✅ Character doesn't slide/float during animation
✅ Animation works with both Animator and Animancer (if using both)
✅ .anim files are in correct Assets/Characters/[Name]/Animations/ folder
✅ File names follow convention: CharacterName_ActionName.anim
✅ No errors in Unity Console when playing animation
✅ Frame rate stays at 60fps (check Profiler if concerned)
✅ Animation is appropriate for educational context (see .vscode/rules)
```

---

## Performance Guidelines

### Target Performance

- **Desktop:** 60fps with 10+ animated characters
- **Mobile:** 30fps with 5+ animated characters
- **WebGL:** 30-60fps with 3+ animated characters

### Optimization Tips

```csharp
// 1. Cull off-screen animators
animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;

// 2. Reduce IK update frequency
void Update()
{
    if (Time.frameCount % 2 == 0) // Every other frame
        finalIK.solver.Update();
}

// 3. Use simpler animations for background characters
// Create "LOD" versions with fewer keyframes

// 4. Disable unnecessary components
if (!isVisible)
{
    animator.enabled = false;
    finalIK.enabled = false;
}
```

---

## Resources

### Official Documentation

- **UMotion Pro:** [soxware.com/umotion-manual](https://www.soxware.com/umotion-manual/UMotionManual.html)
- **Animancer:** [kybernetik.com.au/animancer/docs](https://kybernetik.com.au/animancer/docs/)
- **Final IK:** [root-motion.com/finalikdox](http://root-motion.com/finalikdox/html/index.html)
- **Unity Animation:** [docs.unity3d.com/Manual/AnimationOverview](https://docs.unity3d.com/Manual/AnimationOverview.html)

### Free Animation Sources

- **Mixamo:** [mixamo.com](https://www.mixamo.com/) - Free humanoid animations
- **Unity Asset Store:** Search "free animations"
- **OpenGameArt:** [opengameart.org](https://opengameart.org/) - CC-licensed content

### Community Support

- **Unity Forums:** [forum.unity.com](https://forum.unity.com/)
- **UMotion Discord:** Check Asset Store page for invite
- **Animancer Discord:** Check Asset Store page for invite

---

## Agent Success Criteria

An agent successfully completes an animation task when:

✅ **Functionality:** Animation plays correctly in Unity Play mode
✅ **Quality:** Animation is smooth, loops properly, no visual glitches
✅ **Integration:** Works with existing CharacterController.cs without breaking code
✅ **Performance:** Maintains 60fps on desktop (profile if uncertain)
✅ **Documentation:** Added comment in code explaining what animation does
✅ **Testing:** Ran through testing checklist above
✅ **Cultural:** Animation is appropriate for Thai educational context (if cultural gesture)
✅ **Commit:** Changes committed with clear message (e.g., "Add enthusiastic wave animation to Leandi")

---

## Troubleshooting (Agent Edition)

### "I can't find UMotion Pro in Unity"

```
1. Window > UMotion Editor > Clip Editor
   - If menu doesn't exist, tool isn't installed
2. Install via Asset Store (see "5-Minute Setup" above)
3. Restart Unity Editor after installation
```

### "Animation is imported but doesn't play"

```
1. Check Inspector settings:
   - Rig Type: Humanoid ✅
   - Import Animation: ✅  
   - Loop Time: ✅ (if looping animation)
2. Check character has Animator or AnimancerComponent
3. Verify animation is assigned in Inspector fields
4. Check Unity Console for error messages
```

### "Character feet are floating/sliding"

```
Solution A: Re-download from Mixamo with "In Place" checked
Solution B: Install Final IK, add Grounder component
Solution C: Adjust Root Transform Position(Y) in animation import settings
```

### "Performance is slow (< 60fps)"

```
1. Open Window > Analysis > Profiler
2. Record while playing animation
3. Check CPU and Rendering categories
4. Common culprits:
   - Too many IK solvers (reduce or update less frequently)
   - Complex animator controllers (simplify or use Animancer)
   - Too many blend trees (reduce states)
5. Optimize per guidelines above
```

### "I need help beyond this guide"

```
1. Check main documentation: Docs/UNITY_NATIVE_ANIMATION_GUIDE.md
2. Search Unity Forums for your specific issue
3. Check official tool documentation (links above)
4. Create GitHub issue with:
   - What you were trying to do
   - What happened (include error messages)
   - What you expected to happen
   - Unity version, tool versions
```

---

## Workflow Comparison Chart

| Task | Blender Workflow | Unity-Native Workflow |
|------|-----------------|----------------------|
| **Create character model** | ✅ Best choice | ❌ Use Blender |
| **Edit animation** | ⚠️ Slow (export/import loop) | ✅ Fast (live preview) |
| **Add new animation** | ⚠️ Moderate (FBX export) | ✅ Fast (drag-drop .anim) |
| **Retarget animation** | ⚠️ Manual process | ✅ Automatic (Humanoid) |
| **Runtime IK** | ❌ Baked only | ✅ Full support |
| **Code integration** | ⚠️ Animator Controller needed | ✅ Animancer (optional) |
| **Agent-friendly** | ⚠️ Requires Blender expertise | ✅ Unity skills only |
| **Iteration speed** | 🐢 Slow (10-30 min/iteration) | 🚀 Fast (2-5 min/iteration) |

**Recommendation:** Use Blender for character creation, Unity-native for everything else.

---

## Next Steps

After completing your animation task:

1. **Test thoroughly** (use checklist above)
2. **Commit changes** with descriptive message
3. **Update this guide** if you discovered better methods
4. **Profile performance** if adding many animations
5. **Document any custom animations** in code comments

**Questions?** Open a GitHub issue or consult the full guide at `Docs/UNITY_NATIVE_ANIMATION_GUIDE.md`.

---

**Last Updated:** December 2024  
**For:** AI Agents working on Sangsom Mini-Me  
**Maintained By:** Development Team
