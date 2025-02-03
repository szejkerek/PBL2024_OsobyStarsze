import pandas as pd
import matplotlib.pyplot as plt
import numpy as np
from mpl_toolkits.mplot3d import Axes3D
import seaborn as sns

def extract_hand_data(actions, hand):
    frames = []
    for action in actions:
        hand_frames = action.right_hand_frames if hand == "right" else action.left_hand_frames
        for frame in hand_frames:
            frames.append({
                "x": frame.position["x"],
                "y": frame.position["y"],
                "z": frame.position["z"],
                "speed": frame.speed
            })
    return pd.DataFrame(frames)

def plot_speed_over_time(actions):
    right_data = extract_hand_data(actions, "right")
    left_data = extract_hand_data(actions, "left")
    
    plt.figure(figsize=(10, 5))
    plt.plot(right_data.index, right_data["speed"], label="Right Hand", color="blue")
    plt.plot(left_data.index, left_data["speed"], label="Left Hand", color="red")
    plt.xlabel("Frame Index")
    plt.ylabel("Speed")
    plt.title("Hand Speed Over Time")
    plt.legend()
    plt.show()

def plot_hand_movement_3d(actions, hand):
    data = extract_hand_data(actions, hand)
    
    fig = plt.figure(figsize=(8, 6))
    ax = fig.add_subplot(111, projection='3d')
    ax.scatter(data["x"], data["y"], data["z"], c=data.index, cmap='viridis', alpha=0.6)
    
    ax.set_xlabel("X Position")
    ax.set_ylabel("Y Position")
    ax.set_zlabel("Z Position")
    ax.set_title(f"3D Movement of {hand.capitalize()} Hand")
    
    plt.show()


import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D

def get_hand_positions(actions, hand):
    positions = []
    directions = []
    speeds = []
    for action in actions:
        frames = action.right_hand_frames if hand == "right" else action.left_hand_frames
        for frame in frames:
            pos = frame.position
            dir_vec = frame.direction
            speed = frame.speed
            positions.append((pos["x"], pos["y"], pos["z"]))
            directions.append((dir_vec["x"], dir_vec["y"], dir_vec["z"]))
            speeds.append(speed)
    return positions, directions, speeds

def plot_hand_heatmap(actions, hand):
    positions, directions, speeds = get_hand_positions(actions, hand)
    if not positions:
        print(f"No {hand} hand positions available.")
        return
    
    x_vals, y_vals, z_vals = zip(*positions)
    u_vals, v_vals, w_vals = zip(*directions)
    
    fig = plt.figure()
    ax = fig.add_subplot(111, projection='3d')
    ax.scatter(x_vals, y_vals, z_vals, c=z_vals, cmap='hot', marker='o')
    
    for i in range(len(x_vals)):
        ax.quiver(
            x_vals[i], y_vals[i], z_vals[i],
            u_vals[i] * speeds[i], v_vals[i] * speeds[i], w_vals[i] * speeds[i],
            color='blue', length=0.1, normalize=True
        )
    
    ax.set_xlabel('X Position')
    ax.set_ylabel('Y Position')
    ax.set_zlabel('Z Position')
    ax.set_title(f'{hand.capitalize()} Hand Position Heatmap')
    
    plt.show()


