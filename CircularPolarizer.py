import numpy as np
import math as math
import random as rand
import matplotlib.pyplot as plt

class light:
    def __init__ (self):
        self.polarization = 0
        self.amplitude = 1
        self.frequency = 1
        self.ampX = 0
        self.ampY = 0
        self.cycleAngle = 0
    def adjustAmp (self):
        self.ampY = math.cos(self.polarization)
        self.ampX = math.sin(self.polarization)
    def adjustRetard (self, retardation, retardAngle):
        newAmpX = self.amplitude * math.cos(retardAngle + self.polarization)
        newAmpy = self.amplitude * math.sin(retardAngle + self.polarization)
        newAmpX = math.cos(self.cycleAngle + retardation) * self.amplitude
        self.ampX = math.cos(retardAngle + self.polarization) * newAmpX + math.cos(retardAngle + self.polarization) * newAmpy
        self.ampY = math.sin(retardAngle + self.polarization) * newAmpX + math.sin(retardAngle + self.polarization) * newAmpy
        #print(self.ampX)
        #self.polarization = math.acos(self.ampX / self.amplitude)
lights = [light() for i in range(100)]
x = [0 for i in range(100)]
y = [0 for i in range(100)]
currentAngle = 0
index = 0
for lightPlace in lights:
    lightPlace.cycleAngle = currentAngle
    lightPlace.polarization = math.pi / 4
    lightPlace.adjustAmp()
    lightPlace.adjustRetard(math.pi / 2, 0)
    x[index] = currentAngle / (math.pi / 24)
    y[index] = lightPlace.ampY
    index+=1
    currentAngle = currentAngle + math.pi / 24
    #print (str(lightPlace.polarization)) 
plt.plot(x, y)
plt.show()
    