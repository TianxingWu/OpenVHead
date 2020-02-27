#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Created on Wed Feb 26 16:19:20 2020

@author: Hawk Shaw
"""


import sys
import socket


class PyClient():

    def __init__(self, host:str, port:int):
        self._host = host
        self._port = port
        self._client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self._connect()
    
    def _connect(self):
        try:
            self._client.connect((self._host, self._port))
            print("Socket connected, listening on %s:%d...\n" % (self._host, self._port))
        except:
            print("ERROR: No socket connection!")

    def send(self, data:str, encoding:str="utf-8"):
        try:
            self._client.send(data.encode(encoding))
        except:
            print("ERROR: Failed to send data to server!")
