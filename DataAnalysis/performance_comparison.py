import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns
import numpy as np

def compare_hand_performance(actions):
    data = {
        "Hand": [],
        "Speed": [],
        "Reaction Time": []
    }

    for action in actions:
        data["Hand"].append("Right")
        data["Speed"].append(np.mean([f.speed for f in action.right_hand_frames]))
        data["Reaction Time"].append(action.reaction_time)

        data["Hand"].append("Left")
        data["Speed"].append(np.mean([f.speed for f in action.left_hand_frames]))
        data["Reaction Time"].append(action.reaction_time)

    df = pd.DataFrame(data)
    
    plt.figure(figsize=(10, 5))
    sns.boxplot(x="Hand", y="Speed", data=df)
    plt.title("Comparison of Hand Speed")
    plt.show()

    plt.figure(figsize=(10, 5))
    sns.boxplot(x="Hand", y="Reaction Time", data=df)
    plt.title("Comparison of Reaction Time")
    plt.show()
