import json
import os
import matplotlib.pyplot as plt
import numpy as np

# Zdefiniuj ścieżkę do folderu
folder_path = "DataVR"

# Listy do przechowywania danych
all_game_times = []
all_scores = []

# Iteracja po plikach w folderze
for filename in os.listdir(folder_path):
    if filename.endswith('.json'):
        file_path = os.path.join(folder_path, filename)
        with open(file_path, 'r') as f:
            data = json.load(f)
            all_game_times.append(data['overallGameTime'])
            all_scores.append(data['overallScore'])

# Tworzenie histogramu
fig, ax = plt.subplots()
ax.hist([all_game_times, all_scores], bins=20, label=['overallGameTime', 'overallScore'])
ax.set_xlabel('Wartość')
ax.set_ylabel('Liczba wystąpień')
ax.set_title('Histogram dla overallGameTime i overallScore')
ax.legend()
plt.show()