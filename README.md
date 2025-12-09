# 🎮 Sangsom Mini-Me: Educational Tamagotchi Universe

> **An AI-powered educational gaming ecosystem where students nurture personalized 3D Mini-Me characters through academic achievement**

[![Unity](https://img.shields.io/badge/Unity-6000.2.15f1-black)](https://unity.com/)
[![C#](https://img.shields.io/badge/C%23-.NET-purple)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-Educational-orange)](LICENSE)

## 🌟 **Project Overview**

Sangsom Mini-Me combines modern AI-assisted development with educational game design to create personalized 3D tamagotchi-style characters for students. Built entirely with **Unity 6000.2.15f1**, we create anime-style characters that integrate homework completion directly into character care mechanics.

### 🎯 **Core Features**

- **🤖 AI Character Generation**: Create anime-style 3D models from reference photos
- **📚 Educational Integration**: Homework completion drives character wellbeing and unlocks resources
- **🎨 Extensive Customization**: Scalable eye sizes, outfits, accessories, and jewelry
- **🏫 Multi-User System**: School-wide deployment with administrative controls
- **⚡ High Performance**: Async I/O and optimized data structures for smooth 60 FPS gameplay
- **🎮 Minime Universe**: Educational side games that contribute resources to main character
- **🔐 Administrative Tools**: Password-protected teacher controls and progress monitoring

## 🚀 **Development Stack**

- **Unity 6000.2.15f1**: Industry-standard game engine for interactive gameplay
- **C# .NET**: Unity scripting with strong type safety and modern language features
- **Async Architecture**: Task-based I/O for non-blocking persistence
- **VSCode**: AI-powered code generation and debugging assistance
- **Data-driven Architecture**: JSON configuration via JsonUtility for scalability
- **NUnit Testing**: PlayMode tests for quality assurance

## 📁 **Project Structure**

```
SANGSOMminiME/
├── Assets/                      # Unity project assets
│   ├── 3rdParty/               # External Unity packages
│   ├── Art/                    # Art assets organized by type
│   │   ├── Animation/          # Animation clips
│   │   ├── Audio/              # Sound effects & music
│   │   ├── Materials/          # Unity materials
│   │   ├── Models/             # 3D models (FBX)
│   │   ├── Textures/           # Texture files
│   │   └── ...
│   ├── Characters/             # Per-character folders
│   │   └── Leandi/             # Test character
│   │       └── Photos/         # Reference images
│   ├── Data/                   # ScriptableObject data
│   ├── Minime-Universe/        # Educational mini-games
│   │   ├── Core-Game/          # Main tamagotchi systems
│   │   └── Side-Games/         # Educational mini-games
│   ├── Prefabs/                # Unity prefabs
│   ├── Resources/              # Runtime loadable assets
│   │   ├── Outfits/            # Purchasable clothing
│   │   └── Accessories/        # Hats, jewelry, items
│   ├── Scenes/                 # Unity scenes
│   │   └── MainScene.unity     # Primary game scene
│   ├── Scripts/                # C# Unity scripts
│   │   ├── Runtime/            # Game logic
│   │   │   ├── GameManager.cs
│   │   │   ├── UserManager.cs
│   │   │   ├── CharacterController.cs
│   │   │   ├── GameUI.cs
│   │   │   └── ...
│   │   ├── Editor/             # Unity editor tools
│   │   └── Tests/              # NUnit PlayMode tests
│   └── Settings/               # Unity project settings
├── Docs/                       # Documentation
├── ProjectSettings/            # Unity project configuration
├── Packages/                   # Unity Package Manager
└── .vscode/                    # VSCode configuration
```

## ⚡ **Quick Start**

### 1. **Prerequisites**

- [Unity Hub](https://unity.com/download) with Unity 6000.2.15f1
- [VSCode](https://code.visualstudio.com/) (recommended for C# development)
- Git for version control

### 2. **Setup**

```bash
# Clone the repository
git clone https://github.com/TeacherEvan/SANGSOMminiME.git
cd SANGSOMminiME

# Open Unity Hub
# Click "Add" and select the SANGSOMminiME folder
# Unity will open with version 6000.2.15f1
# Open Assets/Scenes/MainScene.unity
```

### 3. **Run the Project**

1. Open Unity Hub and add the project
2. Open `Assets/Scenes/MainScene.unity`
3. Press **Play** to test the login screen and character system
4. Use Unity Test Runner (`Window > General > Test Runner`) to run tests

## 🧠 **Architecture Overview**

### **Core Systems**

| Component                 | Description                                                  |
| ------------------------- | ------------------------------------------------------------ |
| `GameManager.cs`          | Singleton orchestrating game state, login flow, and autosave |
| `UserManager.cs`          | User profile persistence via JsonUtility                     |
| `CharacterController.cs`  | Character animations and customization                       |
| `GameUI.cs`               | TMP/UGUI interface bridge                                    |
| `EducationalAnalytics.cs` | Homework tracking and rewards                                |

### **Namespace Structure**

```csharp
SangsomMiniMe.Core        // GameManager, state management
SangsomMiniMe.Character   // CharacterController, animations
SangsomMiniMe.UI          // GameUI, LoginUI (TextMeshPro)
SangsomMiniMe.Educational // Analytics, homework integration
```

## 🎮 **Game Design Philosophy**

### **Educational Nurturing Principles**

- **No Stress Mechanics**: No timers or failure states
- **Cozy Gameplay**: Slow, meditative pace encouraging thoughtful interaction
- **Meaningful Progression**: Characters develop based on care patterns
- **Academic Integration**: Homework completion improves character wellbeing
- **Cross-Game Currency**: Resources from side games benefit main character

### **Technical Architecture**

- **Modular Design**: Systems work independently and together
- **Event-Driven**: Loose coupling using C# delegates and Unity events
- **Data-Driven**: JSON serialization via JsonUtility
- **Performance-First**: Mobile-optimized with 60fps target, object pooling

## 🛠️ **Development Roadmap**

> 📖 **See [Docs/GAMEPLAY_UX_GUIDE.md](Docs/GAMEPLAY_UX_GUIDE.md) for detailed UX specifications and tamagotchi-style best practices.**

### **Phase 1 - Core Systems** ✅ _Complete_

- [x] Unity 6000.2.15f1 project initialization
- [x] C# core systems (GameManager, UserManager, CharacterController)
- [x] Basic UI framework with TextMeshPro
- [x] Event-driven architecture (delegates, OnUserLoggedIn, etc.)
- [x] Async I/O persistence (UserManager.SaveAsync)
- [x] Educational analytics foundation (EducationalAnalytics.cs)

### **Phase 2 - Engagement Loop** ✅ _Complete_

- [x] Daily login bonus system (positive-only streaks)
- [x] Streak tracking with reset protection (no penalties)
- [x] Homework completion flow UI
- [x] Celebration animations & sound effects
- [x] Meter decay system (Happiness/Hunger/Energy with floors)

### **Phase 3 - Character & Customization** 🔄 _(Current Priority)_

- [x] Eye scaling and appearance customization
- [ ] Outfit system with attachment points
- [ ] Accessory system (hats, jewelry, items)
- [x] Shop UI with currency display
- [x] Rarity tier visual indicators (Common → Legendary)
- [ ] Thai gesture animations (Wai, Curtsy, Bow)

### **Phase 4 - Mini-Games & Universe** 📋

- [ ] Minime Universe side game integration
- [ ] Cross-game coin/XP rewards
- [ ] Progress sync with main character
- [ ] ScriptableObject-based resource system
- [ ] Classroom leaderboards (optional)

### **Phase 5 - Multi-User & Social** 📋

- [ ] Multi-user account framework with password protection
- [ ] Teacher administrative dashboard
- [ ] Classroom group system
- [ ] Friend Mini-Me visits & gift exchange
- [ ] Parent/teacher progress reporting

### **Phase 6 - Polish & Deployment** 📋

- [ ] Performance profiling (Unity Profiler, 60 FPS target)
- [ ] Battery optimization for mobile
- [ ] Accessibility audit
- [ ] Localization (Thai/English)
- [ ] Mobile deployment (Android/iOS)

## 🧪 **Testing**

```bash
# Run tests via Unity Test Runner
# Window > General > Test Runner > Run All

# Or via command line
Unity.exe -runTests -testResults results.xml -projectPath .
```

### **Test Coverage**

- `UserProfileTests.cs` - Profile persistence validation
- `GameUtilitiesTests.cs` - Utility function testing

## 🤝 **Contributing**

1. Use Unity 6000.2.15f1 for all work
2. Follow SangsomMiniMe namespace conventions
3. Test changes with the Leandi test character
4. Run Unity Test Runner before committing
5. Document AI prompts in History2.md

## 📄 **License**

Educational Use License - See [LICENSE](LICENSE) for details.

## 🙏 **Acknowledgments**

- **Educational Game Design Research**: Nurturing game design principles from Polaris Game Design
- **Unity Development**: Unity 2022.3 LTS best practices
- **AI-Assisted Development**: Powered by VSCode and GitHub Copilot
- **Cultural Sensitivity**: Respectful implementation of Thai cultural elements

---

**🚀 Ready to build the future of educational gaming!**

_For questions, issues, or contributions, please open a GitHub issue._
