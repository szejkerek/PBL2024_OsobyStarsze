import json

with open('data.json', 'r') as file:
    data = json.load(file)

print("Overall Game Time:", data['overallGameTime'])
print("Overall Score:", data['overallScore'])

for index, action in enumerate(data['actions']):
    print(f"\nAction {index + 1}:")
    print("Hand Movement To Object Time:", action['handMovementToObjectTime'])
    print("Reaction Time:", action['reactionTime'])
    print("Aim Accuracy:", action['aimAccuracy'])
    print("Right Hand Reached Destination:", action['rightHandReachedDestination'])
    print("Left Hand Reached Destination:", action['leftHandReachedDestination'])
    print("Right Hand Positions:", action['rightHandPositions'])
    print("Right Hand Speeds:", action['rightHandSpeeds'])
    print("Left Hand Positions:", action['leftHandPositions'])
    print("Left Hand Speeds:", action['leftHandSpeeds'])
