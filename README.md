# 🎮 Sangsom Mini-Me: Educational Tamagotchi Universe

> **An AI-powered educational gaming ecosystem where students nurture personalized 3D Mini-Me characters through academic achievement**

[![Unity](https://img.shields.io/badge/Unity-2022.3_LTS-blue)](https://unity3d.com/)
[![Cursor AI](https://img.shields.io/badge/Cursor_AI-Enabled-green)](https://cursor.sh/)
[![C#](https://img.shields.io/badge/C%23-8.0+-purple)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-Educational-orange)](LICENSE)

## 🌟 **Project Overview**

Sangsom Mini-Me combines modern AI-assisted development with educational game design to create personalized 3D tamagotchi-style characters for students. Using **Cursor AI** and **Unity**, we generate anime-style characters from student photos and integrate homework completion directly into character care mechanics.

### 🎯 **Core Features**
- **🤖 AI Character Generation**: Create anime-style 3D models from 2-3 reference photos
- **📚 Educational Integration**: Homework completion drives character wellbeing and unlocks resources
- **🎨 Extensive Customization**: Scalable eye sizes, outfits, accessories, and jewelry
- **🏫 Multi-User System**: School-wide deployment with administrative controls
- **🎮 Minime Universe**: Educational side games that contribute resources to main character
- **🔐 Administrative Tools**: Password-protected teacher controls and progress monitoring

## 🚀 **Modern Development Stack**

This project leverages cutting-edge AI-assisted development for maximum productivity:

- **Unity 2022.3 LTS**: Game engine with WebGL deployment
- **Cursor AI**: AI-powered code generation and debugging assistance  
- **C# 8.0+**: Modern language features with nullable reference types
- **ScriptableObject Architecture**: Data-driven design for scalability
- **Multi-Structural Folders**: Optimized project organization for performance

## 📁 **Project Structure**

```
SANGSOMminiME/
├── .cursor/
│   └── rules/                  # AI behavior rules for development
│       ├── core-rules/         # Project-wide AI guidelines
│       ├── unity-rules/        # Unity-specific coding standards
│       ├── educational-rules/  # Educational game design principles
│       └── character-rules/    # Character generation and customization
├── Assets/
│   ├── Characters/
│   │   └── Leandi/            # Test character (your wife as test model)
│   │       ├── Photos/        # Reference images for AI generation
│   │       ├── Models/        # Generated 3D models
│   │       └── Animations/    # Custom character animations
│   ├── Minime-Universe/
│   │   ├── Core-Game/         # Main tamagotchi systems
│   │   └── Side-Games/        # Educational mini-games
│   ├── Resources/
│   │   ├── Outfits/           # Purchasable clothing items
│   │   └── Accessories/       # Hats, jewelry, special items
│   └── Scripts/
│       ├── Core/              # Game management systems
│       ├── Character/         # Character behavior and customization
│       ├── UI/                # Interface and menu systems
│       └── Educational/       # Homework integration
└── SangsomMini-Me.mdc         # Updated project specification
```

## ⚡ **Quick Start with Cursor AI**

### 1. **Setup Development Environment**
```bash
# Install Unity 2022.3 LTS
# Download from: https://unity3d.com/get-unity/download

# Install Cursor AI Editor
# Download from: https://cursor.sh/

# Clone this repository
git clone https://github.com/yourusername/SANGSOMminiME.git
cd SANGSOMminiME
```

### 2. **Configure Cursor AI for Unity**
```bash
# Set Cursor as Unity's external script editor
# Unity → Edit → Preferences → External Tools → External Script Editor → Cursor

# Install Unity-Cursor integration plugin
# Package Manager → Add package from git URL:
# https://github.com/boxqkrtm/com.unity.ide.cursor.git
```

### 3. **AI-Assisted Development Setup**
The project includes pre-configured **Cursor AI rules** that will automatically:
- ✅ Generate Unity C# code following project standards
- ✅ Apply educational game design principles
- ✅ Create character customization systems
- ✅ Implement multi-user account management
- ✅ Build nurturing game mechanics without stress/timers

## 🧠 **AI-Powered Development Features**

### **Intelligent Code Generation**
Ask Cursor AI to:
```
"Create a character customization system with scalable eye sizes using Unity sliders"
"Generate an educational homework integration system that rewards character care"
"Build a multi-user account system with password-protected admin controls"
"Create an anime-style character animation controller with cultural gestures"
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
2. **🤖 AI Generation**: Use Cursor AI to generate anime-style 3D model from photos
3. **🎨 Customization**: Implement eye size scaling and basic outfit system
4. **🎭 Animation**: Create dance, wave, wai, curtsy, and bow animations
5. **📚 Educational**: Connect character care to mock homework completion system

### **AI Prompts for Phase 1**
```
"Generate an anime-style 3D character model based on the photos in Assets/Characters/Leandi/Photos/ with customizable eye scaling and attachment points for accessories"

"Create a Unity animation controller for the Leandi character with smooth transitions between idle, dance, wave, wai, curtsy, and bow animations"

"Build a homework completion system that increases the Leandi character's happiness and unlocks new customization options"
```

## 🎮 **Game Design Philosophy**

### **Educational Nurturing Principles**
- **Cozy Gameplay**: Slow, meditative pace encouraging thoughtful interaction
- **Meaningful Progression**: Characters develop personality based on care patterns
- **Cross-Game Currency**: Resources from Minime Universe side games benefit main character
- **Administrative Oversight**: Teachers monitor engagement without invading student privacy

### **Technical Architecture**
- **Modular Design**: Systems work independently and together seamlessly
- **Event-Driven**: Loose coupling using Unity Events and C# events
- **Data-Driven**: ScriptableObjects for all configuration and content
- **Performance-First**: Mobile-optimized with 60fps target

## 🛠️ **Development Roadmap**

### **Phase 1 - Leandi Test Model** *(Current)*
- [x] Project structure and Cursor AI rules setup
- [ ] Generate Leandi character from photos using AI
- [ ] Implement basic customization (eye scaling, outfits)
- [ ] Create essential animations (dance, wave, wai, curtsy, bow)
- [ ] Build homework integration prototype

### **Phase 2 - Core Systems**
- [ ] Multi-user account framework with password protection
- [ ] Room environments and interaction systems
- [ ] Administrative dashboard for teachers
- [ ] Performance optimization and mobile deployment

### **Phase 3 - Educational Integration**
- [ ] School homework platform API integration
- [ ] First Minime Universe side game development
- [ ] Cross-game resource system implementation
- [ ] Parent/teacher progress reporting

### **Phase 4 - Universe Expansion**
- [ ] Additional educational games for Minime Universe
- [ ] Advanced customization features (jewelry, seasonal items)
- [ ] Cultural content expansion (international outfits, gestures)
- [ ] Scalability improvements for large school deployments

## 🎯 **Success Metrics**

- **📈 Student Engagement**: Increased homework completion rates
- **⏱️ Platform Usage**: Daily active time with Mini-Me characters
- **🎓 Academic Performance**: Correlation between character care and grades
- **🏫 School Adoption**: Number of schools implementing the system

## 🤝 **Contributing**

This project uses **AI-assisted development** with Cursor AI. When contributing:

1. Use the provided Cursor rules for consistent code generation
2. Follow the established project structure and naming conventions
3. Test all changes with the Leandi test character first
4. Ensure mobile optimization for all new features
5. Document AI prompts used for significant code generation

## 📄 **License**

Educational Use License - See [LICENSE](LICENSE) for details.

## 🙏 **Acknowledgments**

- **Educational Game Design Research**: Based on nurturing game design principles from Polaris Game Design
- **Unity Development**: Following modern Unity best practices for 2024/2025
- **AI-Assisted Development**: Powered by Cursor AI for rapid prototyping and iteration
- **Cultural Sensitivity**: Respectful implementation of Thai cultural elements

---

**🚀 Ready to build the future of educational gaming with AI assistance!**

*For questions, issues, or contributions, please open a GitHub issue or contact the development team.* 