import json
import matplotlib.pyplot as plt
import numpy as np

# Wczytaj dane z pliku JSON
with open('data.json', 'r') as file:
    data = json.load(file)
    

# Obliczenie średniej dokładności
total_accuracy = 0
for action in data['actions']:
    total_accuracy += action['aimAccuracy']
average_accuracy = total_accuracy / len(data['actions'])

# Obliczenie liczby sukcesów dla lewej i prawej ręki
left_successes = sum(action['leftHandReachedDestination'] for action in data['actions'])
right_successes = sum(action['rightHandReachedDestination'] for action in data['actions'])
total_actions = len(data['actions'])

# Tworzenie wykresów
fig, (ax1, ax2) = plt.subplots(1, 2)

# Wykres 1: Średnia dokładność
colors = ['green' if average_accuracy >= 80 else 'red', 'red']  # Dostosuj próg dokładności
ax1.pie([average_accuracy, 100 - average_accuracy], labels=['Średnia dokładność', 'Błąd'], autopct='%1.1f%%', colors=colors)
ax1.set_title('Średnia dokładność celowania')

# Wykres 2: Sukcesy dla lewej i prawej ręki
colors = ['red', 'blue']
ax2.pie([left_successes, right_successes], labels=['Lewa ręka', 'Prawa ręka'], autopct='%1.1f%%', colors=colors)
ax2.set_title('Sukcesy w osiągnięciu celu')

plt.show()