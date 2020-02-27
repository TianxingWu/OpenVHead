#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Created on Fri Sep 27 16:42:52 2019

@author: James Wu
"""


import cv2
import numpy as np
import time
import math
import sys


class KalmanObj:

    XX = 0.
    PP = 0.01

    def __init__(self, m, Qval, Rval):
        self.K = np.zeros((m,m))
        self.xx = np.zeros(m)
        self.P = np.eye(m)
        self.F = np.eye(m)
        self.B = np.eye(m)
        self.H = np.eye(m)
        self.Q = Qval * np.eye(m)
        self.R = Rval * np.eye(m)


    def update(self, uu, zz):
        self.xx = self.F.dot(self.xx) + self.B.dot(uu)
        self.P = self.F.dot(self.P).dot(self.F.T) + self.Q
        self.K = self.P.dot(self.H.T).dot( np.linalg.inv(self.H.dot(self.P).dot(self.H.T) + self.R) )
        self.xx = self.xx + self.K.dot(zz - self.H.dot(self.xx))
        self.P = self.P - self.K.dot(self.H).dot(self.P)


    def kalman_filter_simple(self, data, Q, R):
        K = self.PP / (self.PP + R)
        self.XX = self.XX + K * (data - self.XX)
        self.PP = self.PP - K * self.PP + Q
        return self.XX


class VisionCap:

    POINTS_NUM_LANDMARK = 68

    boxPoints3D = np.array(([ 500.,  500.,  500.],
                            [-500.,  500.,  500.],
                            [-500., -500.,  500.],
                            [ 500., -500.,  500.],
                            [ 500.,  500., -500.],
                            [-500.,  500., -500.],
                            [-500., -500., -500.],
                            [ 500., -500., -500.]))
    boxPoints2D = np.zeros((1,1,8,2))

    # model_points = np.array([(   0.0,    0.0,    0.0),      # Nose tip
    #                          (   0.0, -330.0,  -65.0),      # Chin
    #                          (-225.0,  170.0, -135.0),      # Left eye left corner
    #                          ( 225.0,  170.0, -135.0),      # Right eye right corner
    #                          (-150.0, -150.0, -125.0),      # Left Mouth corner
    #                          ( 150.0, -150.0, -125.0)])     # Right mouth corner
    model_points = np.array([(   0.0,    0.0,    0.0),      # Nose tip
                             (   0.0, -330.0,  -65.0),      # Chin
                             (-225.0,  170.0, -135.0),      # Left eye left corner
                             ( 225.0,  170.0, -135.0),      # Right eye right corner
                             (-349.0,   85.0, -300.0),      # Left head corner
                             ( 349.0,   85.0, -300.0)])     # Right head corner

    # Parameters for mean filter
    windowlen_1 = 5
    queue3D_points = np.zeros((windowlen_1,POINTS_NUM_LANDMARK,2))
    windowlen_2 = 5
    queue1D = np.zeros(windowlen_2)

    
    # ==================================================================
    # Initialization
    #   - detector: 
    #   - predictor: 
    # ==================================================================
    def __init__(self, detector, predictor):
        self.detector  = detector
        self.predictor = predictor

        # Open the camera
        self.camera    = cv2.VideoCapture(0)

        # CLAHE Object (for Adaptive histogram equalization)
        self.clahe = cv2.createCLAHE(clipLimit=2.0, tileGridSize=(8,8)) 

        # Initialize Kalman object: Tune Q, R to change landmarks_x / landmarks_y sensitivity
        self.KalmanX = KalmanObj(self.POINTS_NUM_LANDMARK,1,10)
        self.KalmanY = KalmanObj(self.POINTS_NUM_LANDMARK,1,10)
        self.uu_     = np.zeros((self.POINTS_NUM_LANDMARK))

        # Initialize PARAMETERS
        self.landmarks = np.zeros((self.POINTS_NUM_LANDMARK,2))


    # ==================================================================
    # Smooth filter
    #   - queue3D_points:
    #   - landmarks_orig: 
    # ==================================================================
    def mean_filter_for_landmarks(self, queue3D_points, landmarks_orig):
        for i in range(self.windowlen_1 - 1):
            queue3D_points[i,:,:] = queue3D_points[i+1,:,:]
        queue3D_points[self.windowlen_1-1,:,:] = landmarks_orig
        return queue3D_points.mean(axis=0)


    # ==================================================================
    # Smooth filter simple
    #   - queue1D:
    #   - data: 
    # ==================================================================
    def mean_filter_simple(self, queue1D, data):
        for i in range(self.windowlen_2 - 1):
            queue1D[i] = queue1D[i+1]
        queue1D[self.windowlen_2-1] = data
        return queue1D.mean()


    # ==================================================================
    # Format convert 
    #   - landmarks:
    #   - dtype: 
    # ==================================================================  
    def landmarks_to_np(self, landmarks, dtype="int"):
        # Get number of landmarks
        num = landmarks.num_parts
        
        # Initialize the list of (x,y) coordinates
        coords = np.zeros((num, 2), dtype=dtype)
        
        # Loop over the 68 facial landmarks and convert them to a 2-tuple of (x,y) coordinates
        for i in range(num):
            coords[i] = (landmarks.part(i).x, landmarks.part(i).y)
        
        # return the list of (x,y) coordinates
        return coords


    # ==================================================================
    # Get feature_parameters of facial expressions
    #   - landmarks:
    # ==================================================================  
    def get_feature_parameters(self, landmarks):
        # Length of face (eyebrow to chin)
        d00 = np.linalg.norm(landmarks[27] - landmarks[8]) 

        # Width of face
        d11 = np.linalg.norm(landmarks[0] - landmarks[16]) 
        d_reference = (d00 + d11)/2

        # Left eye
        d1  = np.linalg.norm(landmarks[37] - landmarks[41])
        d2  = np.linalg.norm(landmarks[38] - landmarks[40])

        # Right eye
        d3  = np.linalg.norm(landmarks[43] - landmarks[47])
        d4  = np.linalg.norm(landmarks[44] - landmarks[46])

        # Mouth width
        d5  = np.linalg.norm(landmarks[51] - landmarks[57])

        # Mouth length
        d6  = np.linalg.norm(landmarks[60] - landmarks[64])
        
        leftEyeWid  = ((d1 + d2)/(2*d_reference) - 0.02)*6
        rightEyewid = ((d3 + d4)/(2*d_reference) - 0.02)*6
        mouthWid = (d5/d_reference - 0.13)*1.27 + 0.02
        mouthLen = d6/d_reference
        return leftEyeWid, rightEyewid, mouthWid,mouthLen


    # ==================================================================
    # Get largest face
    #   - dets:
    # ==================================================================  
    def get_largest_face(self, dets):
        if len(dets) == 1: return 0
        face_areas = [(det.right() - det.left())*(det.bottom() - det.top()) for det in dets]
        largest_area  = face_areas[0]
        largest_index = 0
        for i in range(1, len(dets)):
            if face_areas[i] > largest_area :
                largest_index = i
                largest_area  = face_areas[i]
        print("largest_face index is {} in {} faces".format(largest_index, len(dets)))
        return largest_index
    

    # ==================================================================
    # Feature points extraction using dlib
    #   - img:
    # ==================================================================  
    def get_image_points(self, img): 
        # Adaptive histogram equalization  
        gray    = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
        gray_eq = self.clahe.apply(gray) 
        # cv2.imshow("gray", gray)
        # cv2.imshow("gray_eq", gray_eq)
        dets = self.detector(gray_eq, 0)

        if len(dets) == 0:
            print("ERROR: Failed to detect face!")
            return -1, None
        largest_index  = self.get_largest_face(dets)
        face_rectangle = dets[largest_index]
        landmark_shape = self.predictor(img, face_rectangle)
        return 0, landmark_shape


    # ==================================================================
    # Pose estimation: get rotation vector and translation vector 
    #   - img_size:
    #   - image_points:
    # ==================================================================  
    def get_pose_estimation(self, img_size, image_points):
        # Camera internals     
        focal_length = img_size[1]
        center = (img_size[1]/2, img_size[0]/2)
        camera_matrix = np.array([[focal_length, 0, center[0]],
                                  [0, focal_length, center[1]],
                                  [0, 0, 1]], dtype = "double")     
        print("Camera Matrix:\n {}".format(camera_matrix))
        
        # Assuming no lens distortion
        dist_coeffs = np.zeros((4,1)) 
        imagePoints = np.ascontiguousarray(image_points[:,:2]).reshape((6,1,2))
        (success, rotation_vector, translation_vector) \
            = cv2.solvePnP(self.model_points, imagePoints, camera_matrix, dist_coeffs, flags=cv2.SOLVEPNP_DLS)
        
        # rotation_vector[0] = kalman_filter_simple(rotation_vector[0], 0.1, 0.01)
        # rotation_vector[1] = kalman_filter_simple(rotation_vector[1], 0.1, 0.01)
        # rotation_vector[2] = kalman_filter_simple(rotation_vector[2], 0.1, 0.01)

        print("Rotation Vector:\n {}".format(rotation_vector))
        print("Translation Vector:\n {}".format(translation_vector))
        return success, rotation_vector, translation_vector, camera_matrix, dist_coeffs


    # ==================================================================
    # Convert rotation_vector to quaternion 
    #   - rotation_vector:
    # ==================================================================  
    def get_quaternion(self, rotation_vector):
        # Calculate rotation angles
        theta = cv2.norm(rotation_vector, cv2.NORM_L2)
        # theta = self.mean_filter_simple(self.queue1D, theta)
        
        # Transformed to quaterniond
        w = math.cos(theta/2)
        x = math.sin(theta/2)*rotation_vector[0][0]/theta
        y = math.sin(theta/2)*rotation_vector[1][0]/theta
        z = math.sin(theta/2)*rotation_vector[2][0]/theta
        return round(w,4), round(x,4), round(y,4), round(z,4)



    # ==================================================================
    # Visualization
    #   - rotation_vector:
    #   - translation_vector:
    #   - camera_matrix:
    #   - dist_coeffs:
    # ==================================================================  
    def visualize(self, rotation_vector, translation_vector, camera_matrix, dist_coeffs):
        # Project a 3D point set onto the image plane, which is used to draw a bounding box
        (nose_end_point2D, jacobian) = cv2.projectPoints(np.array([(0., 0., 1000)]), \
            rotation_vector, translation_vector, camera_matrix, dist_coeffs)
        
        for i in range(8):
            (self.boxPoints2D[:,:,i,:], jacobian) = cv2.projectPoints(np.array([self.boxPoints3D[i]]), \
                rotation_vector, translation_vector, camera_matrix, dist_coeffs)        
        self.boxPoints2D = self.boxPoints2D.astype(int)

        for p in self.image_points:
            cv2.circle(self.img, (int(p[0]), int(p[1])), 3, (0,0,255), -1)

        boxset_1 = self.boxPoints2D[0,0,0:4,:]
        boxset_2 = self.boxPoints2D[0,0,4:8,:]
        boxset_3 = np.vstack((self.boxPoints2D[0,0,0,:],self.boxPoints2D[0,0,4,:]))
        boxset_4 = np.vstack((self.boxPoints2D[0,0,1,:],self.boxPoints2D[0,0,5,:]))
        boxset_5 = np.vstack((self.boxPoints2D[0,0,2,:],self.boxPoints2D[0,0,6,:]))
        boxset_6 = np.vstack((self.boxPoints2D[0,0,3,:],self.boxPoints2D[0,0,7,:]))

        cv2.polylines(self.img, [boxset_1], True, (255,0,0), 3)
        cv2.polylines(self.img, [boxset_2], True, (255,0,0), 3)
        cv2.polylines(self.img, [boxset_3], True, (255,0,0), 3)
        cv2.polylines(self.img, [boxset_4], True, (255,0,0), 3)
        cv2.polylines(self.img, [boxset_5], True, (255,0,0), 3)
        cv2.polylines(self.img, [boxset_6], True, (255,0,0), 3)
        
        p1 = (int(self.image_points[0][0]), int(self.image_points[0][1]))
        p2 = (int(nose_end_point2D[0][0][0]), int(nose_end_point2D[0][0][1]))
        cv2.line(self.img, p1, p2, (0,255,0), 2)

        cv2.imshow("Visualization", self.img)


    # ==================================================================
    # Start vision measurement for once
    # ==================================================================  
    def measure(self):
        start_time = time.time()
    
        # Read image
        ret, self.img = self.camera.read()
        if not ret:
            print("ERROR: Failed to read frame!")
            return
        size = self.img.shape
        
        # Compress image if it's to large
        if size[0] > 700:
            h = size[0]/3
            w = size[1]/3
            self.img  = cv2.resize(self.img, (int(w), int(h)), interpolation=cv2.INTER_CUBIC)
            size = self.img.shape
        
        # self.img = cv2.normalize(self.img, dst=None, alpha=350, beta=10, norm_type=cv2.NORM_MINMAX)
        ret, landmark_shape = self.get_image_points(self.img)
        if ret != 0:
            print("ERROR: Failed to get image points!")
            return
        
        # Compute feature parameters of facial expressions (eyes, mouth)
        landmarks_orig = self.landmarks_to_np(landmark_shape)
        
        # Apply kalman filter to landmarks FOR POSE ESTIMATION
        self.KalmanX.update(self.uu_, landmarks_orig[:,0])
        self.KalmanY.update(self.uu_, landmarks_orig[:,1])
        self.landmarks[:,0] = self.KalmanX.xx.astype(np.int32)
        self.landmarks[:,1] = self.KalmanY.xx.astype(np.int32)

        # Apply smooth filter to landmarks FOR POSE ESTIMATION
        self.landmarks = self.mean_filter_for_landmarks(self.queue3D_points, self.landmarks) 
        leftEyeWid, rightEyewid, mouthWid, mouthLen = self.get_feature_parameters(landmarks_orig)
        print ("leftEyeWid:{}, rightEyewid:{}, mouthWid:{}, mouthLen:{}".format(leftEyeWid, rightEyewid, mouthWid, mouthLen))

        # Five feature points for pose estimation
        # self.image_points = np.vstack((self.landmarks[30], self.landmarks[8], self.landmarks[36], \
        #     self.landmarks[45], self.landmarks[48], self.landmarks[54]))
        self.image_points = np.vstack((self.landmarks[30], self.landmarks[8], self.landmarks[36], \
            self.landmarks[45], self.landmarks[1], self.landmarks[15]))
        
        ret, rotation_vector, translation_vector, camera_matrix, dist_coeffs \
            = self.get_pose_estimation(size, self.image_points)
        if not ret:
            print("ERROR: Failed to get pose estimation!")
            return
        
        # Convert rotation_vector to quaternion
        w, x, y, z = self.get_quaternion(rotation_vector)
        print("w:{}, x:{}, y:{}, z:{}".format(w, x, y, z))

        used_time = time.time() - start_time
        print("used_time:{} sec".format(round(used_time, 3)))
        print("-"*20)
        
        # Packing data and transmit to server through Socket
        data = str(translation_vector[0,0]) + ":" + str(translation_vector[1,0]) + ":" \
            + str(translation_vector[2,0]) + ":" + str(w) + ":" + str(x) + ":" + str(y) + ":" + str(z) + ":" \
            + str(leftEyeWid) + ":" + str(rightEyewid) + ":" + str(mouthWid) + ":" + str(mouthLen)
        return data, rotation_vector, translation_vector, camera_matrix, dist_coeffs
        