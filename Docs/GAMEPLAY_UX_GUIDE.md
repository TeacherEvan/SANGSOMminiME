# Sangsom Mini-Me: Gameplay UX Guide

## Tamagotchi-Style Virtual Pet Best Practices for Educational Applications

_Version 1.0 | December 2025_

---

## Table of Contents

1. [Core Design Philosophy](#core-design-philosophy)
2. [The Care Loop](#the-care-loop)
3. [Meter Systems](#meter-systems)
4. [Reward Mechanics](#reward-mechanics)
5. [User Interaction Flow](#user-interaction-flow)
6. [Optimization Techniques](#optimization-techniques)
7. [Cultural Considerations](#cultural-considerations)
8. [Implementation Checklist](#implementation-checklist)

---

## Core Design Philosophy

### Tamagotchi Principles Adapted for Education

The original Tamagotchi (1996, Bandai) created emotional bonds through:

- **Simple but expressive visuals** - Pixel art conveying emotion
- **Real-time care requirements** - Pet needs attention throughout day
- **Consequence mechanics** - Poor care = pet death
- **Evolution systems** - Care quality affects final form

### Sangsom Mini-Me Adaptations

| Classic Tamagotchi         | Sangsom Mini-Me                              |
| -------------------------- | -------------------------------------------- |
| Pet dies from neglect      | Pet gets lonely but **never dies**           |
| Punishment for missed care | Positive reinforcement only                  |
| Random evolution           | Progress tied to **homework completion**     |
| Constant attention needed  | Scheduled check-ins respecting school hours  |
| Generic pet                | Culturally authentic **Thai student avatar** |

### Golden Rule

> **"Educational gameplay forbids timers/fail states; reward homework completion with happiness/coins"**
> — Project Design Principles

---

## The Care Loop

### Daily Engagement Cycle

```
┌─────────────────────────────────────────────────────────┐
│                    MORNING (6-8 AM)                     │
│  • Wake-up greeting animation                           │
│  • Check daily goals                                    │
│  • Quick care actions                                   │
└─────────────────────────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────┐
│                   SCHOOL HOURS (8AM-3PM)                │
│  • App in passive mode                                  │
│  • NO notifications during class                        │
│  • Offline progress accumulation                        │
└─────────────────────────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────┐
│                  AFTERNOON (3-6 PM)                     │
│  • Homework completion flow                             │
│  • Mini-games in Minime Universe                        │
│  • Customization shopping                               │
└─────────────────────────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────┐
│                   EVENING (6-8 PM)                      │
│  • Social interactions (virtual classroom)              │
│  • Thai cultural gestures practice                      │
│  • Daily summary and rewards                            │
└─────────────────────────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────┐
│                    NIGHT (8 PM+)                        │
│  • Sleep animation                                      │
│  • Streak bonus calculated                              │
│  • Auto-save triggered                                  │
└─────────────────────────────────────────────────────────┘
```

### Core Actions

| Action        | Trigger         | Reward                     | Animation        |
| ------------- | --------------- | -------------------------- | ---------------- |
| **Feed**      | Hunger < 50%    | +20 Hunger, +5 Happy       | Eating animation |
| **Play**      | User initiated  | +15 Happy, +10 XP          | PlayDance()      |
| **Learn**     | Homework submit | +30 Happy, +25 XP, +Coins  | Celebration      |
| **Greet**     | Thai gesture    | +10 Happy, cultural unlock | Wai/Curtsy/Bow   |
| **Rest**      | Energy < 30%    | +40 Energy                 | Sleep cycle      |
| **Customize** | Shop purchase   | Cosmetic change            | Outfit swap      |

---

## Meter Systems

### Primary Meters

```csharp
// UserProfile.cs meter definitions
public float Happiness { get; private set; }  // 0-100, floors at 50
public float Hunger { get; private set; }     // 0-100, floors at 20
public float Energy { get; private set; }     // 0-100, natural regen
```

### Meter Behavior

#### Happiness (Primary Engagement Metric)

- **Decay Rate:** -2/hour (slow, forgiving)
- **Floor:** 50% (never critical)
- **Boost Sources:**
  - Homework completion: +30
  - Mini-game win: +15
  - Daily login: +10
  - Thai gesture: +10

#### Hunger (Care Reminder)

- **Decay Rate:** -5/hour
- **Floor:** 20% (always survivable)
- **Restore:** Feeding action (+20)

#### Energy (Activity Limiter)

- **Decay Rate:** -10/activity
- **Natural Regen:** +5/hour
- **Full Restore:** Sleep cycle

### Anti-Addiction Safeguards

```
┌────────────────────────────────────────────────────────────┐
│  METER FLOORS PREVENT NEGATIVE SPIRALS                    │
│                                                            │
│  Happiness: ████████████████████░░░░░░░░░░ 50% MIN        │
│  Hunger:    ████████░░░░░░░░░░░░░░░░░░░░░░ 20% MIN        │
│  Energy:    ████░░░░░░░░░░░░░░░░░░░░░░░░░░ 10% MIN        │
│                                                            │
│  → User can always recover from any state                  │
│  → No punishment for extended absence                      │
│  → Positive-sum gameplay only                              │
└────────────────────────────────────────────────────────────┘
```

---

## Reward Mechanics

### Currency System

| Currency  | Earn Method                   | Spend On             |
| --------- | ----------------------------- | -------------------- |
| **Coins** | Homework, mini-games, streaks | Outfits, accessories |
| **XP**    | All activities                | Level progression    |
| **Stars** | Perfect homework              | Rare cosmetics       |

### Progression System

```
Level 1-5:   NEWCOMER    → Basic customization unlocked
Level 6-10:  LEARNER     → Mini-game access
Level 11-20: STUDENT     → Side games unlocked
Level 21-30: SCHOLAR     → Rare cosmetics available
Level 31-50: MASTER      → All content unlocked
Level 50+:   LEGEND      → Prestige rewards
```

### Streak Bonuses (Positive Only)

```javascript
// Daily login rewards - NO PENALTIES for missing days
const streakBonuses = {
  1: { coins: 10, message: "Welcome back!" },
  3: { coins: 25, message: "3 days strong!" },
  7: { coins: 50, xpBoost: 1.1, message: "Weekly warrior!" },
  14: { coins: 100, xpBoost: 1.2, message: "Two week streak!" },
  30: {
    coins: 250,
    xpBoost: 1.5,
    rareUnlock: true,
    message: "Monthly master!",
  },
};

// Missing a day: streak resets but NO penalty applied
// User keeps all earned rewards and can rebuild streak
```

### Collectibility (Tamagotchi Lesson: Multiple Characters)

```
OUTFIT RARITY TIERS:
├── Common (60%)     - Basic colors, always available
├── Uncommon (25%)   - Patterns, unlocked via progression
├── Rare (10%)       - Animated effects, streak rewards
├── Epic (4%)        - Cultural costumes, achievement unlock
└── Legendary (1%)   - Limited events, perfect attendance
```

---

## User Interaction Flow

### Onboarding Sequence

```
1. WELCOME SCREEN
   └── "Welcome to Sangsom Mini-Me!"

2. PROFILE CREATION
   ├── Enter name (UserManager.CreateUser)
   ├── Choose display name
   └── Select initial appearance

3. MINI-ME HATCHING (Tamagotchi homage)
   ├── Egg animation
   ├── Character reveal
   └── First interaction prompt

4. TUTORIAL LOOP
   ├── "Try feeding your Mini-Me" → Feed action
   ├── "Let's play a game" → Mini-game intro
   ├── "Complete your first task" → Homework flow
   └── "You earned coins!" → Shop preview

5. DAILY FLOW BEGINS
   └── Full gameplay unlocked
```

### Homework Integration Flow

```
┌─────────────────────────────────────────────────────────┐
│  TEACHER                                                │
│  Assigns homework via external LMS                      │
└───────────────────────┬─────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────┐
│  STUDENT                                                │
│  Completes homework (physical/digital)                  │
└───────────────────────┬─────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────┐
│  APP: LOG COMPLETION                                    │
│  Student opens app → taps "Log Homework"                │
│  → Selects subject/type                                 │
│  → Optional: photo proof upload                         │
└───────────────────────┬─────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────┐
│  MINI-ME CELEBRATION                                    │
│  • PlayDance() animation triggered                      │
│  • Confetti particle effect                             │
│  • Sound effect: celebration chime                      │
│  • "+30 Happiness! +25 XP! +15 Coins!"                  │
└───────────────────────┬─────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────┐
│  UNLOCK CHECK                                           │
│  If milestone reached → New customization available     │
│  Analytics logged via EducationalAnalytics.cs           │
└─────────────────────────────────────────────────────────┘
```

### Emotional Design Patterns

| State    | Mini-Me Expression   | Animation   | Sound         |
| -------- | -------------------- | ----------- | ------------- |
| Happy    | Smiling, bouncing    | Idle_Happy  | Cheerful hum  |
| Hungry   | Looking at food icon | Idle_Hungry | Gentle rumble |
| Tired    | Yawning, droopy eyes | Idle_Sleepy | Soft yawn     |
| Excited  | Stars in eyes        | Celebration | Sparkle SFX   |
| Learning | Focused, book nearby | Study_Loop  | Page turn     |

---

## Optimization Techniques

### Unity-Specific Optimizations

#### 1. Event-Driven Architecture (Already Implemented)

```csharp
// UserManager.cs - Events replace polling
public event Action<UserProfile> OnUserLoggedIn;
public event Action OnUserLoggedOut;
public event Action OnDataSaved;

// GameManager.cs - Coroutine replaces Update() polling
private IEnumerator AutoSaveCoroutine()
{
    while (true)
    {
        yield return new WaitForSeconds(autoSaveInterval);
        if (userManager != null) userManager.SaveIfDirty();
    }
}
```

#### 2. Object Pooling for UI

```csharp
// Recommended: Pool frequently created UI elements
public class UIPool : MonoBehaviour
{
    private Queue<GameObject> rewardPopups = new Queue<GameObject>();

    public GameObject GetRewardPopup()
    {
        if (rewardPopups.Count > 0)
            return rewardPopups.Dequeue();
        return Instantiate(rewardPopupPrefab);
    }

    public void ReturnRewardPopup(GameObject popup)
    {
        popup.SetActive(false);
        rewardPopups.Enqueue(popup);
    }
}
```

#### 3. Async I/O (Already Implemented)

```csharp
// UserManager.cs - Non-blocking saves
private async void SaveAsync()
{
    if (isSaving || !enableDataPersistence) return;
    isSaving = true;
    try
    {
        string json = JsonUtility.ToJson(collection, true);
        await Task.Run(() => File.WriteAllText(saveFilePath, json));
        isDirty = false;
    }
    finally { isSaving = false; }
}
```

#### 4. Throttled Meter Updates

```csharp
// Avoid updating meters every frame
private float meterUpdateInterval = 1f; // Once per second max
private float lastMeterUpdate;

void Update()
{
    if (Time.time - lastMeterUpdate >= meterUpdateInterval)
    {
        UpdateMeters();
        lastMeterUpdate = Time.time;
    }
}
```

#### 5. Sprite Atlasing

```
Assets/
└── Art/
    └── Sprites/
        ├── UI_Atlas.spriteatlas       // All UI elements
        ├── Character_Atlas.spriteatlas // Character parts
        └── Effects_Atlas.spriteatlas   // Particles/VFX
```

### Performance Benchmarks

| Operation       | Target  | Current              |
| --------------- | ------- | -------------------- |
| App Launch      | < 2s    | TBD                  |
| Scene Load      | < 1s    | TBD                  |
| Save Operation  | < 100ms | Async (non-blocking) |
| Animation Blend | 60 FPS  | TBD                  |
| Memory Usage    | < 150MB | TBD                  |

### Battery Optimization (Mobile)

```csharp
// Reduce updates when app backgrounded
void OnApplicationPause(bool paused)
{
    if (paused)
    {
        // Disable non-essential updates
        meterUpdateInterval = 10f;
        DisableParticles();
        SaveAsync();
    }
    else
    {
        // Resume normal operation
        meterUpdateInterval = 1f;
        CalculateOfflineProgress();
        EnableParticles();
    }
}
```

---

## Cultural Considerations

### Thai Cultural Integration

#### Greeting Gestures

| Gesture    | Thai Name | Context             | Animation Reference          |
| ---------- | --------- | ------------------- | ---------------------------- |
| **Wai**    | ไหว้      | Respectful greeting | CharacterController.Wai()    |
| **Curtsy** | การคารวะ  | Formal respect      | CharacterController.Curtsy() |
| **Bow**    | การก้มหัว | Elder respect       | CharacterController.Bow()    |

#### Implementation

```csharp
// CharacterController.cs - Already implemented
public void Wai() => PlayAnimation("Wai");
public void Curtsy() => PlayAnimation("Curtsy");
public void Bow() => PlayAnimation("Bow");

// Reward cultural gesture practice
public void PracticeGesture(GestureType type)
{
    PlayGestureAnimation(type);
    UserProfile.AddHappiness(10);
    UserProfile.AddExperience(5);
    EducationalAnalytics.LogEvent("cultural_gesture", type.ToString());
}
```

#### Educational Context

- Thai wai appropriate for different social contexts
- Respect for teachers and elders built into mechanics
- Traditional clothing options in customization shop
- Thai holidays trigger special events

---

## Implementation Checklist

> 📖 **Synced with [README.md Development Roadmap](../README.md#-development-roadmap)**

### Phase 1: Core Systems ✅ _Complete_

- [x] Unity 6000.2.15f1 project initialization
- [x] User profile persistence (UserManager.cs with async I/O)
- [x] Basic meter system (UserProfile.cs)
- [x] Event-driven architecture (GameManager.cs, delegates)
- [x] Character animation hooks (CharacterController.cs)
- [x] Educational analytics (EducationalAnalytics.cs)
- [x] O(1) user lookup via Dictionary cache

### Phase 2: Engagement Loop ✅ _Complete_

- [x] Daily login bonus system
  - Coins: 5 base + streak bonus (max 10) + milestones
  - Milestones at 3/7/14/30 days
  - NO penalties for missed days ✓
- [x] Streak tracking with reset protection
- [x] Homework completion flow UI
  - Complete homework button triggers rewards
  - Reward display via feedback text
- [x] Celebration animations (PlayDance on milestones)
- [x] Sound effect integration (chimes, sparkles)
- [x] Meter decay implementation
  - Happiness: 0.5/min, floor 20%
  - Hunger: 1.0/min, floor 10%
  - Energy: 0.75/min, floor 15%
- [x] Character care actions (Feed/Rest/Play buttons)

### Phase 3: Character & Customization 🔄 _Current Priority_

- [ ] Eye scaling system
- [ ] Outfit system implementation
  - Attachment points on character mesh
  - Runtime outfit swapping
- [ ] Accessory system (hats, jewelry, items)
- [ ] Shop UI with currency display
- [ ] Rarity tier visual indicators
  - Common (60%), Uncommon (25%), Rare (10%), Epic (4%), Legendary (1%)
- [ ] Unlock notification system
- [ ] Thai gesture animations (Wai, Curtsy, Bow)

### Phase 4: Mini-Games & Universe 📋

- [ ] Minime Universe integration
- [ ] Side game coin/XP rewards
- [ ] Progress sync with main app
- [ ] ScriptableObject resource system
- [ ] Leaderboard (classroom-only, optional)

### Phase 5: Multi-User & Social 📋

- [ ] Password-protected accounts
- [ ] Teacher administrative dashboard
- [ ] Classroom group system
- [ ] Friend Mini-Me visits
- [ ] Gift exchange mechanic
- [ ] Thai gesture social interactions
- [ ] Parent/teacher reporting

### Phase 6: Polish & Deployment 📋

- [ ] Performance profiling (60 FPS target)
- [ ] Battery optimization testing
- [ ] Object pooling implementation
- [ ] Accessibility audit
- [ ] Localization (Thai/English)
- [ ] Android build & testing
- [ ] iOS build & testing

---

## References

- **Tamagotchi** (Wikipedia, 2025) - Core mechanics research
- **Sangsom Mini-Me Design Principles** - copilot-instructions.md
- **Unity Optimization Best Practices** - Unity Documentation
- **Thai Cultural Guidelines** - Project stakeholder input

---

_Document maintained by development team. Last updated: December 2025_
