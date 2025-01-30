class Frame:
    def __init__(self, data):
        self.position = data.get("position", {"x": 0.0, "y": 0.0, "z": 0.0})
        self.rotation = data.get("rotation", {"x": 0.0, "y": 0.0, "z": 0.0, "w": 1.0})
        self.rotation_euler = data.get("rotationEuler", {"x": 0.0, "y": 0.0, "z": 0.0})
        self.direction = data.get("direction", {"x": 0.0, "y": 0.0, "z": 0.0})
        self.speed = data.get("speed", 0.0)

class Action:
    def __init__(self, data):
        self.hand_movement_to_object_time = data.get("handMovementToObjectTime", 0.0)
        self.reaction_time = data.get("reactionTime", 0.0)
        self.hand_reached_destination_timestamp = data.get("handReachedDestinationTimestamp", 0.0)
        self.hand_reached_destination = data.get("handReachedDestination", False)
        self.right_hand_reached_destination = data.get("rightHandReachedDestination", False)
        self.left_hand_reached_destination = data.get("leftHandReachedDestination", False)
        self.aim_accuracy = data.get("aimAccuracy", 0.0)
        self.left_hand_frames = [Frame(frame) for frame in data.get("leftHandFrames", [])]
        self.right_hand_frames = [Frame(frame) for frame in data.get("rightHandFrames", [])]

class Game:
    def __init__(self, file_name, data):
        self.file_name = file_name
        self.overall_game_time = data.get("overallGameTime", 0.0)
        self.overall_score = data.get("overallScore", 0.0)
        self.app_type = data.get("appType", "Unknown")
        self.start_timestamp = data.get("startTimestamp", 0.0)
        self.end_timestamp = data.get("endTimestamp", 0.0)
        self.actions = [Action(action) for action in data.get("actions", [])]
