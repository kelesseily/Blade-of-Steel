# 🛡️ Blades of Steel – Prototype README

This document outlines the current features, controls, and functionality implemented in the **Blades of Steel** Unity prototype.  
The project demonstrates the foundation of core gameplay systems such as camera control, player movement, interaction mechanics, and environmental dynamics.

---

## 🎮 Player Controls

| **Key / Input** | **Action** |
|------------------|------------|
| `W A S D` | Player Movement (walk / strafe) |
| `Mouse` | Look around (First-Person) / Aim camera (Third-Person) |
| `Right Mouse (Hold)` | In Third-Person: Enables “free-look” mode around the character. Releasing recenters the camera. |
| `V` | Toggle between First-Person and Third-Person camera views |
| `X` | Interact with nearby objects when the interaction prompt appears |

---

## ⚙️ Core Features Implemented

### **1. Dynamic Camera System**
- Players can instantly switch between immersive **First-Person** and tactical **Third-Person** views by pressing `V`.  
- In Third-Person mode:
  - The camera automatically follows the player.
  - Holding the **Right Mouse Button** allows free rotation around the character.
  - Releasing the button snaps the camera back behind the player.

---

### **2. Intelligent Player Movement**
- The movement system adapts to context:
  - **While Moving:** The character automatically faces the movement direction relative to the camera’s orientation.  
  - **While Standing Still:** Pressing `A` or `D` rotates the character in place smoothly, preventing “sliding” movement.  

This creates natural character control, improving both responsiveness and immersion.

---

### **3. Weapon Interaction (Swap System)**
- Weapons placed in the world can be **picked up** or **swapped**.  
- **Pick Up:** Approaching a weapon shows an on-screen prompt (`X` to interact).  
- **Swap:** If the player is already holding a weapon, interacting with another will drop the current one and equip the new one automatically.  

This mechanic lays the groundwork for a more advanced combat system.

---

### **4. Interactive Environment**
- The world includes **interactive torches** and other environmental props.  
- **Torch Interaction:**  
  - When near a torch, an interaction prompt appears.  
  - Press `X` to toggle it on or off.  
  - Each torch has a realistic **flickering flame** effect that also controls its light intensity.  

These interactions give the player a sense of presence and control over the environment.

---

### **5. World Atmosphere**
- **Day/Night Cycle:**  
  - The world lighting changes dynamically over time.  
  - The directional light (sun) rotates to simulate the passage of a full day, altering the mood and brightness.  
- **Background Music:**  
  - Ambient music plays automatically when the game starts.  
  - This helps establish tone and immersion for the medieval setting.  

---

## 🚧 Future Improvements

The prototype provides a solid technical and gameplay foundation. The following improvements are planned for future development:

### **1. Animation Polish**
- Improve synchronization between walking animation and player speed to remove the “ice skating” effect.  
- Add proper **turn-in-place** animations for smoother idle transitions.

### **2. Advanced Weapon System**
- Expand weapon interactions to include:
  - **Light and heavy attack animations**  
  - **Damage detection and hit registration**  
  - **Unique weapon stats** (range, weight, stamina cost, and durability)  

### **3. Additional Features**
- Basic combat AI for enemy dummies or NPC opponents.  
- HUD elements showing stamina, equipped weapon, and interaction prompts.  
- Environmental audio effects (e.g., wind, footsteps, battle ambience).

---

## 🧰 Technical Details

- **Engine:** Unity (URP Template)  
- **Language:** C#  
- **Platform:** PC (Windows)  
- **Scene Name:** `PrototypeScene.unity`  

---

## ▶️ How to Run

1. Clone or download this repository.  
2. Open the project in **Unity (URP Template)** – version 2021 LTS or newer recommended.  
3. In the Project window, open the scene located at:  

