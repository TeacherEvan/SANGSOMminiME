<!-- 
# 🎮 Sangsom Mini-Me: Educational Tamagotchi Universe -->

> **An AI-powered educational gaming ecosystem where students nurture personalized 3D Mini-Me characters through academic achievement**

[![Unity](https://img.shields.io/badge/Unity-2022.3.12f1-black)](https://unity.com/)
[![Blender](https://img.shields.io/badge/Blender-5.0.0-blue)](https://www.blender.org/)
[![C#](https://img.shields.io/badge/C%23-.NET-purple)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Python](https://img.shields.io/badge/Python-3.11+-green)](https://www.python.org/)
[![License](https://img.shields.io/badge/License-Educational-orange)](LICENSE)

## 🌟 **Project Overview**

Sangsom Mini-Me combines modern AI-assisted development with educational game design to create personalized 3D tamagotchi-style characters for students. Built with **Unity 2022.3.12f1 LTS** and **Blender 5.0.0**, we generate anime-style characters from student photos and integrate homework completion directly into character care mechanics.

### 🎯 **Core Features**

- **🤖 AI Character Generation**: Create anime-style 3D models from 2-3 reference photos
- **📚 Educational Integration**: Homework completion drives character wellbeing and unlocks resources
- **🎨 Extensive Customization**: Scalable eye sizes, outfits, accessories, and jewelry
- **🏫 Multi-User System**: School-wide deployment with administrative controls
- **🎮 Minime Universe**: Educational side games that contribute resources to main character
- **🔐 Administrative Tools**: Password-protected teacher controls and progress monitoring

## 🚀 **Modern Development Stack**

This project leverages cutting-edge AI-assisted development for maximum productivity:

- **Unity 2022.3.12f1 LTS**: Industry-standard game engine for interactive gameplay
- **Blender 5.0.0**: 3D modeling and animation with Python scripting
- **C# .NET**: Unity scripting with strong type safety and modern language features
- **VSCode**: AI-powered code generation and debugging assistance  
- **Python 3.11+**: Blender automation and asset pipeline scripting
- **Data-driven Architecture**: JSON/YAML configuration for scalability
- **Multi-Structural Folders**: Optimized project organization for performance

## 📁 **Project Structure**

```
SANGSOMminiME/
├── .vscode/
│   └── rules/                  # AI behavior rules for development
│       ├── core-rules/         # Project-wide AI guidelines
│       ├── blender-rules/      # Blender-specific coding standards
│       ├── educational-rules/  # Educational game design principles
│       └── character-rules/    # Character generation and customization
├── Assets/                      # Unity project assets
│   ├── Characters/
│   │   └── Leandi/            # Test character (your wife as test model)
│   │       ├── Photos/        # Reference images for AI generation
│   │       ├── Models/        # Generated 3D models (exported from Blender)
│   │       └── Animations/    # Custom character animations
│   ├── Minime-Universe/
│   │   ├── Core-Game/         # Main tamagotchi systems
│   │   └── Side-Games/        # Educational mini-games
│   ├── Resources/
│   │   ├── Outfits/           # Purchasable clothing items
│   │   └── Accessories/       # Hats, jewelry, special items
│   ├── Scenes/                # Unity scene files (MainScene.unity)
│   └── Scripts/
│       ├── Runtime/           # Unity C# game logic
│       │   ├── Core/          # GameManager, UserManager
│       │   ├── Character/     # CharacterController
│       │   ├── UI/            # GameUI, LoginUI (TextMeshPro)
│       │   └── Educational/   # EducationalAnalytics
│       └── Tests/             # NUnit PlayMode tests
├── Blender/                     # Blender asset creation scripts
│   ├── startup_script.py      # Project initialization
│   ├── character_controller.py # Character system (mirrors Unity)
│   └── export_character.py    # Export to Unity formats
├── ProjectSettings/             # Unity project configuration
│   └── ProjectVersion.txt     # Unity 2022.3.12f1
└── SangsomMini-Me.mdc         # Updated project specification
```

## ⚡ **Quick Start with Unity & Blender**

### 1. **Setup Development Environment**

```bash
# Install Unity Hub and Unity 2022.3.12f1 LTS
# Download from: https://unity.com/download

# Install Blender 5.0.0
# Download from: https://www.blender.org/download/

# Install VSCode Editor (recommended for C# and Python)
# Download from: https://code.visualstudio.com/

# Clone this repository
git clone https://github.com/yourusername/SANGSOMminiME.git
cd SANGSOMminiME
```

### 2. **Open Unity Project**

```bash
# Open Unity Hub
# Click "Add" and select the SANGSOMminiME folder
# Unity will open with version 2022.3.12f1
# Open MainScene.unity from Assets/Scenes/
```

### 3. **Configure Blender for Asset Export**

```bash
# Open Blender 5.0.0
# Set VSCode as your external script editor (Edit > Preferences > File Paths)
# Run Blender/startup_script.py to initialize project settings

# Install Blender Python API stubs for better code completion in VSCode
pip install blender-stubs
```

### 3. **AI-Assisted Development Setup**

The project includes pre-configured **VSCode rules** and **Unity C# scripts** that will automatically:

- ✅ Generate Unity C# code following SangsomMiniMe namespace conventions
- ✅ Generate Blender Python code for asset creation following project standards
- ✅ Apply educational game design principles
- ✅ Create character customization systems
- ✅ Implement multi-user account management with Unity's JsonUtility
- ✅ Build nurturing game mechanics without stress/timers

## 🧠 **AI-Powered Development Features**

### **Intelligent Code Generation**

Ask VSCode to:

```
"Create a Unity C# character customization system with scalable eye sizes following SangsomMiniMe namespace conventions"

"Generate a Blender Python script to export character animations to Unity FBX format"

"Build a Unity homework integration system that increases character happiness when academic tasks complete"

"Create Unity animation controller for Thai cultural gestures: wai, curtsy, bow with smooth transitions"
```

### **Educational Game Design**

The AI understands research-backed principles for educational gaming:

- **No stress mechanics**: No timers or failure states
- **Authentic nurturing**: Characters develop meaningful relationships with students
- **Academic integration**: Homework completion directly improves character wellbeing
- **Cultural sensitivity**: Proper implementation of Thai gestures (wai) and customs

## 👩‍🏫 **Phase 1: Leandi Test Model**

The first development phase creates a working prototype using **Leandi** (your wife) as the test subject:

### **Implementation Steps**

1. **📸 Photo Setup**: Upload 2-3 reference photos to `Assets/Characters/Leandi/Photos/`
2. **🤖 AI Generation**: Use Blender and AI to generate anime-style 3D model from photos
3. **🎨 Customization**: Implement eye size scaling and basic outfit system in Unity C#
4. **🎭 Animation**: Create dance, wave, wai, curtsy, and bow animations in Blender, export to Unity
5. **📚 Educational**: Connect character care to mock homework completion system in Unity

### **AI Prompts for Phase 1**

```
"Generate an anime-style 3D character model in Blender based on the photos in Assets/Characters/Leandi/Photos/ with customizable eye scaling and attachment points for accessories. Export as FBX for Unity."

"Create a Unity C# CharacterController script with methods for eye scaling, outfit changes, and playing animations (dance, wave, wai, curtsy, bow) using Unity's Animator component."

"Build a Unity homework completion system in C# that increases the Leandi character's happiness and unlocks new customization options. Use Unity's JsonUtility for save/load."
```

## 🎮 **Game Design Philosophy**

### **AI Code Generation Examples**

```
"Create a Unity C# character customization system with scalable eye sizes"
"Generate a Unity educational homework integration system that rewards character care"
"Build a Unity multi-user account system with password-protected admin controls using JsonUtility"
"Create a Unity animation controller with Blender-exported anime-style character animations and cultural gestures"
```

### **Educational Nurturing Principles**

- **Cozy Gameplay**: Slow, meditative pace encouraging thoughtful interaction
- **Meaningful Progression**: Characters develop personality based on care patterns
- **Cross-Game Currency**: Resources from Minime Universe side games benefit main character
- **Administrative Oversight**: Teachers monitor engagement without invading student privacy

### **Technical Architecture**

- **Modular Design**: Unity C# systems work independently and together seamlessly
- **Event-Driven**: Loose coupling using Unity's event system and C# delegates
- **Data-Driven**: JSON serialization via Unity's JsonUtility for all user data
- **Performance-First**: Mobile-optimized with 60fps target, object pooling for efficiency
- **Dual Animation Pipeline**: 
  - **Blender Workflow**: Custom character modeling and rigging, export to Unity via FBX/GLB
  - **Unity-Native Workflow**: Animation editing and control with UMotion Pro, Animancer, Final IK
  - See [Unity-Native Animation Guide](Docs/UNITY_NATIVE_ANIMATION_GUIDE.md) and [Agent Quickstart](Docs/ANIMATION_QUICKSTART_AGENTS.md)

## 🛠️ **Development Roadmap**

### **Phase 1 - Leandi Test Model** *(Current)*

- [x] Project structure and VSCode rules setup
- [x] Unity 2022.3.12f1 project initialization
- [x] Unity C# core systems (GameManager, UserManager, CharacterController)
- [x] Unity-native animation pipeline documentation (UMotion Pro, Animancer, Final IK)
- [ ] Generate Leandi character from photos using AI in Blender
- [ ] Export character to Unity (FBX format)
- [ ] Implement basic customization in Unity (eye scaling, outfits)
- [ ] Create essential animations (dance, wave, wai, curtsy, bow)
  - Option A: Blender workflow (traditional)
  - Option B: Unity-native tools (faster iteration)
- [ ] Import animations into Unity Animator or Animancer
- [ ] Build homework integration prototype in Unity C#

### **Phase 2 - Core Systems**

- [ ] Multi-user account framework in Unity C# with password protection
- [ ] Room environments in Unity scenes with interaction systems
- [ ] Administrative dashboard UI for teachers using Unity TextMeshPro
- [ ] Performance optimization using Unity Profiler and object pooling
- [ ] Mobile deployment builds (Android/iOS) via Unity Build Pipeline

### **Phase 3 - Educational Integration**

- [ ] School homework platform API integration in Unity C#
- [ ] First Minime Universe side game development in Unity
- [ ] Cross-game resource system implementation using Unity ScriptableObjects
- [ ] Parent/teacher progress reporting with Unity Analytics

### **Phase 4 - Universe Expansion**

- [ ] Additional educational games for Minime Universe in Unity
- [ ] Advanced customization features (jewelry, seasonal items) with Unity addressables
- [ ] Cultural content expansion (international outfits, gestures)
- [ ] Scalability improvements for large school deployments using Unity multiplayer services

## 🎯 **Success Metrics**

- **📈 Student Engagement**: Increased homework completion rates
- **⏱️ Platform Usage**: Daily active time with Mini-Me characters
- **🎓 Academic Performance**: Correlation between character care and grades
- **🏫 School Adoption**: Number of schools implementing the system

## 🤝 **Contributing**

This project uses **AI-assisted development** with Unity and VSCode. When contributing:

1. Use Unity 2022.3.12f1 LTS for all Unity work
2. Use the provided VSCode rules for consistent C# and Python code generation
3. Follow the established project structure and SangsomMiniMe namespace conventions
4. Test all changes in Unity PlayMode with the Leandi test character first
5. Use Unity Test Runner (NUnit) to run automated tests
6. Ensure mobile optimization for all new features (test with Unity Profiler)
7. Document AI prompts used for significant code generation in History2.md
8. Export Blender assets to Unity-compatible formats (FBX for models/animations)

## 📄 **License**

Educational Use License - See [LICENSE](LICENSE) for details.

## 🙏 **Acknowledgments**

- **Educational Game Design Research**: Based on nurturing game design principles from Polaris Game Design
- **Unity Development**: Following Unity 2022.3 LTS best practices for educational games
- **Blender Development**: Following modern Blender 5.0 best practices for character creation
- **AI-Assisted Development**: Powered by VSCode and GitHub Copilot for rapid prototyping and iteration
- **Cultural Sensitivity**: Respectful implementation of Thai cultural elements

---

**🚀 Ready to build the future of educational gaming with AI assistance!**

*For questions, issues, or contributions, please open a GitHub issue or contact the development team.*
