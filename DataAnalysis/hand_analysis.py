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


def plot_hand_heatmaps(game_data):
    """
    Plots two side-by-side heatmaps (KDE) showing the distribution of 
    (x,z) positions for left and right hands across all actions in 'game_data'.
    
    Parameters:
    -----------
    game_data : GameAnalysisData
        An instance of the Python class that holds overall game data,
        including a list of ActionData, each of which has leftHandFrames 
        and rightHandFrames.
    """
    # Collect all left-hand positions (x, z)
    all_left_x = []
    all_left_z = []

    # Collect all right-hand positions (x, z)
    all_right_x = []
    all_right_z = []

    # Loop through each action and gather frame positions
    for action in game_data.actions:
        for frame in action.left_hand_frames:
            all_left_x.append(frame.position["x"])
            all_left_z.append(frame.position["z"])

        for frame in action.right_hand_frames:
            all_right_x.append(frame.position["x"])
            all_right_z.append(frame.position["z"])

    # Create side-by-side subplots
    fig, axes = plt.subplots(nrows=1, ncols=2, figsize=(12, 6))

    # --- Left Hand Heatmap ---
    sns.kdeplot(
        x=all_left_x, 
        y=all_left_z, 
        cmap="Reds", 
        shade=True, 
        shade_lowest=False, 
        ax=axes[0]
    )
    axes[0].set_title('Left Hand Position Heatmap (X-Z Plane)')
    axes[0].set_xlabel('X Position')
    axes[0].set_ylabel('Z Position')

    # --- Right Hand Heatmap ---
    sns.kdeplot(
        x=all_right_x, 
        y=all_right_z, 
        cmap="Blues", 
        shade=True, 
        shade_lowest=False, 
        ax=axes[1]
    )
    axes[1].set_title('Right Hand Position Heatmap (X-Z Plane)')
    axes[1].set_xlabel('X Position')
    axes[1].set_ylabel('Z Position')

    plt.tight_layout()
    plt.show()

import matplotlib.pyplot as plt
import numpy as np
import pandas as pd
from sklearn.preprocessing import StandardScaler

import numpy as np
import pandas as pd
import matplotlib.pyplot as plt

def analyze_and_plot_hand_distance(game):
    # Extract hand distances from all actions
    hand_distances = [action.hand_distance_to_target for action in game.actions if action.hand_distance_to_target is not None]
    
    if not hand_distances:
        print("No hand distance data available.")
        return
    
    # Convert to NumPy array for calculations
    hand_distances = np.array(hand_distances)
    
    # Compute statistics
    mean_value = np.mean(hand_distances)
    median_value = np.median(hand_distances)
    std_dev = np.std(hand_distances)
    min_value = np.min(hand_distances)
    max_value = np.max(hand_distances)
    
    stats = {
        "Mean": mean_value,
        "Median": median_value,
        "Standard Deviation": std_dev,
        "Min": min_value,
        "Max": max_value
    }
    
    # Display statistics using pandas DataFrame
    stats_df = pd.DataFrame(stats, index=["Value"])
    print(stats_df)
    
    # Plot Histogram with mean and median annotations
    plt.figure(figsize=(10, 5))
    plt.hist(hand_distances, bins=20, alpha=0.7, edgecolor='black', label="Hand Distances")
    plt.axvline(mean_value, color='r', linestyle='dashed', linewidth=2, label=f'Mean: {mean_value:.2f}')
    plt.axvline(median_value, color='g', linestyle='dashed', linewidth=2, label=f'Median: {median_value:.2f}')
    
    plt.xlabel("Hand Distance to Target")
    plt.ylabel("Frequency")
    plt.title("Histogram of Hand Distances to Target")
    plt.legend()
    plt.grid(True)
    plt.show()
    
    # Plot Line Chart with mean reference
    plt.figure(figsize=(10, 5))
    plt.plot(hand_distances, marker='o', linestyle='-', label="Hand Distance")
    plt.axhline(mean_value, color='r', linestyle='dashed', linewidth=2, label=f'Mean: {mean_value:.2f}')
    
    plt.xlabel("Action Index")
    plt.ylabel("Hand Distance to Target")
    plt.title("Hand Distance Over Actions")
    plt.legend()
    plt.grid(True)
    plt.show()
