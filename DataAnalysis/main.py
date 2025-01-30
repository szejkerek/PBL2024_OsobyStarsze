import json
from hands_heatmaps import plot_hand_heatmap
from game_classes import Game 

file_name = "data.json"
with open(file_name, "r") as file:
    json_data = json.load(file)

game = Game(file_name, json_data)

plot_hand_heatmap(game.actions, "right")
plot_hand_heatmap(game.actions, "left")