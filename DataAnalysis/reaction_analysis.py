import matplotlib.pyplot as plt
import seaborn as sns

def plot_reaction_time_distribution(actions):
    reaction_times = [action.hand_reached_destination_timestamp for action in actions if action.hand_reached_destination_timestamp > 0]
    
    if not reaction_times:
        print("No valid reaction times above 0 to plot.")
        return

    plt.figure(figsize=(8, 5))
    sns.histplot(reaction_times, bins=20, kde=True)
    plt.xlabel("Reach Time (s)")
    plt.ylabel("Frequency")
    plt.title("Reaction Time Distribution")
    plt.show()

def plot_aim_accuracy(actions):
    aim_accuracies = [action.aim_accuracy for action in actions]
    plt.figure(figsize=(8, 5))
    plt.scatter(range(len(aim_accuracies)), aim_accuracies, alpha=0.6, color="green")
    plt.xlabel("Action Index")
    plt.ylabel("Aim Accuracy")
    plt.title("Aim Accuracy Over Time")
    plt.show()
