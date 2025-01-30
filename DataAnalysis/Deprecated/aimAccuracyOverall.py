import json
import os
import matplotlib.pyplot as plt
import numpy as np

# Zdefiniuj ścieżkę do folderu
folder_path = "DataVR"

# Lista do przechowywania wartości aimAccuracy
all_accuracies = []

# Iteracja po plikach w folderze
for filename in os.listdir(folder_path):
    if filename.endswith('.json'):
        file_path = os.path.join(folder_path, filename)
        with open(file_path, 'r') as f:
            data = json.load(f)
            for action in data['actions']:
                all_accuracies.append(action['aimAccuracy'])

# Tworzenie histogramu
plt.hist(all_accuracies, bins=20)
plt.xlabel('Dokładność celowania (%)')
plt.ylabel('Liczba wystąpień')
plt.title('Histogram dokładności celowania')
plt.show()