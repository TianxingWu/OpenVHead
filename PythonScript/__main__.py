#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Created on Fri Sep 27 16:42:52 2019

@author: James Wu
"""


import dlib
import cv2
from visioncap import *
from pyclient  import *


# Create a client object
# client = PyClient()

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
        # client.send(data, encoding="utf-8")
        
        # Visualization
        worker.visualize(rotation_vector, translation_vector, camera_matrix, dist_coeffs)
    except:
        pass
    
    # Press ESC to quit
    if (cv2.waitKey(5) & 0xFF) == 27: break
    

# Socket disconnect
worker.camera.release()
cv2.destroyAllWindows()