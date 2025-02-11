import matplotlib.pyplot as plt
import numpy as np
import pandas as pd
from sklearn.preprocessing import StandardScaler

import numpy as np
import pandas as pd
import matplotlib.pyplot as plt

import pandas as pd
import matplotlib.pyplot as plt
import numpy as np
from mpl_toolkits.mplot3d import Axes3D
import seaborn as sns

import matplotlib.pyplot as plt
import seaborn as sns
import numpy as np
from mpl_toolkits.mplot3d import Axes3D


import matplotlib.pyplot as plt
import numpy as np

import numpy as np
import matplotlib.pyplot as plt
import seaborn as sns
import pandas as pd

import matplotlib.pyplot as plt
import pandas as pd

import matplotlib.pyplot as plt
import pandas as pd

import matplotlib.pyplot as plt
import pandas as pd

import matplotlib.pyplot as plt
import pandas as pd

import matplotlib.pyplot as plt
import pandas as pd
import seaborn as sns
import numpy as np

import matplotlib.pyplot as plt
import pandas as pd
import seaborn as sns
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns

def is_ambidextrous(game, speed_threshold=0.1, usage_threshold=0.1, position_threshold=0.1):
    """
    Determines if the user is ambidextrous based on hand speed, usage frequency, and hand position in 3D space.

    Parameters:
    - game: An instance of the Game class containing actions.
    - speed_threshold: Threshold for the difference in hand speeds (default: 0.1).
    - usage_threshold: Threshold for the difference in hand usage frequency (default: 0.1).
    - position_threshold: Threshold for the difference in hand positions in 3D space (default: 0.1).

    Returns:
    - True if the user is ambidextrous, False otherwise.
    """
    
    # Extract hand speed data
    right_speeds = []
    left_speeds = []
    
    for action in game.actions:
        for frame in action.right_hand_frames:
            right_speeds.append(frame.speed)
        for frame in action.left_hand_frames:
            left_speeds.append(frame.speed)
    
    # Calculate mean speed for each hand
    mean_right_speed = np.mean(right_speeds) if right_speeds else 0
    mean_left_speed = np.mean(left_speeds) if left_speeds else 0
    
    # Calculate speed difference
    speed_difference = abs(mean_right_speed - mean_left_speed) / max(mean_right_speed, mean_left_speed) if max(mean_right_speed, mean_left_speed) != 0 else 0
    
    # Calculate hand usage frequency
    right_usage = len(right_speeds)
    left_usage = len(left_speeds)
    total_usage = right_usage + left_usage
    usage_difference = abs(right_usage - left_usage) / total_usage if total_usage != 0 else 0
    
    # Calculate hand position differences in 3D space
    right_positions = []
    left_positions = []
    
    for action in game.actions:
        for frame in action.right_hand_frames:
            right_positions.append([frame.position["x"], frame.position["y"], frame.position["z"]])
        for frame in action.left_hand_frames:
            left_positions.append([frame.position["x"], frame.position["y"], frame.position["z"]])
    
    mean_right_position = np.mean(right_positions, axis=0) if right_positions else [0, 0, 0]
    mean_left_position = np.mean(left_positions, axis=0) if left_positions else [0, 0, 0]
    
    position_difference = np.linalg.norm(np.array(mean_right_position) - np.array(mean_left_position)) / np.linalg.norm(np.array(mean_right_position)) if np.linalg.norm(np.array(mean_right_position)) != 0 else 0
    
    # Determine if the user is ambidextrous based on thresholds
    if (speed_difference < speed_threshold and 
        usage_difference < usage_threshold and 
        position_difference < position_threshold):
        print("True")
        return True
    else:
        print("False")
        return False

def visualize_hand_speeds(game, rolling_window=5):
    """
    Extracts hand speed data, visualizes speed over action index and frame index,
    and provides statistical metrics with enhanced plots.
    """
    # Extract hand speed data
    data = []
    actions = game.actions[1:-1]  # Skip first and last actions
    for action_idx, action in enumerate(actions, start=1):
        for frame_idx, frame in enumerate(action.right_hand_frames):
            data.append({
                "action_index": action_idx,
                "frame_index": frame_idx,
                "hand": "Right",
                "speed": frame.speed
            })
        for frame_idx, frame in enumerate(action.left_hand_frames):
            data.append({
                "action_index": action_idx,
                "frame_index": frame_idx,
                "hand": "Left",
                "speed": frame.speed
            })
    
    df = pd.DataFrame(data)
    if df.empty:
        print("No hand speed data available.")
        return
    
    # Compute statistics
    stats = df.groupby("hand")["speed"].describe()
    stats["median"] = df.groupby("hand")["speed"].median()
    stats["std"] = df.groupby("hand")["speed"].std()
    stats["90th_percentile"] = df.groupby("hand")["speed"].quantile(0.90)
    print("Hand Speed Statistics:")
    print(stats)
    
    # Sort by action index and frame index for visualization
    df.sort_values(by=["action_index", "frame_index"], inplace=True)
    
    # Apply rolling mean for smoothing
    df["rolling_speed"] = df.groupby("hand")["speed"].transform(lambda x: x.rolling(rolling_window, min_periods=1).mean())
    
    # Create visualizations
    fig, axes = plt.subplots(2, 2, figsize=(16, 10))
    
    # Speed over action index plot
    sns.lineplot(data=df, x="action_index", y="rolling_speed", hue="hand", ax=axes[0, 0], linewidth=2)
    axes[0, 0].set_title("Hand Speed Over Action Index (Smoothed)")
    axes[0, 0].set_xlabel("Action Index")
    axes[0, 0].set_ylabel("Speed")
    axes[0, 0].grid(True)
    
    # Speed scatter plot over frame index
    sns.scatterplot(data=df, x="frame_index", y="speed", hue="hand", ax=axes[0, 1], alpha=0.6)
    axes[0, 1].set_title("Hand Speed per Frame")
    axes[0, 1].set_xlabel("Frame Index")
    axes[0, 1].set_ylabel("Speed")
    
    # Box plot for hand speeds
    sns.boxplot(data=df, x="hand", y="speed", ax=axes[1, 0], showfliers=False)
    axes[1, 0].set_title("Box Plot of Hand Speeds")
    axes[1, 0].set_xlabel("Hand")
    axes[1, 0].set_ylabel("Speed")
    
    # KDE plot for distribution analysis
    sns.kdeplot(data=df, x="speed", hue="hand", ax=axes[1, 1], fill=True, alpha=0.3)
    axes[1, 1].set_title("KDE Plot of Hand Speed")
    axes[1, 1].set_xlabel("Speed")
    axes[1, 1].set_ylabel("Density")
    
    plt.tight_layout()
    plt.show()

def visualize_and_calculate_metrics(game):
    """
    Visualizes and calculates statistics for boolean metrics in a Game object.

    Parameters:
    - game (Game): An instance of the Game class containing actions.
    """
    
    # Extract relevant action data into a DataFrame
    data_list = [{
        "goodTargetFound": action.good_target_found,
        "handReachedDestination": action.hand_reached_destination,
        "rightHandReachedDestination": action.right_hand_reached_destination,
        "leftHandReachedDestination": action.left_hand_reached_destination,
        "goodTargetRightHand": action.good_target_found and action.right_hand_reached_destination,
        "goodTargetLeftHand": action.good_target_found and action.left_hand_reached_destination,
        "badTargetRightHand": (not action.good_target_found) and action.right_hand_reached_destination,
        "badTargetLeftHand": (not action.good_target_found) and action.left_hand_reached_destination
    } for action in game.actions]

    # Convert extracted data to a DataFrame
    df = pd.DataFrame(data_list)

    # Define the relevant columns
    base_columns = ["goodTargetFound", "handReachedDestination", "rightHandReachedDestination", "leftHandReachedDestination"]
    target_columns = ["goodTargetRightHand", "goodTargetLeftHand", "badTargetRightHand", "badTargetLeftHand"]

    # Ensure all columns exist, filling missing values with False
    for col in base_columns + target_columns:
        df[col] = df.get(col, False)

    # Count occurrences of True values for each metric
    base_counts = df[base_columns].sum()
    target_counts = df[target_columns].sum()
    total_records = len(df) if len(df) > 0 else 1  # Prevent division by zero

    # Calculate percentages
    base_percentages = (base_counts / total_records) * 100
    target_percentages = (target_counts / total_records) * 100

    # Create DataFrames for statistics
    stats_base_df = pd.DataFrame({
        "Count": base_counts,
        "Percentage (%)": base_percentages.round(2)
    })

    stats_target_df = pd.DataFrame({
        "Count": target_counts,
        "Percentage (%)": target_percentages.round(2)
    })

    # Create figure and subplots
    fig, axes = plt.subplots(1, 3, figsize=(18, 5))

    # Bar Chart - General Counts
    axes[0].bar(stats_base_df.index, stats_base_df["Count"], color='skyblue', edgecolor='black')
    axes[0].set_xlabel("Metrics")
    axes[0].set_ylabel("Count")
    axes[0].set_title("Overall Metric Counts")
    axes[0].set_xticklabels(stats_base_df.index, rotation=25)
    axes[0].grid(axis='y', linestyle='--', alpha=0.7)

    # Add values on top of bars
    for i, v in enumerate(stats_base_df["Count"]):
        axes[0].text(i, v + 0.5, str(v), ha='center', fontsize=10, fontweight='bold')

    # Horizontal Bar Chart - Percentage
    axes[1].barh(stats_base_df.index, stats_base_df["Percentage (%)"], color='lightcoral', edgecolor='black')
    axes[1].set_xlabel("Percentage (%)")
    axes[1].set_ylabel("Metrics")
    axes[1].set_title("Metric Success Rate (%)")
    axes[1].grid(axis='x', linestyle='--', alpha=0.7)

    # Add values next to bars
    for i, v in enumerate(stats_base_df["Percentage (%)"]):
        axes[1].text(v + 1, i, f"{v:.1f}%", va='center', fontsize=10, fontweight='bold')

    # Stacked Bar Chart - Good vs Bad Targets for Hands
    target_categories = stats_target_df.index
    axes[2].bar(target_categories, stats_target_df["Count"], color=['green', 'blue', 'red', 'orange'], edgecolor='black')
    axes[2].set_xlabel("Target Types")
    axes[2].set_ylabel("Count")
    axes[2].set_title("Good vs. Bad Target Hits by Hands")
    axes[2].set_xticklabels(target_categories, rotation=25)
    axes[2].grid(axis='y', linestyle='--', alpha=0.7)

    # Add values on top of bars
    for i, v in enumerate(stats_target_df["Count"]):
        axes[2].text(i, v + 0.5, str(v), ha='center', fontsize=10, fontweight='bold')

    # Adjust layout
    plt.tight_layout()

    # Show plots
    plt.show()

    # Print the statistics
    print("\nGeneral Metrics:")
    print(stats_base_df)
    print("\nGood vs. Bad Target Hits:")
    print(stats_target_df)

    return stats_base_df, stats_target_df

def analyze_hand_to_object_times(game):
    # Extract hand movement times, filtering only values >= 0
    hand_movement_times = np.array([action.hand_movement_to_object_time for action in game.actions if action.hand_movement_to_object_time >= 0])

    if hand_movement_times.size == 0:
        print("No valid hand movement times found.")
        return

    # Compute statistics
    stats = {
        "Mean": np.mean(hand_movement_times),
        "Median": np.median(hand_movement_times),
        "Min": np.min(hand_movement_times),
        "Max": np.max(hand_movement_times),
        "Standard Deviation": np.std(hand_movement_times),
    }

    # Print statistics
    print("\nHand Movement Time Statistics:")
    for key, value in stats.items():
        print(f"{key}: {value:.2f} seconds")

    # Create plots
    fig, axes = plt.subplots(2, 2, figsize=(14, 10))

    # Histogram & Boxplot Combined
    ax1 = axes[0, 0]
    sns.histplot(hand_movement_times, bins=20, kde=False, ax=ax1, color="skyblue", edgecolor="black")
    sns.boxplot(x=hand_movement_times, ax=ax1, color="orange", width=0.2, boxprops={'alpha': 0.6})
    ax1.axvline(stats["Mean"], color="red", linestyle="--", label=f"Mean: {stats['Mean']:.2f}s")
    ax1.axvline(stats["Median"], color="blue", linestyle="-.", label=f"Median: {stats['Median']:.2f}s")
    ax1.set_title("Distribution of Hand Movement Times (Histogram & Boxplot)")
    ax1.set_xlabel("Time (seconds)")
    ax1.set_ylabel("Frequency")
    ax1.legend()

    # Density Plot (KDE)
    ax2 = axes[0, 1]
    sns.kdeplot(hand_movement_times, ax=ax2, color="blue", fill=True, alpha=0.6)
    ax2.axvline(stats["Mean"], color="red", linestyle="--", label=f"Mean: {stats['Mean']:.2f}s")
    ax2.axvline(stats["Median"], color="blue", linestyle="-.", label=f"Median: {stats['Median']:.2f}s")
    ax2.set_title("Density of Hand Movement Times (KDE)")
    ax2.set_xlabel("Time (seconds)")
    ax2.legend()

    # Cumulative Distribution Function (CDF)
    ax3 = axes[1, 0]
    sorted_data = np.sort(hand_movement_times)
    cdf = np.arange(len(sorted_data)) / float(len(sorted_data))
    ax3.plot(sorted_data, cdf, marker=".", linestyle="none", label="CDF", color="black")
    ax3.axvline(stats["Mean"], color="red", linestyle="--", label=f"Mean: {stats['Mean']:.2f}s")
    ax3.set_title("Cumulative Distribution Function (CDF)")
    ax3.set_xlabel("Time (seconds)")
    ax3.set_ylabel("Probability")
    ax3.legend()

    # Scatter Plot with Mean Line
    ax4 = axes[1, 1]
    ax4.scatter(range(len(hand_movement_times)), hand_movement_times, alpha=0.6, label="Data Points", color="blue")
    ax4.axhline(stats["Mean"], color="red", linestyle="--", label=f"Mean: {stats['Mean']:.2f}s")
    ax4.set_title("Scatter Plot of Time Taken to Reach for an Object")
    ax4.set_xlabel("Sample Index")
    ax4.set_ylabel("Time (seconds)")
    ax4.legend()

    plt.tight_layout()
    plt.show()




def plot_hand_stats(game_data):
    """
    Plots various visualizations of (x, y, z) hand positions, including:
    - KDE heatmaps (x, z plane)
    - 3D scatter plots
    - Movement trajectory plots
    - Box plots (distribution along each axis)
    
    Parameters:
    -----------
    game_data : GameAnalysisData
        An instance of the Python class that holds overall game data,
        including a list of ActionData, each of which has leftHandFrames 
        and rightHandFrames.
    """
    all_left_x, all_left_y, all_left_z = [], [], []
    all_right_x, all_right_y, all_right_z = [], [], []
    
    # Collect positions
    for action in game_data.actions:
        for frame in action.left_hand_frames:
            all_left_x.append(frame.position["x"])
            all_left_y.append(frame.position["y"])
            all_left_z.append(frame.position["z"])
        
        for frame in action.right_hand_frames:
            all_right_x.append(frame.position["x"])
            all_right_y.append(frame.position["y"])
            all_right_z.append(frame.position["z"])
    
    fig, axes = plt.subplots(2, 3, figsize=(18, 12))
    
    # KDE Heatmaps (X-Z plane)
    sns.kdeplot(x=all_left_x, y=all_left_z, cmap="Reds", shade=True, ax=axes[0, 0])
    axes[0, 0].set_title("Left Hand KDE (X-Z Plane)")
    axes[0,0].set_xlabel("X Position")
    axes[0,0].set_ylabel("Z Position")
    
    sns.kdeplot(x=all_right_x, y=all_right_z, cmap="Blues", shade=True, ax=axes[0, 1])
    axes[0, 1].set_title("Right Hand KDE (X-Z Plane)")
    axes[0,1].set_xlabel("X Position")
    axes[0,1].set_ylabel("Z Position")
    
    # 3D Scatter Plot
    ax_3d = fig.add_subplot(2, 3, 3, projection='3d')
    ax_3d.scatter(all_left_x, all_left_y, all_left_z, c='red', label='Left Hand', alpha=0.6)
    ax_3d.scatter(all_right_x, all_right_y, all_right_z, c='blue', label='Right Hand', alpha=0.6)
    ax_3d.set_title("3D Scatter Plot of Hand Positions")
    ax_3d.set_xlabel("X Position")
    ax_3d.set_ylabel("Y Position")
    ax_3d.set_zlabel("Z Position")
    ax_3d.legend()
    
    # Movement Trajectories (X-Z)
    axes[1, 0].plot(all_left_x, all_left_z, 'r-', alpha=0.5, label='Left Hand')
    axes[1, 0].plot(all_right_x, all_right_z, 'b-', alpha=0.5, label='Right Hand')
    axes[1, 0].set_title("Hand Movement Trajectories (X-Z)")
    axes[1, 0].set_xlabel("X Position")
    axes[1, 0].set_ylabel("Z Position")
    axes[1, 0].legend()
    
    # Box Plots for X, Y, Z
    sns.boxplot(data=[all_left_x, all_right_x], ax=axes[1, 1])
    axes[1, 1].set_title("Box Plot of X Positions")
    axes[1, 1].set_ylabel("X Position")
    axes[1, 1].set_xticklabels(["Left Hand", "Right Hand"])
    
    sns.boxplot(data=[all_left_y, all_right_y], ax=axes[1, 2])
    axes[1, 2].set_title("Box Plot of Y Positions")
    axes[1, 2].set_ylabel("Y Position")
    axes[1, 2].set_xticklabels(["Left Hand", "Right Hand"]) 
    
    plt.tight_layout()
    plt.show()

def detect_fatigue(hand_distances, window_size=10, threshold=0.2, min_stable_windows=3):
    """
    Wykrywa momenty zmęczenia na podstawie ruchu ręki.
    :param hand_distances: Lista odległości ręki do celu.
    :param window_size: Rozmiar okna do obliczania średniej i wariancji.
    :param threshold: Próg zmienności. Jeśli `None`, wyliczamy jako 10 percentyl wariancji.
    :param min_stable_windows: Liczba kolejnych "stabilnych" okien potrzebnych do oznaczenia zmęczenia.
    :return: Lista indeksów, gdzie zaczyna się zmęczenie.
    """
    if len(hand_distances) < window_size:
        return []
    
    rolling_stds = []
    fatigue_indices = []

    for i in range(len(hand_distances) - window_size + 1):
        window = hand_distances[i:i + window_size]
        rolling_stds.append(np.std(window))

    rolling_stds = np.array(rolling_stds)
    
    if threshold is None:
        threshold = np.percentile(rolling_stds, 10)  # Dynamiczny próg (10% najniższych wartości)

    stable_count = 0
    for i in range(1, len(rolling_stds)):
        if rolling_stds[i] < threshold:
            stable_count += 1
        else:
            stable_count = 0

        if stable_count >= min_stable_windows:
            fatigue_indices.append(i + window_size - 1)

    #print("Wykryte momenty zmęczenia:", fatigue_indices)  # Debugowanie
    return fatigue_indices

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
    q1 = np.percentile(hand_distances, 25)
    q3 = np.percentile(hand_distances, 75)
    iqr = q3 - q1
    
    stats = {
        "Mean": mean_value,
        "Median": median_value,
        "Standard Deviation": std_dev,
        "Min": min_value,
        "Max": max_value,
        "Q1": q1,
        "Q3": q3,
        "IQR": iqr
    }
    
    # Display statistics using pandas DataFrame
    stats_df = pd.DataFrame(stats, index=["Value"])
    print(stats_df)
    
    fatigue_indices = detect_fatigue(hand_distances)
    
    # Plot multiple visualizations in a single figure
    fig, axes = plt.subplots(2, 3, figsize=(12, 10))
    
    # Histogram
    axes[0, 0].hist(hand_distances, bins=20, alpha=0.7, edgecolor='black', label="Hand Distances")
    axes[0, 0].axvline(mean_value, color='r', linestyle='dashed', linewidth=2, label=f'Mean: {mean_value:.2f}')
    axes[0, 0].axvline(median_value, color='g', linestyle='dashed', linewidth=2, label=f'Median: {median_value:.2f}')
    axes[0, 0].set_xlabel("Hand Distance to Target")
    axes[0, 0].set_ylabel("Frequency")
    axes[0, 0].set_title("Histogram of Hand Distances")
    axes[0, 0].legend()
    axes[0, 0].grid(True)
    
    # Line Chart
    axes[0, 1].plot(hand_distances, marker='o', linestyle='-', label="Hand Distance")
    axes[0, 1].axhline(mean_value, color='r', linestyle='dashed', linewidth=2, label=f'Mean: {mean_value:.2f}')
    axes[0, 1].set_xlabel("Action Index")
    axes[0, 1].set_ylabel("Hand Distance to Target")
    axes[0, 1].set_title("Hand Distance Over Actions")
    axes[0, 1].legend()
    axes[0, 1].grid(True)
    
    # Box Plot
    sns.boxplot(y=hand_distances, ax=axes[1, 0], color='skyblue')
    axes[1, 0].set_title("Boxplot of Hand Distances")
    axes[1, 0].set_ylabel("Hand Distance to Target")
    axes[1, 0].set_xlabel("Hand Distance Distribution")
    
    # Violin Plot
    sns.violinplot(y=hand_distances, ax=axes[1, 1], color='lightcoral')
    axes[1, 1].set_title("Violin Plot of Hand Distances")
    axes[1, 1].set_ylabel("Hand Distance to Target")
    axes[1, 1].set_xlabel("Hand Distance Distribution")
    
    # Line Chart (z oznaczeniem zmęczenia)
    axes[0, 2].plot(hand_distances, marker='o', linestyle='-', label="Hand Distance")
    axes[0, 2].axhline(mean_value, color='r', linestyle='dashed', linewidth=2, label=f'Mean: {mean_value:.2f}')
    
    # Oznaczenie momentów zmęczenia na wykresie
    for idx in fatigue_indices:
        axes[0, 2].axvline(idx, color='purple', linestyle='dotted', linewidth=1, label="Fatigue Detected" if idx == fatigue_indices[0] else "")

    axes[0, 2].set_xlabel("Action Index")
    axes[0, 2].set_ylabel("Hand Distance to Target")
    axes[0, 2].set_title("Hand Distance Over Actions with Fatigue Detection")
    axes[0, 2].legend()
    axes[0, 2].grid(True)
    
    plt.tight_layout()
    plt.show()