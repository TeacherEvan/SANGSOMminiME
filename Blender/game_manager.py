import bpy
import time
from user_manager import UserManager
from character_controller import CharacterController
from game_configuration import get_game_config

class GameManager:
    _instance = None
    
    @classmethod
    def get_instance(cls):
        if cls._instance is None:
            cls._instance = GameManager()
        return cls._instance
    
    def __init__(self):
        if GameManager._instance is not None:
            raise RuntimeError("GameManager is a singleton! Use get_instance() instead.")
        
        self.game_config = get_game_config()
        self.user_manager = UserManager.get_instance()
        self.character_controller = None
        self.is_running = False
        self.last_update_time = 0
        self.update_interval = 1.0 / self.game_config.target_fps
        
        print("GameManager initialized")

    def start_game(self):
        """Initialize game session."""
        self.is_running = True
        self.last_update_time = time.time()
        
        # Initialize character
        # In a real scenario, we'd load the active user's character
        self.character_controller = CharacterController("Leandi")
        
        # Register update handler
        if self.update not in bpy.app.handlers.frame_change_pre:
            bpy.app.handlers.frame_change_pre.append(self.update)
            
        print("Game started")

    def stop_game(self):
        """Stop game session."""
        self.is_running = False
        
        if self.update in bpy.app.handlers.frame_change_pre:
            bpy.app.handlers.frame_change_pre.remove(self.update)
            
        print("Game stopped")

    def update(self, scene):
        """Main game loop called every frame."""
        if not self.is_running:
            return
            
        current_time = time.time()
        dt = current_time - self.last_update_time
        
        # Logic update (could be throttled if needed)
        if self.character_controller:
            # Example: Decay happiness over time (very slowly)
            # self.character_controller.decrease_happiness(dt * 0.1)
            pass
            
        self.last_update_time = current_time

    def complete_homework(self, assignment_id):
        """Handle homework completion event."""
        if not self.user_manager.current_user:
            print("No user logged in!")
            return
            
        print(f"Completing homework: {assignment_id}")
        
        # 1. Update User Profile
        user = self.user_manager.current_user
        user.add_experience(self.game_config.homework_xp_reward)
        user.add_coins(self.game_config.homework_coin_reward)
        
        # 2. Update Character
        if self.character_controller:
            self.character_controller.increase_happiness(self.game_config.homework_happiness_reward)
            self.character_controller.play_animation("dance") # Celebration
            
        # 3. Save Data
        self.user_manager.save_profiles()

# Global accessor
def get_game_manager():
    return GameManager.get_instance()
