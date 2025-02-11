import json
from game_classes import * 
from hand_analysis import *
from reaction_analysis import plot_reaction_time_distribution, plot_aim_accuracy
from performance_comparison import compare_hand_performance
from game_stats import *
from print_stats import *
from newStats import *

file_name = "dataNewBig.json"
with open(file_name, "r") as file:
    json_data = json.load(file)
5
VR = Game(file_name, json_data)

file_name = "fakeKinect.json"
with open(file_name, "r") as file:
    json_data = json.load(file)

FakeKinect = Game(file_name, json_data)


is_ambidextrous(VR)

plot_hand_stats(VR)
analyze_hand_to_object_times(VR, 0.1)
analyze_and_plot_hand_distance(VR)
visualize_and_calculate_metrics(VR)
visualize_hand_speeds(VR, rolling_window = 5)

compare_hand_speed_distributions(VR, FakeKinect)
compare_reaction_time_distributions(VR, FakeKinect, 0.1)
compare_hand_distance_to_target(VR, FakeKinect, 0.08)
compare_frame_position_distributions(VR, FakeKinect, 0.25)