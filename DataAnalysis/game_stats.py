import numpy as np
import math
from scipy.stats import pearsonr
from sklearn.cluster import KMeans

def distance_between_positions(pos1, pos2):
    """
    Calculates the Euclidean distance between two 3D positions.
    This helps analyze movement between frames.
    """
    return math.sqrt((pos2["x"] - pos1["x"])**2 + (pos2["y"] - pos1["y"])**2 + (pos2["z"] - pos1["z"])**2)

def game_duration(game):
    """
    Computes the total duration of the game session.
    """
    return game.end_timestamp - game.start_timestamp

def average_reach_time(game):
    """
    Computes the average time taken to reach the destination.
    """
    times = [action.hand_reached_destination_timestamp for action in game.actions]
    return sum(times) / len(times) if times else 0.0

def reach_time_variance(game):
    """
    Computes the variance of reach times.
    """
    times = [action.hand_reached_destination_timestamp for action in game.actions]
    return np.var(times) if times else 0.0


def reach_time_standard_deviation(game):
    """
    Measures how much reach times deviate from the mean.
    """
    times = [action.hand_reached_destination_timestamp for action in game.actions]
    return np.std(times) if times else 0.0

def reach_time_percentiles(game):
    """
    Computes 25th, 50th (median), and 75th percentiles of reach time.
    """
    times = np.array([action.hand_reached_destination_timestamp for action in game.actions])
    if times.size == 0:
        return {"25th": 0.0, "50th": 0.0, "75th": 0.0}
    return {"25th": np.percentile(times, 25), "50th": np.median(times), "75th": np.percentile(times, 75)}

def hand_velocity_analysis(game):
    """
    Calculates the average velocity (speed) of hands per frame.
    """
    velocities = []
    for action in game.actions:
        frames = action.left_hand_frames + action.right_hand_frames
        for i in range(1, len(frames)):
            distance = distance_between_positions(frames[i - 1].position, frames[i].position)
            time_delta = 1  # Assuming constant frame time
            velocities.append(distance / time_delta)
    return np.mean(velocities) if velocities else 0.0

def hand_acceleration_analysis(game):
    """
    Computes the average acceleration of hand movements.
    Helps measure smoothness and consistency of hand control.
    """
    accelerations = []
    velocities = []
    for action in game.actions:
        frames = action.left_hand_frames + action.right_hand_frames
        for i in range(1, len(frames)):
            distance = distance_between_positions(frames[i - 1].position, frames[i].position)
            time_delta = 1  # Assuming constant frame time
            velocities.append(distance / time_delta)
    
    for i in range(1, len(velocities)):
        accelerations.append((velocities[i] - velocities[i - 1]) / 1)  # Assuming constant time step
    return np.mean(accelerations) if accelerations else 0.0

def moving_average_hand_speed(game, window_size=5):
    """
    Computes the moving average of hand speed.
    Helps smooth out short-term fluctuations and highlight trends.
    """
    speeds = [frame.speed for action in game.actions for frame in action.left_hand_frames + action.right_hand_frames]
    if len(speeds) < window_size:
        return speeds
    return np.convolve(speeds, np.ones(window_size) / window_size, mode='valid')


def reach_time_vs_accuracy_correlation(game):
    """
    Computes correlation between reach time and aim accuracy.
    """
    reach_times = [action.hand_reached_destination_timestamp for action in game.actions]
    accuracies = [action.aim_accuracy for action in game.actions]
    if len(reach_times) < 2 or len(accuracies) < 2:
        return 0.0
    correlation, _ = pearsonr(reach_times, accuracies)
    return correlation

def kmeans_cluster_reach_times(game, clusters=3):
    """
    Uses K-Means clustering to categorize reach times into groups.
    Helps identify player skill levels.
    """
    reach_times = np.array([action.hand_reached_destination_timestamp for action in game.actions]).reshape(-1, 1)
    if reach_times.size < clusters:
        return []
    kmeans = KMeans(n_clusters=clusters, random_state=0, n_init=10).fit(reach_times)
    return list(zip(reach_times.flatten(), kmeans.labels_))

def successful_reaches_ratio(game):
    """
    Calculates the percentage of actions where the player successfully reached the target.
    """
    successful_reaches = sum(1 for action in game.actions if action.hand_reached_destination)
    return successful_reaches / len(game.actions) if game.actions else 0.0

def right_vs_left_hand_usage(game):
    """
    Compares how often the right hand was used versus the left hand.
    Useful for analyzing handedness and dominance.
    """
    right_hand_used = sum(1 for action in game.actions if action.right_hand_reached_destination)
    left_hand_used = sum(1 for action in game.actions if action.left_hand_reached_destination)
    return {"right_hand": right_hand_used, "left_hand": left_hand_used}

def average_distance_traveled_per_hand(game):
    """
    Calculates the total distance traveled by each hand.
    Helps analyze movement patterns.
    """
    left_hand_distance = 0.0
    right_hand_distance = 0.0
    
    for action in game.actions:
        if action.left_hand_frames:
            for i in range(1, len(action.left_hand_frames)):
                left_hand_distance += distance_between_positions(
                    action.left_hand_frames[i - 1].position, action.left_hand_frames[i].position
                )
        
        if action.right_hand_frames:
            for i in range(1, len(action.right_hand_frames)):
                right_hand_distance += distance_between_positions(
                    action.right_hand_frames[i - 1].position, action.right_hand_frames[i].position
                )

    return {"left_hand": left_hand_distance, "right_hand": right_hand_distance}











