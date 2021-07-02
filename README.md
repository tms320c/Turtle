# Turtle

## The projects

### Turtle World
This is a class library which is used by other two projects. It has three namespaces:
 1. __Core__ contains the 'fundamental' things that defines the semantic of the solution.
 1. __Entities__ is for the essential objects which are required in the most parts of the projects. 
 1. __Structure__ has 'glue' elements that aids in conversion of the more or less abstract _core_ and _entities_ into useful things.

### FTurtle
.NET 5 console application designed in a kind of functional approach where the solution is a series of the data transformations:
 from configuration file to list of commands to list of positions to final result.

_Use_: FTurtle 'file name' 

where mandatory parameter 'file name' is the name of a configuration file in the specified format.

### OTurtle
.NET 5 console application designed as a simple and kind of naive OOP model of the turtle and world where it moves.
The turtle is rather passive, and there is a mover that controls it.

_Use_: OTurtle 'file name'

where mandatory parameter 'file name' is the name of a configuration file in the specified format.

### TurtleTests
xUnit tests for all projects, with bigger emphasis on the _Turtle World_ project.

### General notes
I tried to fulfill "performance even on a larger scale" and "Avoid the use of mutation" requirements.
The application uses lazy collections for most of the data transformations. The most serious limitation is the assumption that
entire configuration file must fit into RAM, while it is read line by line.

I did not find a good way of sensible using async and multithreading for such tiny project, which already looks quite close to overkill.
Anyway, some classes are 'async ready' for the demo purposes.

By the same reason I did not use DI, events etc. Neither logging, nor Prometeus instrumentation has been utilized too :)

Both applications use console output to display the results. I made it on purpose for the simplicity. In any case, the output
it implemented using delegates, so it has no real dependency of console. It can be anything that converts a string to whatever.

### Coordinate system
The coordinate frame is defined as:
 1. The origin _(0,0)_ is at top left corner of the board.
 1. X-axis direction is from top to bottom, along _height_ of the board.
 1. Y-axis direction is from left to right, along _width_ of the board.
 1. Top boundary of the board is _North_ boundary.
 1. Left boundary of the board is _West_ boundary.
 1. Bottom boundary of the board is _South_ boundary.
 1. Right boundary of the board is _East_ boundary.

### Coordinate notation in the configuration file
 1. Coordinates of mines are defined as (x,y) paris. Record <code>3,4</code> means x=3, y=4.
 1. Coordinates of the exit point (target) are defined as (x y) pair. Record <code>7 8</code> means x = 7, y = 8.
 1. Coordinates of the start point are defined in the same way as for the exit point with addition of the initial heading. 
    Record <code>0 1 S</code> means starting coordinates x=0, y= 1, heading South (i.e. toward bottom boundary of the board).
    
__Note__: the board dimensions are defined as _width_ _height_ pair. _width_ is a span along Y axis (left to right) and _height_
is a span along X axis (top to bottom). 

Record <code>7 8</code> means that width=7 and height=8. 
It also means that X<sub>min</sub> = 0, X<sub>max</sub> = _height_ - 1 = 7
and Y<sub>min</sub> = 0, Y<sub>max</sub> = _width_ - 1 = 6.
