#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Created on Wed Feb 26 16:19:20 2020

@author: Hawk Shaw
"""


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
