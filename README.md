# WeatherPrototype v0.1.1
Welcome to WeatherProtoype, a project with the goal of analyzing National Weather Service regional radar loops and informing users of any approaching weather in their vicinity by accurately identifying areas of precipitation and tracking their motion. This project is still in the prototype stage and is developing its proposed functionality.

The project currently exists as a Winforms application with basic functionality for selecting radar loop GIFs, detecting user location, and checking whether or not rain is in the user's proximity. It uses the following NuGet packages: 
- Emgu.CV (process images and identify precip.)
- Magick.NET (dissect/recompile GIFs)
  
The following packages may be necessary:
- Emgu.CV.runtime.windows
- System.Drawing.Common

## Obtaining Loops For The Program
The program is currently configured to interpret NWS regional radar loops, which can be found [here](https://radar.weather.gov/region/conus/standard). First you will need to click the search icon which says "National" and pick your local region from the dropdown list, where you will then be directed to your regional radar loop along with an "Image Loop" link which will take you to a new tab with the loop GIF. From there you can right click and save it on your system as a GIF file. _**!! As of current, only "Southern Mississippi Valley" is fully supported !!**_

## Using the program
The program will begin with a prompt to enter your WGS84 decimal geocoordinates (this is the coordinate system used most familiarly, you can find them using Google Maps). Once valid coordinates have been entered, you will be taken to a landing page where you can upload your radar loop and process it. Once the loop is properly processed, you will see your location pop up on the map. 

![image](https://i.imgur.com/kzmzd3N.png)

From here, you can go to `Debug` and toggle `Show Program Log`. A log will appear below the loop, containing information on any approaching rain detected in the vicinity.

![image](https://i.imgur.com/lb6IGS6.png)
