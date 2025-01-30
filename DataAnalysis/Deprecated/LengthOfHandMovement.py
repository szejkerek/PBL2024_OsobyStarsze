import json
import matplotlib.pyplot as plt
from math import sqrt

# Wczytaj dane z pliku JSON
with open('data.json', 'r') as file:
    data = json.load(file)

# Funkcja do obliczenia odległości między dwoma punktami
def calculate_distance(p1, p2):
    return sqrt((p2['x'] - p1['x'])**2 + (p2['y'] - p1['y'])**2 + (p2['z'] - p1['z'])**2)

# Funkcja do obliczenia odległości między kolejnymi punktami
def calculate_segment_distances(positions):
    distances = []
    for i in range(1, len(positions)):
        distances.append(calculate_distance(positions[i - 1], positions[i]))
    return distances

# Wyodrębnij pozycje dla prawej i lewej ręki
right_hand_positions = []
left_hand_positions = []

for action in data['actions']:
    right_hand_positions.extend(action['rightHandPositions'])
    left_hand_positions.extend(action['leftHandPositions'])

# Oblicz odległości między kolejnymi punktami dla prawej i lewej ręki
right_hand_distances = calculate_segment_distances(right_hand_positions)
left_hand_distances = calculate_segment_distances(left_hand_positions)

# Oblicz całkowite odległości
right_hand_total_distance = sum(right_hand_distances)
left_hand_total_distance = sum(left_hand_distances)

# Przygotowanie danych do wykresu
steps = range(len(right_hand_distances))

# Tworzenie wykresu 
plt.figure(figsize=(12, 6))
plt.plot(steps, right_hand_distances, label='Prawa ręka', color='blue')
plt.plot(steps, left_hand_distances, label='Lewa ręka', color='red')

# Dodanie etykiet i tytułu
plt.title('Odległości przebyte przez ręce pomiędzy odczytami', fontsize=14)
plt.xlabel('Kolejne odczyty', fontsize=12)
plt.ylabel('Odległość (jednostki)', fontsize=12)
plt.xticks(steps, [str(int(x)) for x in steps])
plt.legend(fontsize=12)
plt.grid(True)

# Dodanie informacji o całkowitej drodze
props = dict(boxstyle='round', facecolor='white', alpha=0.5)
info_text = f'Całkowita droga:\nPrawa ręka: {right_hand_total_distance:.2f}\nLewa ręka: {left_hand_total_distance:.2f}'
# Pozycja tekstu zależna od szerokości wykresu
text_position_x = 1.02 - (0.2) * 0.5
plt.gca().text(text_position_x, 0.5, info_text, transform=plt.gca().transAxes, fontsize=12, bbox=props)

# Wyświetlenie wykresu
plt.show()
