import json
from game_classes import Game 
from hand_analysis import plot_speed_over_time, plot_hand_movement_3d, plot_hand_heatmap
from reaction_analysis import plot_reaction_time_distribution, plot_aim_accuracy
from performance_comparison import compare_hand_performance

file_name = "data.json"
with open(file_name, "r") as file:
    json_data = json.load(file)

game = Game(file_name, json_data)

# Hand heatmaps
plot_hand_heatmap(game.actions, "right")
plot_hand_heatmap(game.actions, "left")

# Hand analysis
plot_speed_over_time(game.actions)
plot_hand_movement_3d(game.actions, "right")
plot_hand_movement_3d(game.actions, "left")

# Reaction and accuracy analysis
plot_reaction_time_distribution(game.actions)
plot_aim_accuracy(game.actions)

# Performance comparison
compare_hand_performance(game.actions)
