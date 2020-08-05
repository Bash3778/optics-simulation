import numpy
import astropy
import IPython
import math as math
import random

#The number of points in the drawing
numbPoints = 7
#The arrays of the x/y/z positions of the points
xpositions = [0.0 for i in range(numbPoints)]
ypositions = [0.0 for i in range(numbPoints)]
zpositions = [0.0 for i in range(numbPoints)]
#The array of every possible combination
combinations = [[0 for i in range(numbPoints)] for j in range(int(math.pow(numbPoints, numbPoints)))]
permutations = [[0 for i in range(numbPoints)] for j in range(math.factorial(numbPoints))]
permPath = [[0.0 for i in range(numbPoints)] for j in range(math.factorial(numbPoints))]
bestOrder =[0 for i in range(numbPoints)]
#The index of the best path
bestPath = -1
bestLength = -1.0
def pathLength (start, end):
    xdist = abs(xpositions[start - 1] - xpositions[end - 1])
    ydist = abs(ypositions[start - 1] - ypositions[end - 1])
    zdist = abs(zpositions[start - 1] - zpositions[end - 1])
    dist = math.sqrt(math.pow(xdist, 2) + math.pow(ydist, 2) + math.pow(zdist, 2))
    return dist
datas = None
with open('/Users/benjaminash/Desktop/School/Optics/StartingPositions', 'r') as file:
    datas = file.read()
sort = datas.split()
positionCounter = 0
indexCounter = 0
for sorting in sort:
    if positionCounter == 0:
        xpositions [indexCounter] = float(sorting)
        positionCounter+=1
    elif positionCounter == 1: 
        ypositions [indexCounter] = float(sorting)
        positionCounter+=1
    else:
        zpositions[indexCounter] = float(sorting)
        positionCounter = 0
        indexCounter+=1
def preBake ():
    global bestLength
    global bestPath
    global bestOrder
    flipnumb = 0
    for i in range(numbPoints):
        combinations[0][i] = 1
    for i in range(int(math.pow(numbPoints, numbPoints))):
        for j in range(numbPoints):
            if i != 0:
                combinations[i][j] = combinations[i - 1][j]
        if i != 0:
            combinations[i][0]+=1
        for j in range(numbPoints):
            if combinations[i][j-1] == numbPoints + 1:
                combinations[i][j-1] = 1
                combinations[i][j]+=1
        repeat = False
        for j in range(numbPoints):
            for n in range(numbPoints):
                if combinations[i][j] == combinations[i][n] and j != n:
                    repeat = True
        if repeat == False:
            permutations[flipnumb] = combinations[i]
            flipnumb+=1
    for i in range(math.factorial(numbPoints)):
        length = 0.0
        for j in range(numbPoints):
            if j != 0 and j != numbPoints - 1:
                length += pathLength(permutations[i][j], permutations[i][j-1])
            elif j == numbPoints - 1:
                length += pathLength(permutations[i][j], permutations[i][0])
        permPath[i] = length
        if bestPath < 0:
            bestPath = i
            bestLength = length
        elif length < bestLength:
            bestPath = i
            bestLength = length
    bestOrder = permutations[bestPath]      
def animate ():
    global bestLength
    global bestOrder
    known = [0 for i in range(numbPoints)]
    possible = [0 for i in range(numbPoints)]
    for i in range(len(known)):
        known[i] = i
        possible[i] = i
    for i in range(len(known)):
        if i != 0:
            distances = [0.0 for i in range(len(possible))]
            smallestLength = -1.0
            smallestIndex = -1
            for j in range(len(possible)):
                if i != j:
                    distances [j] = pathLength(possible[j], known[i])
            for j in range(len(distances)):
                if distances[j] != 0.0:
                    if smallestIndex == -1:
                        smallestLength = distances[j]
                        smallestIndex = j
                    if distances[j] < smallestLength:
                        smallestIndex = j
                        smallestLength = distances[j]
            if bestLength == -1.0:
                bestLength = smallestLength
            else:
                bestLength += smallestLength
            bestOrder[i] = possible[smallestIndex]
            possible.remove(possible[smallestIndex])
        else:
            bestOrder[i] = i     