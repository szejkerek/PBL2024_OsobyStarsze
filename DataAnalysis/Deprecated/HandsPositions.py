import json
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D

# Wczytaj dane z pliku JSON
with open('data.json', 'r') as file:
    data = json.load(file)

# Wyodrębnij pozycje dla prawej i lewej ręki
def extract_positions(positions):
    return [(p['x'], p['y'], p['z']) for p in positions]

right_hand_positions = []
left_hand_positions = []

for action in data['actions']:
    right_hand_positions.extend(extract_positions(action['rightHandPositions']))
    left_hand_positions.extend(extract_positions(action['leftHandPositions']))

# Przygotowanie danych do wykresu
right_hand_positions = list(zip(*right_hand_positions))  # Rozdzielenie na osie x, y, z
left_hand_positions = list(zip(*left_hand_positions))

# Tworzenie wykresu 3D
fig = plt.figure(figsize=(12, 8))
ax = fig.add_subplot(111, projection='3d')

# Wykres dla prawej ręki
ax.plot(right_hand_positions[0], right_hand_positions[1], right_hand_positions[2], 
        label='Prawej ręki', color='blue', linestyle='-')

# Wykres dla lewej ręki
ax.plot(left_hand_positions[0], left_hand_positions[1], left_hand_positions[2], 
        label='Lewej ręki', color='red', linestyle='--')

# Dodanie etykiet i tytułu
ax.set_title('Ścieżka ruchu rąk w przestrzeni 3D', fontsize=14)
ax.set_xlabel('X', fontsize=12)
ax.set_ylabel('Y', fontsize=12)
ax.set_zlabel('Z', fontsize=12)
ax.legend(fontsize=12)

# Wyświetlenie wykresu
plt.show()
