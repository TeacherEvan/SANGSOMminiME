# Sangsom Mini-Me - Extension & Tool Recommendations

## Essential Blender Addons

### Built-in Addons (Enable in Preferences > Add-ons)

1. **Rigify** (Built-in)
   - Purpose: Character rigging with pre-built control systems
   - Enable: Search "Rigify" in Add-ons
   - Usage: Generate professional character rigs quickly

2. **Auto-Rig Pro** (Commercial, Recommended)
   - Purpose: One-click character rigging
   - Link: https://blendermarket.com/products/auto-rig-pro
   - Why: Faster rigging workflow, export-friendly

3. **Pose Library** (Built-in, Blender 4.0+)
   - Purpose: Save and reuse character poses
   - Usage: Store animation poses for Mini-Me gestures

### Animation & Character

4. **Mixamo Animation Retargeting**
   - Purpose: Use Mixamo animations with custom rigs
   - Link: Search Blender Market
   - Why: Access to 1000s of free animations

5. **Animation Layers** (Built-in, Blender 4.0+)
   - Purpose: Layer-based animation editing
   - Usage: Combine base animations with additive layers

### Modeling & Sculpting

6. **MB-Lab / MPFB** (Free)
   - Purpose: Generate human base meshes
   - Link: https://github.com/animate1978/MB-Lab
   - Why: Quick character base for anime stylization

7. **Retopoflow** (Commercial)
   - Purpose: Clean retopology tools
   - Link: https://blendermarket.com/products/retopoflow
   - Why: Create game-ready topology from sculpts

### Texturing & Materials

8. **Anime Shader / Toon Shader Pack**
   - Purpose: Cel-shading for anime aesthetic
   - Options:
     - Komikaze (Free): Basic toon shading
     - Anime Shaders (Commercial): Advanced effects
   - Why: Core visual style for Mini-Me

9. **Textures.com Integration** (Free)
   - Purpose: Access texture libraries
   - Why: Quick material creation

### Export & Optimization

10. **GLTF/GLB Exporter** (Built-in)
    - Purpose: Export for web/mobile
    - Enable: Already enabled by default
    - Why: Best format for Three.js/WebGL

11. **Mesh Quality Check** (Built-in)
    - Purpose: Validate mesh topology
    - Enable: Mesh > Clean Up > Check All
    - Why: Ensure export-ready models

### Productivity

12. **Node Wrangler** (Built-in)
    - Purpose: Faster node editing
    - Enable: Search "Node Wrangler" in Add-ons
    - Why: Essential for shader work

13. **Copy Attributes Menu** (Built-in)
    - Purpose: Copy transforms between objects
    - Enable: Search "Copy Attributes" in Add-ons

14. **Import Images as Planes** (Built-in)
    - Purpose: Quick reference image import
    - Enable: Search "Import Images" in Add-ons
    - Why: Import character reference photos

---

## AI & External Tools

### AI Character Generation

1. **Cursor AI** (IDE)
   - Purpose: AI-assisted code generation
   - Link: https://cursor.sh
   - Usage: Generate Blender Python scripts, fix bugs

2. **Midjourney / DALL-E / Stable Diffusion**
   - Purpose: Generate character concept art
   - Usage: Create reference images for modeling

3. **ChatGPT / Claude**
   - Purpose: Blender scripting assistance
   - Usage: Generate complex Python code, debug scripts

### Image Processing

4. **Krita** (Free)
   - Purpose: Digital painting, texture creation
   - Link: https://krita.org
   - Why: Free Photoshop alternative

5. **GIMP** (Free)
   - Purpose: Image editing
   - Link: https://gimp.org
   - Why: Texture preparation

### Reference & Planning

6. **PureRef** (Pay what you want)
   - Purpose: Reference image organization
   - Link: https://pureref.com
   - Why: Keep character references visible

7. **Figma** (Free tier)
   - Purpose: UI/UX design
   - Link: https://figma.com
   - Why: Design game interfaces

---

## Recommended Blender Settings

### Preferences Configuration

```
Edit > Preferences > Interface
├── Display: Line Width = Thick (better visibility)
├── Editors: Python Tooltips = ON (for learning)
└── Translation: Interface = English (for tutorials)

Edit > Preferences > Input
├── Keyboard: Emulate Numpad = OFF (use numpad shortcuts)
└── Mouse: Emulate 3 Button = As needed

Edit > Preferences > Navigation
├── Auto: Auto Perspective = ON
└── Orbit: Turntable (more intuitive)

Edit > Preferences > System
├── Memory: Undo Steps = 64 (more undo history)
└── Sound: Audio Device = Your device
```

### Project-Specific Settings

```python
# Run in Blender Python console to apply settings
import bpy

# Render settings
scene = bpy.context.scene
scene.render.engine = 'BLENDER_EEVEE_NEXT'
scene.render.fps = 60
scene.render.resolution_x = 1920
scene.render.resolution_y = 1080

# EEVEE settings
scene.eevee.taa_render_samples = 64
scene.eevee.use_gtao = True
scene.eevee.use_bloom = True

# Color management (for anime style)
scene.view_settings.view_transform = 'Standard'
scene.view_settings.look = 'None'

print("Settings applied!")
```

---

## Learning Resources

### Blender Fundamentals
- Blender Guru (YouTube): Beginner tutorials
- CGCookie: Professional courses
- Blender Manual: https://docs.blender.org/manual

### Character Creation
- Royal Skies: Anime character tutorials
- Danny Mac: Blender rigging tutorials
- Imphenzia: Low-poly character techniques

### Blender Python Scripting
- Blender Python API: https://docs.blender.org/api
- Scripting for Artists (Blender Studio)
- Python for Beginners (Real Python)

### Game Development
- Three.js Documentation: https://threejs.org/docs
- Babylon.js Playground: https://playground.babylonjs.com
- WebGL Fundamentals: https://webglfundamentals.org

---

## Installation Checklist

```
[ ] Blender 5.0.0 installed
[ ] Cursor AI installed and configured
[ ] Enable built-in addons:
    [ ] Rigify
    [ ] Node Wrangler
    [ ] Import Images as Planes
    [ ] Copy Attributes Menu
[ ] Install Mini-Me addon (Blender/minime_addon.py)
[ ] Run startup script (Blender/startup_script.py)
[ ] Configure preferences as recommended
[ ] Set up reference images folder
[ ] Test Python scripting environment
```

---

## Troubleshooting

### Common Issues

**Addon won't install:**
- Ensure Blender version is 4.0+
- Check file permissions
- Run Blender as administrator (Windows)

**Scripts not running:**
- Check Python console for errors (Window > Toggle System Console)
- Verify Python path setup
- Restart Blender after script changes

**Performance issues:**
- Lower viewport samples
- Disable unneeded overlays
- Use simplified materials for editing

**Export problems:**
- Apply all transforms before export (Ctrl+A)
- Check for non-manifold geometry
- Limit texture sizes to 2048x2048

---

*Document maintained by Sangsom Mini-Me Development Team*
*Last updated: 2025*
