# Python Client Structure

- \_\_main\_\_.py
- models.py
    + *class* **Model**, defines a parameter or function template for different models, such as the coordinates of feature points. CubeHanHan and KizunaAI are both instances of *class* **Model**.
- visioncap.py => *class* **VisionCap**
    + *class* **KalmanFilter**
    + *class* **SmoothFilter**
    + other *functions*
- pyclient.py => *class* **PyClient**, defines functions to connect C# server, send data to C# server, and receive data from C# server.

---

## Examples

### main.py

```Python
import dlib
from visioncap import *
from pyclient  import *


# Create a client object
client = PyClient()

# Create a new vision-measurement working object
detector  = dlib.get_frontal_face_detector()
predictor = dlib.shape_predictor(sys.argv[1])
worker    = VisionCap(detector, predictor)

# Loop while the camera is opened
while worker.camera.isOpened():

    try:
        # Start measurement for once
        data = worker.measure()
        
        # Send message to server
        client.send(data, encoding="utf-8")
        
        # Visualization
        worker.visualize()
    except:
        pass

worker.camera.release()
```

### models.py

```Python
import numpy as np

class Model():

    def __init__(self, feature_points):
        self.feature_points = feature_points

    def foo(self):
        # TODO: do something...
        pass


feature_points = np.array([(   0.0,    0.0,    0.0),      # Nose tip
                           (   0.0, -330.0,  -65.0),      # Chin
                           (-225.0,  170.0, -135.0),      # Left eye left corner
                           ( 225.0,  170.0, -135.0),      # Right eye right corner
                           (-349.0,   85.0, -300.0),      # Left head corner
                           ( 349.0,   85.0, -300.0)])     # Right head corner
KizunaAI = Model(feature_points)
print(KizunaAI.feature_points)
```

### visioncap.py

```Python
from models import *

class VisionCap():

    def __init__(self, detector, predictor, model:Model):
        self.detector  = detector
        self.predictor = predictor
        self.model     = model
        self.camera    = cv2.VideoCapture(0)

    def measure(self):
        pass

    def visualize(self):
        pass
```


### pyclient.py

```Python
import sys
import socket


class PyClient():

    ip_addr = "127.0.0.1"
    ip_port = 1755

    def __init__(self):
        self.client = socket.socket()
        self.connect()
    
    def connect(self):
        try:
            self.client.connect((self.ip_addr, self.ip_port))
            print("ERROR: Socket connected, listening on %s:%d...\n" % (self.ip_addr, self.ip_port))
        except:
            print("ERROR: No socket connection!\n")
            sys.exit(0)

    def send(self, data:str, encoding:str="utf-8"):
        try:
            self.client.send(data.encode(encoding))
        except:
            print("ERROR: Failed to send data to server!\n")


client = PyClient()
client.send("Hello world!")
```
