# WeatherPrototype v0.1.1
Welcome to WeatherProtoype, a project that has the goal of analyzing National Weather Service regional radar loops and informing users of any weather in the vicinity by accurately identifying areas of precipitation and tracking their motion. This project is still in the prototype stage and is very much a work-in-progress.

The project currently exists as a Winforms application with basic functionality for processing radar loop GIFs, detecting user location, and identifying areas of precipitation in each frame of the loop. It uses the following NuGet packages: 
- Emgu.CV (process images and identify precip)
- Magick.NET (dissect/recompile GIFs)
  
The following packages may be necessary:
- Emgu.CV.runtime.windows
- System.Drawing.Common

## Obtaining Loops For The Program
The program is currently configured to interpret NWS regional radar loops, which can be found [here](https://radar.weather.gov/region/conus/standard). First you will need to click the search icon which says "National" and pick your local region from the dropdown list, where you will then be directed to your regional radar loop along with an "Image Loop" link which will take you to a new tab with the loop GIF. From there you can right click and save it on your system as a GIF file. 

## Using the program
The program will begin with a prompt to enter your WGS84 geocoordinates (this is the coordinate system used most familiarly, you can find them using Google Maps). Once valid coordinates have been entered, you will be taken to a landing page where you can upload your radar loop and process it. Once the loop is properly processed, you will see your location pop up on the map as well as strings of "RAIN" appended to identified areas of rain (this is currently a visualizer that will not be present in future versions).

![image](https://github.com/BTubbs200/WeatherPrototype/assets/131938002/d27312dd-fe9f-4077-9d95-d7bb55294af9)

## Note
The program does a nearly perfect job of detecting all areas of rain, although this may not reflect 100% accurately in the text display, which only serves as feedback for human eyes and is limited by a threshold to prevent text from completely bombarding the screen.
