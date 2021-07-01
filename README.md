# Turtle

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
