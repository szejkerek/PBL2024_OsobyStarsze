import json
import matplotlib.pyplot as plt

# Wczytanie danych z pliku JSON
with open('data.json', 'r') as file:
    data = json.load(file)

# Ekstrakcja danych
times = []
for action in data['actions']:
    times.append({
        'handMovementToObjectTime': action['handMovementToObjectTime'],
        'reactionTime': action['reactionTime'],
        'handReachedDestinationTimestamp': action['handReachedDestinationTimestamp']
    })

# Przygotowanie danych do wykresu
x = range(len(times))
y1 = [t['handMovementToObjectTime'] for t in times]
y2 = [t['reactionTime'] for t in times]
y3 = [t['handReachedDestinationTimestamp'] for t in times]

# Tworzenie wykresu
plt.figure(figsize=(10, 6))
plt.plot(x, y1, label='Czas ruchu ręki do obiektu')
plt.plot(x, y2, label='Czas reakcji')
plt.plot(x, y3, label='Czas dotarcia ręki do celu')

# Dodanie tytułu i legendy
plt.title('Wykres czasów akcji', fontsize=14)
plt.xlabel('Numer akcji' ,fontsize=12)
plt.ylabel('Czas [s]' ,fontsize=12)
plt.legend(loc='upper right', bbox_to_anchor=(1.0, 1.0), ncol=1, frameon=True)
plt.grid(True, linestyle='--', alpha=0.6)

# Wyświetlenie wykresu
plt.show()