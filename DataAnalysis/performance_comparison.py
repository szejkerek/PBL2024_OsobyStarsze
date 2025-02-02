import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns
import numpy as np

import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns

def compare_hand_performance(actions):
    data = {
        "Hand": [],
        "Speed": [],
        "Reach Time": []
    }

    for action in actions:
        if action.hand_reached_destination_timestamp > 0:
            data["Hand"].append("Right")
            data["Speed"].append(np.mean([f.speed for f in action.right_hand_frames]))
            data["Reach Time"].append(action.hand_reached_destination_timestamp)

            data["Hand"].append("Left")
            data["Speed"].append(np.mean([f.speed for f in action.left_hand_frames]))
            data["Reach Time"].append(action.hand_reached_destination_timestamp)

    if not data["Hand"]:  # Check if there's any valid data to plot
        print("No valid reach times above 0 to plot.")
        return

    df = pd.DataFrame(data)

    plt.figure(figsize=(10, 5))
    sns.boxplot(x="Hand", y="Speed", data=df)
    plt.title("Comparison of Hand Speed")
    plt.show()

    plt.figure(figsize=(10, 5))
    sns.boxplot(x="Hand", y="Reach Time", data=df)
    plt.title("Comparison of Hand Reach Time")
    plt.show()

