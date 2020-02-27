#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Created on Wed Feb 26 16:19:20 2020

@author: Hawk Shaw
"""


import dlib
import cv2
from visioncap import *
from pyclient  import *


# Create a client object
client = PyClient(host="127.0.0.1", port=1755)

# Create a new vision-measurement working object
detector  = dlib.get_frontal_face_detector()
predictor = dlib.shape_predictor(sys.argv[1])
worker    = VisionCap(detector, predictor)

# Loop while the camera is opened
while worker.camera.isOpened():

    try:
        # Start measurement for once
        data, rotation_vector, translation_vector, camera_matrix, dist_coeffs = worker.measure()
        
        # Send message to server
        client.send(data, encoding="utf-8")
        
        # Visualization
        worker.visualize(rotation_vector, translation_vector, camera_matrix, dist_coeffs)
    except:
        pass
    
    # Press ESC to quit
    if (cv2.waitKey(5) & 0xFF) == 27: break

worker.camera.release()
cv2.destroyAllWindows()