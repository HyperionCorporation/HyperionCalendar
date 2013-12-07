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

+ Add events to the calendar
+ Events are synced to remote database
+ User accounts with login

###Currently Implemented Features

+ Can Add events and delete them
+ Can create users and log in
+ Calender Generation
+ User information stored in local database


###Bugs
+ None right now

###ToDo
+ Custom calendar cell's rectangles need work
+ Add Remote Database and sync
+ Add validity checking
+ Add ability to create and delete events
+ Add events to database
+ Clean up how Users are handled, One class/struct only, not 2.
+ Commment code