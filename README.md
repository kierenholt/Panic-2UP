# Panic-2UP
2 player python-script-to-driving-AI game. Written over about 3 months in early 2018

## Why? 
I wanted to make something fun for my students that were learning Python. Some of them finished the work much earlier than their peers. This game was designed to soak up some of that time in an engaging, possibly competitive way.

## Which bits are you most proud of?
It ran very slowly to begin with but I managed to get it to run smoothly after several optimisations:
* CollisionChecker.cs - A very fast 2D box collision checker which makes only the necessary boundary checks due to storing a collision state and then updating it every frame
* PythonScript.cs -  A wrapper for the IronPython Compiler which turns Python code into a c# <Func> which can be called every frame
* Bearing.cs - In order to update the car sprite, a 'heading' needed to be calculated from the velocity x and y components each frame. After profiliing the very slow atan function, I replaced it with a class which retained state information about the previous 'heading' so only two linear inequalities were needed instead of the inverse tan function

## What did you learn?
* Game.cs - Implement and manage parallel threads using the System.Threading.Task class
* Game.cs - How to use WriteableBitmap as a sort of graphics buffer
* More MVVM magic

## What happened to it? 
Unfortunately I was only able to use it in lessons for a couple of months before the school changed its IT policy which prevented students running the file, and then the PC's were replaced with Chromebooks


