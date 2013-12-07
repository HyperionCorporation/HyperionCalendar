#Hyperion Calculator

##Project by Bryan Perino and Matt Crichton

###How to Compile

Project includes the files for Visual Studio. I will eventually move to an automated build system similar to Make if I can find one. Just
open the project in Visual Studio to build. 

You need to link the SQLite DLL in order to compile. It is included in the repository

Calendar/System.Data.SQLite.dll

To Add the reference in Visual Studio

+ Right Click on Project
+ Click Add Reference
+ On the Right Side click Browse
+ Click Browse on the bottom and browse to the file
+ Click OK until you're out of the dialogs

###Planned Features

+ Events are synced to remote database
+ View Events with Right Click

###Currently Implemented Features

+ Can Add events and delete them
+ Can create users and log in
+ Calender Generation
+ User information stored in local database
+ User accounts with login
+ Add events to the calendar

###Bugs
+ None right now

###ToDo
+ Custom calendar cell's rectangles need work
+ Add validity checking
+ Commment code