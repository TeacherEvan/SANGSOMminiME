# Blender Setup & Optimization Guide

## 1. Essential Blender Configuration

To ensure the "Sangsom Mini-Me" project runs correctly and AI integration works smoothly:

### A. Enable Python Execution (CRITICAL)

1. Go to **Edit > Preferences**.
2. Select the **Save & Load** tab.
3. Check **Auto Run Python Scripts**.
   * *Why:* The project relies on `startup_script.py` and `minime_addon.py` to initialize the game state. Without this, the game logic won't load.

### B. Developer Tools (Recommended)

1. Go to **Interface** tab.
2. Check **Python Tooltips**.
   * *Why:* Hovering over any button shows the Python command. Essential for AI to know what commands to generate.
3. Check **Developer Extras**.
   * *Why:* Adds "Copy Python Command" to right-click menus.

### C. Install the Addon

1. Go to **Add-ons** tab.
2. Click **Install...** and select `Blender/minime_addon.py`.
3. Enable **Sangsom Mini-Me Tools**.

---

## 2. AI Character Generation Workflow (Phase 1)

Since Blender does not natively generate 3D characters from photos, use this optimized workflow:

### Option A: The "AI Magic" Route (Fast)

**Tools:** [Rodin (Hyper3D)](https://hyperhuman.deemos.com/) + [AccuRIG](https://actorcore.reallusion.com/auto-rig)

1. **Generate:** Upload 2-3 photos to Rodin. Use prompt: *"Anime style character, clean topology"*.
2. **Export:** Download `.glb` or `.fbx`.
3. **Rig:** Import to AccuRIG (Free). Auto-rig the body. Export as FBX.
4. **Import:** Import FBX into Blender.
5. **Style:** Apply a "Toon Shader" (Cel Shading) material in Blender.

### Option B: The "Educational" Route (Recommended)

**Tool:** [VRoid Studio](https://vroid.com/en/studio) (Free)

1. **Create:** Student uses their photo as a reference background in VRoid.
2. **Customize:** Manually adjust sliders to match their face (Educational value: Art/Anatomy).
3. **Export:** Export as `.vrm`.
4. **Import:** Use "VRM Addon for Blender" to import. Character is fully rigged with facial expressions.

---

## 3. Optimization Notes

* **Game Loop:** The `GameManager` now uses `bpy.app.handlers.frame_change_pre` for the game loop. This is efficient for logic updates.
* **Data Saving:** User data saving is currently synchronous. In the future, this should be asynchronous or batched to prevent frame drops during auto-save.
* **File Structure:**
  * `Blender/*.py`: The actual game code.
  * `Assets/Scripts/Runtime/*.cs`: Reference Unity code (do not use in Blender).

