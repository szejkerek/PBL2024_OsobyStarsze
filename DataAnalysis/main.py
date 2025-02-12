import json
from game_classes import * 
from hand_analysis import *
from reaction_analysis import plot_reaction_time_distribution, plot_aim_accuracy
from performance_comparison import compare_hand_performance
from game_stats import *
from print_stats import *
from newStats import *

file_name = "vr1.json"
with open(file_name, "r") as file:
    json_data = json.load(file)
VR1 = Game(file_name, json_data)

file_name = "vr2.json"
with open(file_name, "r") as file:
    json_data = json.load(file)
VR2 = Game(file_name, json_data)
file_name = "vr3.json"
with open(file_name, "r") as file:
    json_data = json.load(file)
VR3 = Game(file_name, json_data)
file_name = "vr4.json"
with open(file_name, "r") as file:
    json_data = json.load(file)
VR4 = Game(file_name, json_data)




# analyze_game_data([VR1, VR2, VR3, VR4])

# is_ambidextrous(VR)

# plot_hand_stats(VR)
# analyze_hand_to_object_times(VR, 0.1)
# analyze_and_plot_hand_distance(VR)
# visualize_and_calculate_metrics(VR)
# visualize_hand_speeds(VR, rolling_window = 5)

compare_hand_speed_distributions(VR4, VR2)
compare_reaction_time_distributions(VR4, VR2, 0.1)
compare_hand_distance_to_target(VR4, VR2, 0.08)
compare_frame_position_distributions(VR4, VR2, 0.25)