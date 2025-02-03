import json
from game_classes import * 
from hand_analysis import *
from reaction_analysis import plot_reaction_time_distribution, plot_aim_accuracy
from performance_comparison import compare_hand_performance
from game_stats import *
from newStats import *

file_name = "data.json"
with open(file_name, "r") as file:
    json_data = json.load(file)

game = Game(file_name, json_data)


# # # Hand heatmaps
plot_hand_stats(game)
analyze_and_plot_hand_distance(game)
analyze_hand_to_object_times(game)
visualize_and_calculate_metrics(game)
visualize_hand_speeds(game, rolling_window = 5)

# # # Hand analysis
# # plot_speed_over_time(game.actions)
# # plot_hand_movement_3d(game.actions, "right")
# # plot_hand_movement_3d(game.actions, "left")

# # # Reaction and accuracy analysis
# # plot_reaction_time_distribution(game.actions)
# plot_aim_accuracy(game.actions)

# # Performance comparison
# compare_hand_performance(game.actions)


# results = {
#     "Game Duration": game_duration(game),
#     "Average Reaction Time": average_reach_time(game),
#     "Reaction Time Std Dev": reach_time_standard_deviation(game),
#     "Reaction Time Percentiles": reach_time_percentiles(game),
#     "Hand Velocity (Avg)": hand_velocity_analysis(game),
#     "Hand Acceleration (Avg)": hand_acceleration_analysis(game),
#     "Reaction Time Clusters": kmeans_cluster_reach_times(game),
#     "Successful Reaches (%)": successful_reaches_ratio(game),
#     "Hand Usage (%)": right_vs_left_hand_usage(game),
#     "Distance Traveled per Hand": average_distance_traveled_per_hand(game),
# }


# # Handle warnings
# warnings = []

# if len(results["Reaction Time Clusters"]) < 3:
#     warnings.append("⚠️ Warning: Clustering found fewer groups than expected (insufficient unique data).")

# # Pretty Print Results
# print("\n📊 GAME STATISTICS 📊")
# print("=" * 40)
# for key, value in results.items():
#     print(f"{key:35} | {value}")
# print("=" * 40)

# # Display warnings if any
# if warnings:
#     print("\n⚠️ WARNINGS ⚠️")
#     for warning in warnings:
#         print(warning)