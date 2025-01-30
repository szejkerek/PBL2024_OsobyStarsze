import json
import matplotlib.pyplot as plt
import numpy as np

# Wczytaj dane z pliku JSON
with open('data.json', 'r') as file:
    data = json.load(file)

# Wyodrębnij prędkości dla prawej i lewej ręki
def calculate_total_speed(speeds):
    return [s['x']+ s['y'] + s['z'] for s in speeds]

right_hand_speeds = []
left_hand_speeds = []

for action in data['actions']:
    right_hand_speeds.extend(calculate_total_speed(action['rightHandSpeeds']))
    left_hand_speeds.extend(calculate_total_speed(action['leftHandSpeeds']))

# Przygotuj dane do osi czasu
time_points = range(len(right_hand_speeds))

# Tworzenie wykresu
plt.figure(figsize=(10, 6))
plt.plot(time_points, right_hand_speeds, label='Prawej ręki', color='blue', linestyle='-')
plt.plot(time_points, left_hand_speeds, label='Lewej ręki', color='red', linestyle='--')

# Dodanie etykiet i tytułu
plt.title('Prędkość całkowita ruchu ręki w czasie', fontsize=14)
plt.xlabel('Punkt czasu', fontsize=12)
plt.ylabel('Prędkość całkowita', fontsize=12)
plt.legend(fontsize=12)
plt.grid(True, linestyle='--', alpha=0.6)

# Wyświetlenie wykresu
plt.show()
