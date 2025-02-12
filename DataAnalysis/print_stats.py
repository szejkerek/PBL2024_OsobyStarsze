import numpy as np
import scipy.stats as stats
import pandas as pd
import seaborn as sns
import matplotlib.pyplot as plt
import logging
import numpy as np
import matplotlib.pyplot as plt
from scipy.stats import entropy
from scipy.stats import entropy, skew, kurtosis, iqr, mode


# Configure logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')

def analyze_game_data(games_list):
    """
    Analyzes game data to extract correlations between key performance indicators.
    This function processes game action data, extracts key metrics, and generates statistical plots.

    - Position & Speed Analysis
    - Hand Synchronization & Performance
    - Accuracy vs Score
    - Movement Efficiency vs Score
    - Performance vs Reaction Time
    
    Parameters:
        games_list (list): List of game objects containing player actions and performance data.
    """
    
    logging.info("Starting game data analysis.")
    data = []
    
    # Iterate over each game session
    for game in games_list:
        for action in game.actions:
            
            # Validate frame data availability
            if not action.right_hand_frames or not action.left_hand_frames:
                logging.warning("Skipping action due to missing hand frame data.")
                continue
            
            # Calculate average speed for each hand
            right_hand_speed = sum(frame.speed for frame in action.right_hand_frames) / len(action.right_hand_frames)
            left_hand_speed = sum(frame.speed for frame in action.left_hand_frames) / len(action.left_hand_frames)

            # Calculate average position for each hand
            right_hand_avg_pos = calculate_average_position(action.right_hand_frames)
            left_hand_avg_pos = calculate_average_position(action.left_hand_frames)

            # Collect relevant data points
            data.append({
                "reaction_time": action.reaction_time,
                "hand_movement_time": action.hand_movement_to_object_time,
                "hand_accuracy": action.hand_distance_to_target,
                "right_hand_speed": right_hand_speed,
                "left_hand_speed": left_hand_speed,
                "right_hand_pos_x": right_hand_avg_pos["x"],
                "right_hand_pos_y": right_hand_avg_pos["y"],
                "right_hand_pos_z": right_hand_avg_pos["z"],
                "left_hand_pos_x": left_hand_avg_pos["x"],
                "left_hand_pos_y": left_hand_avg_pos["y"],
                "left_hand_pos_z": left_hand_avg_pos["z"],
                "score": game.overall_score,
                "game_time": game.overall_game_time
            })
    
    if not data:
        logging.error("No valid data collected. Aborting analysis.")
        return
    
    # Convert to DataFrame
    df = pd.DataFrame(data)
    
    # Compute correlation matrix
    corr = df.corr()
    
    # Plot heatmap of correlations
    plot_correlation_matrix(corr)
    
    # Generate scatter plots for specific correlations
    generate_scatter_plots(df)

def calculate_average_position(frames):
    """Calculates the average position (x, y, z) from a list of frames."""
    return {
        "x": sum(frame.position["x"] for frame in frames) / len(frames),
        "y": sum(frame.position["y"] for frame in frames) / len(frames),
        "z": sum(frame.position["z"] for frame in frames) / len(frames),
    }

def plot_correlation_matrix(corr):
    """Plots a heatmap of the correlation matrix for game metrics."""
    plt.figure(figsize=(12, 8))
    sns.heatmap(corr, annot=True, cmap="coolwarm", fmt=".2f", linewidths=0.5)
    plt.title("Correlation Matrix of Game Data")
    plt.show()

def generate_scatter_plots(df):
    """Generates scatter plots with regression lines for key correlations."""
    plot_correlation(df, "reaction_time", "score", "Reaction Time vs Score")
    plot_correlation(df, "hand_movement_time", "score", "Hand Movement Time vs Score")
    plot_correlation(df, "hand_accuracy", "score", "Hand Accuracy vs Score")
    plot_correlation(df, "right_hand_speed", "score", "Right Hand Speed vs Score")
    plot_correlation(df, "left_hand_speed", "score", "Left Hand Speed vs Score")

def plot_correlation(df, x_col, y_col, title):
    """Helper function to plot scatter plots with regression lines."""
    plt.figure(figsize=(8, 6))
    sns.regplot(x=df[x_col], y=df[y_col], scatter_kws={"s": 10}, line_kws={"color": "red"})
    plt.xlabel(x_col.replace("_", " ").title())
    plt.ylabel(y_col.replace("_", " ").title())
    plt.title(title)
    plt.grid(True, linestyle='--', alpha=0.6)
    plt.show()



def kl_divergence(p, q, bin_width=0.05):
    """
    Compute KL divergence between two distributions using histograms with custom bin width.
    """
    min_val = min(np.min(p), np.min(q))
    max_val = max(np.max(p), np.max(q))
    bins = np.arange(min_val, max_val + bin_width, bin_width)
    
    p_hist, _ = np.histogram(p, bins=bins, density=True)
    q_hist, _ = np.histogram(q, bins=bins, density=True)
    
    p_hist = np.where(p_hist == 0, 1e-10, p_hist)
    q_hist = np.where(q_hist == 0, 1e-10, q_hist)
    
    return entropy(p_hist, q_hist)

def compare_hand_speed_distributions(vr_game, kinect_game):
    """
    Compare the hand speed distributions between VR and Kinect using KL divergence.
    Also provides additional statistical metrics and visual representation.
    """
    vr_speeds = np.array([frame.speed for action in vr_game.actions for frame in action.right_hand_frames + action.left_hand_frames])
    kinect_speeds = np.array([frame.speed for action in kinect_game.actions for frame in action.right_hand_frames + action.left_hand_frames])
    
    if len(vr_speeds) == 0 or len(kinect_speeds) == 0:
        print("Insufficient data: One of the datasets is empty.")
        return
    
    kl_speed = kl_divergence(vr_speeds, kinect_speeds)
    
    stats = {
        "VR Mean Speed": (np.mean(vr_speeds), f"VR: {np.mean(vr_speeds):.4f} vs Kinect: {np.mean(kinect_speeds):.4f} - Average hand speed"),
        "VR Median Speed": (np.median(vr_speeds), f"VR: {np.median(vr_speeds):.4f} vs Kinect: {np.median(kinect_speeds):.4f} - Middle value of hand speed distribution"),
        "VR Std Dev": (np.std(vr_speeds), f"VR: {np.std(vr_speeds):.4f} vs Kinect: {np.std(kinect_speeds):.4f} - Standard deviation indicating speed variability"),
        "VR Skewness": (skew(vr_speeds), f"VR: {skew(vr_speeds):.4f} vs Kinect: {skew(kinect_speeds):.4f} - Asymmetry of the speed distribution"),
        "VR Kurtosis": (kurtosis(vr_speeds), f"VR: {kurtosis(vr_speeds):.4f} vs Kinect: {kurtosis(kinect_speeds):.4f} - Peakedness of the speed distribution"),
        "VR IQR": (iqr(vr_speeds), f"VR: {iqr(vr_speeds):.4f} vs Kinect: {iqr(kinect_speeds):.4f} - Range of middle 50% speeds"),
        "KL Divergence": (kl_speed, f"KL Divergence: {kl_speed:.4f} - Measure of difference between distributions")
    }
    
    print("\n=== Hand Speed Analysis ===")
    for key, (value, description) in stats.items():
        print(f"{key}: {description}")
    
    print("\nKL Divergence Interpretation: Higher values indicate greater dissimilarity between VR and Kinect hand speed distributions.\n")
    
    plt.figure(figsize=(10, 5))
    plt.hist(vr_speeds, bins=50, alpha=0.6, color='blue', label='VR Hand Speeds', density=True)
    plt.hist(kinect_speeds, bins=50, alpha=0.6, color='red', label='Kinect Hand Speeds', density=True)
    plt.xlabel('Hand Speed')
    plt.ylabel('Density')
    plt.title('VR vs Kinect Hand Speed Distributions')
    plt.legend()
    plt.grid(True)
    plt.show()




def compare_reaction_time_distributions(vr_game, kinect_game, bin_width=0.05):
    """
    Compare the reaction time distributions between VR and Kinect using KL divergence.
    Uses a customizable bin width for better histogram representation.
    """
    vr_reaction_times = np.array([action.reaction_time for action in vr_game.actions])
    kinect_reaction_times = np.array([action.reaction_time for action in kinect_game.actions])
    
    if len(vr_reaction_times) == 0 or len(kinect_reaction_times) == 0:
        print("Insufficient data: One of the datasets is empty.")
        return
    
    kl_reaction_time = kl_divergence(vr_reaction_times, kinect_reaction_times, bin_width)
    
    stats = {
        "VR Mean Reaction Time": (np.mean(vr_reaction_times), f"VR: {np.mean(vr_reaction_times):.4f} vs Kinect: {np.mean(kinect_reaction_times):.4f} - Average reaction time"),
        "VR Median Reaction Time": (np.median(vr_reaction_times), f"VR: {np.median(vr_reaction_times):.4f} vs Kinect: {np.median(kinect_reaction_times):.4f} - Middle value of reaction time distribution"),
        "VR Std Dev": (np.std(vr_reaction_times), f"VR: {np.std(vr_reaction_times):.4f} vs Kinect: {np.std(kinect_reaction_times):.4f} - Standard deviation indicating variability"),
        "VR Skewness": (skew(vr_reaction_times), f"VR: {skew(vr_reaction_times):.4f} vs Kinect: {skew(kinect_reaction_times):.4f} - Asymmetry of the reaction time distribution"),
        "VR Kurtosis": (kurtosis(vr_reaction_times), f"VR: {kurtosis(vr_reaction_times):.4f} vs Kinect: {kurtosis(kinect_reaction_times):.4f} - Peakedness of the reaction time distribution"),
        "VR IQR": (iqr(vr_reaction_times), f"VR: {iqr(vr_reaction_times):.4f} vs Kinect: {iqr(kinect_reaction_times):.4f} - Range of middle 50% reaction times"),
        "KL Divergence": (kl_reaction_time, f"KL Divergence: {kl_reaction_time:.4f} - Measure of difference between distributions")
    }
    
    print("\n=== Reaction Time Analysis ===")
    for key, (value, description) in stats.items():
        print(f"{key}: {description}")
    
    print("\nKL Divergence Interpretation: Higher values indicate greater dissimilarity in reaction times between VR and Kinect.\n")
    
    min_val = min(np.min(vr_reaction_times), np.min(kinect_reaction_times))
    max_val = max(np.max(vr_reaction_times), np.max(kinect_reaction_times))
    bins = np.arange(min_val, max_val + bin_width, bin_width)
    
    plt.figure(figsize=(10, 5))
    plt.hist(vr_reaction_times, bins=bins, alpha=0.6, color='blue', label='VR Reaction Times', density=True)
    plt.hist(kinect_reaction_times, bins=bins, alpha=0.6, color='red', label='Kinect Reaction Times', density=True)
    plt.xlabel('Reaction Time (seconds)')
    plt.ylabel('Density')
    plt.title(f'VR vs Kinect Reaction Time Distributions (Bin size = {bin_width}s)')
    plt.legend()
    plt.grid(True)
    plt.show()


def compare_hand_distance_to_target(vr_game, kinect_game, bin_width=0.05):
    """
    Compare the accuracy of hand movement (distance to target) between VR and Kinect.
    Uses KL divergence and additional statistics to analyze aiming precision.
    """
    vr_hand_distance = np.array([action.hand_distance_to_target for action in vr_game.actions])
    kinect_hand_distance = np.array([action.hand_distance_to_target for action in kinect_game.actions])
    
    if len(vr_hand_distance) == 0 or len(kinect_hand_distance) == 0:
        print("Insufficient data: One of the datasets is empty.")
        return
    
    kl_hand_distance = kl_divergence(vr_hand_distance, kinect_hand_distance, bin_width)
    
    stats = {
        "VR Mean Distance to Target": (np.mean(vr_hand_distance), f"VR: {np.mean(vr_hand_distance):.4f} vs Kinect: {np.mean(kinect_hand_distance):.4f} - Average distance to target"),
        "VR Median Distance to Target": (np.median(vr_hand_distance), f"VR: {np.median(vr_hand_distance):.4f} vs Kinect: {np.median(kinect_hand_distance):.4f} - Middle value of distance distribution"),
        "VR Std Dev": (np.std(vr_hand_distance), f"VR: {np.std(vr_hand_distance):.4f} vs Kinect: {np.std(kinect_hand_distance):.4f} - Standard deviation indicating variability"),
        "VR Skewness": (skew(vr_hand_distance), f"VR: {skew(vr_hand_distance):.4f} vs Kinect: {skew(kinect_hand_distance):.4f} - Asymmetry of the distance distribution"),
        "VR Kurtosis": (kurtosis(vr_hand_distance), f"VR: {kurtosis(vr_hand_distance):.4f} vs Kinect: {kurtosis(kinect_hand_distance):.4f} - Peakedness of the distance distribution"),
        "VR IQR": (iqr(vr_hand_distance), f"VR: {iqr(vr_hand_distance):.4f} vs Kinect: {iqr(kinect_hand_distance):.4f} - Range of middle 50% distances"),
        "KL Divergence": (kl_hand_distance, f"KL Divergence: {kl_hand_distance:.4f} - Measure of difference between distributions")
    }
    
    print("\n=== Hand Distance to Target Analysis ===")
    for key, (value, description) in stats.items():
        print(f"{key}: {description}")
    
    print("\nKL Divergence Interpretation: Higher values indicate greater dissimilarity in aiming precision between VR and Kinect.\n")
    
    min_val = min(np.min(vr_hand_distance), np.min(kinect_hand_distance))
    max_val = max(np.max(vr_hand_distance), np.max(kinect_hand_distance))
    bins = np.arange(min_val, max_val + bin_width, bin_width)
    
    plt.figure(figsize=(10, 5))
    plt.hist(vr_hand_distance, bins=bins, alpha=0.6, color='blue', label='VR Hand Distance to Target', density=True)
    plt.hist(kinect_hand_distance, bins=bins, alpha=0.6, color='red', label='Kinect Hand Distance to Target', density=True)
    plt.xlabel('Hand Distance to Target')
    plt.ylabel('Density')
    plt.title(f'VR vs Kinect Hand Distance to Target (Bin size = {bin_width})')
    plt.legend()
    plt.grid(True)
    plt.show()


def compare_frame_position_distributions(vr_game, kinect_game, bin_width=0.05):
    """
    Compare the hand position distributions (x, y, z) between VR and Kinect using KL divergence.
    """
    vr_positions = np.array([(frame.position["x"], frame.position["y"], frame.position["z"]) 
                             for action in vr_game.actions 
                             for frame in action.right_hand_frames + action.left_hand_frames])
    
    kinect_positions = np.array([(frame.position["x"], frame.position["y"], frame.position["z"]) 
                                 for action in kinect_game.actions 
                                 for frame in action.right_hand_frames + action.left_hand_frames])
    
    if len(vr_positions) == 0 or len(kinect_positions) == 0:
        print("Frame Position Comparison: Not enough data to compute KL divergence.")
        return
    
    kl_x = kl_divergence(vr_positions[:, 0], kinect_positions[:, 0], bin_width)
    kl_y = kl_divergence(vr_positions[:, 1], kinect_positions[:, 1], bin_width)
    kl_z = kl_divergence(vr_positions[:, 2], kinect_positions[:, 2], bin_width)
    
    stats = {
        "VR Mean Position (X)": (np.mean(vr_positions[:, 0]), f"VR: {np.mean(vr_positions[:, 0]):.4f} vs Kinect: {np.mean(kinect_positions[:, 0]):.4f} - Average position on X-axis"),
        "VR Median Position (X)": (np.median(vr_positions[:, 0]), f"VR: {np.median(vr_positions[:, 0]):.4f} vs Kinect: {np.median(kinect_positions[:, 0]):.4f} - Middle value on X-axis"),
        "VR Std Dev (X)": (np.std(vr_positions[:, 0]), f"VR: {np.std(vr_positions[:, 0]):.4f} vs Kinect: {np.std(kinect_positions[:, 0]):.4f} - Variability on X-axis"),
        "VR Mean Position (Y)": (np.mean(vr_positions[:, 1]), f"VR: {np.mean(vr_positions[:, 1]):.4f} vs Kinect: {np.mean(kinect_positions[:, 1]):.4f} - Average position on Y-axis"),
        "VR Median Position (Y)": (np.median(vr_positions[:, 1]), f"VR: {np.median(vr_positions[:, 1]):.4f} vs Kinect: {np.median(kinect_positions[:, 1]):.4f} - Middle value on Y-axis"),
        "VR Std Dev (Y)": (np.std(vr_positions[:, 1]), f"VR: {np.std(vr_positions[:, 1]):.4f} vs Kinect: {np.std(kinect_positions[:, 1]):.4f} - Variability on Y-axis"),
        "VR Mean Position (Z)": (np.mean(vr_positions[:, 2]), f"VR: {np.mean(vr_positions[:, 2]):.4f} vs Kinect: {np.mean(kinect_positions[:, 2]):.4f} - Average position on Z-axis"),
        "VR Median Position (Z)": (np.median(vr_positions[:, 2]), f"VR: {np.median(vr_positions[:, 2]):.4f} vs Kinect: {np.median(kinect_positions[:, 2]):.4f} - Middle value on Z-axis"),
        "VR Std Dev (Z)": (np.std(vr_positions[:, 2]), f"VR: {np.std(vr_positions[:, 2]):.4f} vs Kinect: {np.std(kinect_positions[:, 2]):.4f} - Variability on Z-axis"),
        "KL Divergence (X)": (kl_x, f"KL X: {kl_x:.4f} - Measure of X-axis distribution difference"),
        "KL Divergence (Y)": (kl_y, f"KL Y: {kl_y:.4f} - Measure of Y-axis distribution difference"),
        "KL Divergence (Z)": (kl_z, f"KL Z: {kl_z:.4f} - Measure of Z-axis distribution difference")
    }
    
    print("\n=== Frame Position Analysis ===")
    for key, (value, description) in stats.items():
        print(f"{key}: {description}")
    
    print("\nKL Divergence Interpretation: Higher values indicate greater dissimilarity in movement range between VR and Kinect.\n")
    
    plt.figure(figsize=(15, 5))
    for i, axis in enumerate(['X', 'Y', 'Z']):
        plt.subplot(1, 3, i + 1)
        plt.hist(vr_positions[:, i], bins=50, alpha=0.6, color='blue', label=f'VR {axis}', density=True)
        plt.hist(kinect_positions[:, i], bins=50, alpha=0.6, color='red', label=f'Kinect {axis}', density=True)
        plt.xlabel(f'{axis} Position')
        plt.ylabel('Density')
        plt.title(f'{axis}-axis Distribution')
        plt.legend()
        plt.grid(True)
    
    plt.tight_layout()
    plt.show()

# Now, the user can call these functions and pass the already loaded VR and Kinect game data.
# Example:
# compare_hand_speed_distributions(vr_game_data, kinect_game_data)
# compare_reaction_time_distributions(vr_game_data, kinect_game_data)
# compare_hand_distance_to_target(vr_game_data, kinect_game_data)
# compare_frame_position_distributions(vr_game_data, kinect_game_data)
