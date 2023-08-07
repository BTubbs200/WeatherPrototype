# WeatherPrototype
Welcome to WeatherProtoype, a project that has the goal of analyzing National Weather Service regional radar loops and informing users of any weather in the vicinity by accurately identifying areas of precipitation and tracking their motion. This project is still in the prototype stage and is very much a work-in-progress.

The project currently exists as a Winforms application with basic functionality for processing radar loop GIFs and appending "RAIN" text on identified areas of precipitation. The code is in the process of being converted from a Console App to Winforms so there is some cleanup left to do. It uses two custom libraries: Emgu CV for processing each frame of the provided GIF, and ImageMagick for compiling the frames back into a Bitmap which can be properly interpreted and displayed by the System.

## Obtaining Loops For The Program
The program is currently configured to interpret NWS regional radar loops, which can be found [here](https://radar.weather.gov/region/conus/standard). First you will need to click the search icon which says "National" and pick your local region from the dropdown list, where you will then be directed to your regional radar loop along with an "Image Loop" link which will take you to a new tab with the loop GIF. From there you can right click and save it on your system as a GIF file. 

## Using the program
Load your loop GIF into the program from the `File` menu dropdown and your loop will automatically display in the program. From there all that's left to do is press the `Process Loop` button and watch the magic unfold!

![image](https://github.com/BTubbs200/WeatherPrototype/assets/131938002/4b586238-c07a-4abf-b83c-c3cc983882d4)

![image](https://github.com/BTubbs200/WeatherPrototype/assets/131938002/0d401937-abc8-4e2b-a637-1c2d013a8afa)

## Note
The program does a nearly perfect job of detecting all areas of rain, although this may not reflect 100% accurately in the text display, which only serves as feedback for human eyes and is limitied by a threshold to prevent text from completely bombarding the screen.
