import numpy as np
import scipy.stats as stats

import numpy as np
import matplotlib.pyplot as plt
from scipy.stats import entropy
from scipy.stats import entropy, skew, kurtosis, iqr, mode
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
