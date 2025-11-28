# Art Assets

Organized art assets for the Sangsom Mini-Me project.

## Structure

- **Animation/**: Animation clips and controllers
- **Audio/**: Sound effects and music
- **Fonts/**: Typography assets
- **Materials/**: Material definitions
- **Models/**: 3D models (FBX, OBJ)
- **Shaders/**: Custom shaders
- **Sprites/**: 2D sprites and UI elements
- **Textures/**: Texture files

## Naming Conventions

- Use snake_case for all asset names
- Prefix with category: `char_`, `ui_`, `env_`, `fx_`
- Include variant suffix: `_01`, `_normal`, `_emission`

## Performance Guidelines

- Textures: Max 2048x2048, prefer 1024x1024
- Models: Max 10k triangles for characters
- Audio: Use compressed formats (OGG/MP3)
