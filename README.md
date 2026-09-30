# Helicopter Flight Simulation (Prototype)

Physics-based helicopter prototype built in Unity 6 (HDRP) with the Input System.

## Controls

| Key | Action |
|-----|--------|
| **W / S** | Pitch forward / backward (nose down / up) |
| **A / D** | Roll left / right |
| **Space** | Raise collective (more lift) |
| **Left Ctrl** | Lower collective (less lift) |
| **Q / E** | Yaw left / right (tail rotor) |
| **R** | Engine on / off |

## How to fly

1. Wait for the rotor to spool up (about 5 s).
2. Hold Space to raise the collective. The helicopter lifts off once thrust exceeds weight.
3. The collective stays where you leave it, like a real lever. To hover, use Space/Ctrl to keep it steady.
4. Use W/A/S/D to tilt the helicopter. Tilt produces horizontal acceleration. Release to level out.
5. To land, lower the collective with Ctrl and descend gently.

## Requirements

Unity 6000.4.8f1, HDRP, Input System package, Cinemachine.