import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D

def get_hand_positions(actions, hand):
    positions = []
    directions = []
    speeds = []
    for action in actions:
        frames = action.right_hand_frames if hand == "right" else action.left_hand_frames
        for frame in frames:
            pos = frame.position
            dir_vec = frame.direction
            speed = frame.speed
            positions.append((pos["x"], pos["y"], pos["z"]))
            directions.append((dir_vec["x"], dir_vec["y"], dir_vec["z"]))
            speeds.append(speed)
    return positions, directions, speeds

def plot_hand_heatmap(actions, hand):
    positions, directions, speeds = get_hand_positions(actions, hand)
    if not positions:
        print(f"No {hand} hand positions available.")
        return
    
    x_vals, y_vals, z_vals = zip(*positions)
    u_vals, v_vals, w_vals = zip(*directions)
    
    fig = plt.figure()
    ax = fig.add_subplot(111, projection='3d')
    ax.scatter(x_vals, y_vals, z_vals, c=z_vals, cmap='hot', marker='o')
    
    for i in range(len(x_vals)):
        ax.quiver(
            x_vals[i], y_vals[i], z_vals[i],
            u_vals[i] * speeds[i], v_vals[i] * speeds[i], w_vals[i] * speeds[i],
            color='blue', length=0.1, normalize=True
        )
    
    ax.set_xlabel('X Position')
    ax.set_ylabel('Y Position')
    ax.set_zlabel('Z Position')
    ax.set_title(f'{hand.capitalize()} Hand Position Heatmap')
    
    plt.show()
