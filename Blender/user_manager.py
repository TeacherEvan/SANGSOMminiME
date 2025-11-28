"""
Sangsom Mini-Me - User Profile Manager for Blender

This module handles user profile management, data persistence,
and authentication within the Blender environment.

Usage:
    from user_manager import UserManager
    
    manager = UserManager()
    user = manager.create_user("student1", "Happy Student")
    manager.login_user("student1")
"""

import bpy
import json
import os
from typing import Optional, List, Dict
from dataclasses import dataclass, field, asdict
from datetime import datetime
from pathlib import Path
import uuid


@dataclass
class UserProfile:
    """
    Represents a user profile in the Sangsom Mini-Me system.
    
    Attributes:
        user_id: Unique identifier for the user
        username: Login username
        display_name: User's display name
        created_date: Account creation date
        experience_points: Total XP earned
        coins: Current coin balance
        is_active: Whether account is active
        eye_scale: Character eye customization
        current_outfit: Current outfit selection
        current_accessory: Current accessory selection
        homework_completed: Number of homework items completed
        days_active: Number of days the user has been active
        character_happiness: Current character happiness (0-100)
    """
    user_id: str = field(default_factory=lambda: str(uuid.uuid4()))
    username: str = ""
    display_name: str = ""
    created_date: str = field(default_factory=lambda: datetime.now().isoformat())
    experience_points: int = 0
    coins: int = 100  # Starting coins
    is_active: bool = True
    
    # Character customization
    eye_scale: float = 1.0
    current_outfit: str = "default"
    current_accessory: str = "none"
    
    # Progress tracking
    homework_completed: int = 0
    days_active: int = 1
    character_happiness: float = 75.0  # Starting happiness
    
    def add_experience(self, amount: int):
        """Add experience points."""
        self.experience_points += amount
    
    def add_coins(self, amount: int):
        """Add coins to balance."""
        self.coins += amount
    
    def spend_coins(self, amount: int) -> bool:
        """
        Attempt to spend coins.
        
        Returns:
            True if successful, False if insufficient balance
        """
        if self.coins >= amount:
            self.coins -= amount
            return True
        return False
    
    def complete_homework(self):
        """Handle homework completion with rewards."""
        self.homework_completed += 1
        self.add_experience(10)
        self.add_coins(5)
        self.increase_happiness(5.0)
    
    def increase_happiness(self, amount: float):
        """Increase happiness (clamped to 0-100)."""
        self.character_happiness = min(100.0, max(0.0, self.character_happiness + amount))
    
    def decrease_happiness(self, amount: float):
        """Decrease happiness (clamped to 0-100)."""
        self.character_happiness = min(100.0, max(0.0, self.character_happiness - amount))
    
    def set_eye_scale(self, scale: float):
        """Set eye scale (clamped to 0.5-2.0)."""
        self.eye_scale = min(2.0, max(0.5, scale))
    
    def set_outfit(self, outfit: str):
        """Set current outfit."""
        self.current_outfit = outfit
    
    def set_accessory(self, accessory: str):
        """Set current accessory."""
        self.current_accessory = accessory
    
    def get_level(self) -> int:
        """Calculate level from experience points."""
        return self.experience_points // 100 + 1
    
    def to_dict(self) -> Dict:
        """Convert to dictionary for JSON serialization."""
        return asdict(self)
    
    @classmethod
    def from_dict(cls, data: Dict) -> 'UserProfile':
        """Create from dictionary."""
        return cls(**data)


class UserManager:
    """
    Manages user profiles and authentication for the Sangsom Mini-Me system.
    
    This class handles:
    - User creation and deletion
    - Login/logout functionality
    - Data persistence to JSON
    - User lookup and listing
    
    Attributes:
        save_file: Path to the user profiles save file
        current_user: Currently logged in user (or None)
        users: List of all user profiles
    """
    
    _instance: Optional['UserManager'] = None
    
    def __new__(cls):
        """Singleton pattern implementation."""
        if cls._instance is None:
            cls._instance = super().__new__(cls)
            cls._instance._initialized = False
        return cls._instance
    
    def __init__(self):
        """Initialize the user manager."""
        if self._initialized:
            return
        
        self._initialized = True
        self.users: List[UserProfile] = []
        self.current_user: Optional[UserProfile] = None
        
        # Set up save file path
        self.save_file = self._get_save_path()
        
        # Load existing profiles
        self.load_profiles()
        
        print(f"UserManager initialized. Found {len(self.users)} user profiles.")
    
    @staticmethod
    def _get_save_path() -> Path:
        """Get the path for the save file."""
        # Try Blender's user data directory first
        if hasattr(bpy, 'app') and hasattr(bpy.app, 'tempdir'):
            # Use a more persistent location
            user_dir = Path(bpy.utils.user_resource('SCRIPTS'))
            save_dir = user_dir / "sangsom_minime"
        else:
            # Fallback to current directory
            save_dir = Path.cwd() / "data"
        
        save_dir.mkdir(parents=True, exist_ok=True)
        return save_dir / "user_profiles.json"
    
    @classmethod
    def get_instance(cls) -> 'UserManager':
        """Get or create the singleton instance."""
        if cls._instance is None:
            cls._instance = cls()
        return cls._instance
    
    def create_user(self, username: str, display_name: str) -> Optional[UserProfile]:
        """
        Create a new user profile.
        
        Args:
            username: Unique username for login
            display_name: Display name for the user
            
        Returns:
            The created UserProfile, or None if username exists
        """
        # Check if username exists
        if self.get_user_by_name(username):
            print(f"Username '{username}' already exists!")
            return None
        
        # Create new profile
        user = UserProfile(
            username=username,
            display_name=display_name
        )
        
        self.users.append(user)
        self.save_profiles()
        
        print(f"Created new user: {display_name} ({username})")
        return user
    
    def login_user(self, username: str) -> bool:
        """
        Log in a user by username.
        
        Args:
            username: Username to log in
            
        Returns:
            True if login successful, False otherwise
        """
        user = self.get_user_by_name(username)
        
        if user and user.is_active:
            self.current_user = user
            print(f"User logged in: {user.display_name}")
            return True
        
        print(f"Login failed for username: {username}")
        return False
    
    def logout_user(self):
        """Log out the current user."""
        if self.current_user:
            print(f"User logged out: {self.current_user.display_name}")
            self.current_user = None
    
    def get_user_by_name(self, username: str) -> Optional[UserProfile]:
        """Find a user by username (case-insensitive)."""
        for user in self.users:
            if user.username.lower() == username.lower():
                return user
        return None
    
    def get_user_by_id(self, user_id: str) -> Optional[UserProfile]:
        """Find a user by their unique ID."""
        for user in self.users:
            if user.user_id == user_id:
                return user
        return None
    
    def delete_user(self, username: str) -> bool:
        """
        Delete a user by username.
        
        Args:
            username: Username to delete
            
        Returns:
            True if deleted, False if not found
        """
        user = self.get_user_by_name(username)
        if user:
            self.users.remove(user)
            if self.current_user == user:
                self.logout_user()
            self.save_profiles()
            print(f"Deleted user: {username}")
            return True
        return False
    
    def save_profiles(self):
        """Save all user profiles to file."""
        try:
            data = {
                "profiles": [user.to_dict() for user in self.users]
            }
            
            with open(self.save_file, 'w', encoding='utf-8') as f:
                json.dump(data, f, indent=2, ensure_ascii=False)
            
            print(f"User profiles saved to: {self.save_file}")
        except Exception as e:
            print(f"Error saving profiles: {e}")
    
    def load_profiles(self):
        """Load user profiles from file."""
        try:
            if self.save_file.exists():
                with open(self.save_file, 'r', encoding='utf-8') as f:
                    data = json.load(f)
                
                self.users = [
                    UserProfile.from_dict(profile_data)
                    for profile_data in data.get("profiles", [])
                ]
                
                print(f"Loaded {len(self.users)} user profiles from: {self.save_file}")
            else:
                print("No saved profiles found. Starting fresh.")
                self.users = []
        except Exception as e:
            print(f"Error loading profiles: {e}")
            self.users = []
    
    def save_current_user(self):
        """Save the current user's data."""
        if self.current_user:
            self.save_profiles()
    
    def get_all_users(self) -> List[UserProfile]:
        """Get a copy of all user profiles."""
        return list(self.users)
    
    def __str__(self) -> str:
        current = self.current_user.display_name if self.current_user else "None"
        return f"UserManager(users={len(self.users)}, current={current})"


# Convenience function to get the singleton instance
def get_user_manager() -> UserManager:
    """Get the UserManager singleton instance."""
    return UserManager.get_instance()


# Example usage when run directly
if __name__ == "__main__":
    print("Testing UserManager...")
    
    manager = get_user_manager()
    print(manager)
    
    # Create test user if none exist
    if len(manager.users) == 0:
        user = manager.create_user("test_student", "Test Student")
        if user:
            user.add_coins(100)
            user.complete_homework()
            manager.login_user("test_student")
            print(f"Created and logged in test user")
            print(f"User status: Level {user.get_level()}, {user.coins} coins, {user.character_happiness}% happiness")
