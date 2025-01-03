

# [CasaMigos]
website: https://casa0019-group5.github.io/

<img src="./docs/physical device.jpg" alt="Image">

***
## Why did we choose water data?

After we received the initial brief and theme, we started digging for interesting datasets linked to UCL. We then discovered some interesting operational building data about One Pool Street. It was a live MQTT stream of water meter data in One Pool Street since the building first opened. The topic is, UCL/OPSEBOAS/PSW TW20-XX-CE-001/BACnet Interface/Application/Energy Monitoring/Water Meters, for anyone interested in exploring this data themselves! The Water Meter data was only for one tower of One Pool Street and it was split into two sections of the one tower, SAC1 and SAC2. In addition, it showed us data from Floors 4 to 19. Therefore, with SAC1 and SAC2 combined we had 30 individual water meter readings.

***
## Initial Brainstorm

We then had to decide how we wanted to represent this data. After some brainstorming we were all in agreement that an old-fashioned water gauge was out of the picture, we wanted something a little more unique. Taking inspiration from one of Andy's projects where he had used a syringe to visualise, we took his model of the syringe and repurposed it to show the water usage data, through the syringe pushing water up the device to demonstrate the total daily water usage.

<video width="400" controls autoplay>
    <source src="./docs/physical_device_overview.MOV" type="video/mp4">
</video>

*** 
## Physical Device  -- Jiaying Shen
#### Main idea: 
Receiving data from the Raspberry Pi via the Arduino, the servo is controlled to push the syringe, while the total amount of water used by the OPS for the day is displayed via a scale and NeoPixel lights.
####  Design flow:
<div style="display: flex; align-items: center;">
  <img src="https://raw.githubusercontent.com/casa0019-group5/casa0019-group5.github.io/refs/heads/main/PhysicalDevice/img/img1.png" width="600">
</div>

<b> Stage 1: Syringe attachment method </b>
Thanks to Andy for supplying us with the raw materials: 10ml syringes, 10ml cylinder, plastic tube and 3D model for syringe holder, rack gear and pinion gear. Slightly changes have been made to cope with project.

When designing the physical device, I tried three different methods. The first method was to use a U-shaped tube with a hole at the bottom connected to a syringe. By pushing or pulling the syringe with a servo, water was injected or extracted to display the water level. However, due to the principle of communicating vessels, the water level on both sides of the U-tube remained the same, making it impossible to accurately reflect the movement of the syringe. This approach was eventually abandoned.

The second method used a soft tube to connect the syringe to a measuring cylinder. By pushing the syringe, water was injected into the cylinder to indicate water usgae, and by pulling back, water was withdrawn. However, during testing, it was found that some water remained in the cylinder after each reset, causing it to build up over time, leading to inaccurate readings and water loss. As a result, this method was not adopted.

The final method successfully implemented was to connect two syringes with a plastic tube to form a sealed hydraulic system. Squeezing one syringe forced water into the other, moving the plunger to indicate water consumption. This solution prevented water residue, allowed full water recirculation and ensured accurate resets, providing a relatively stable and accurate measurement system.

<b> stage2: Servo selection and syringe connection </b>
As the syringe is filled with water, the servo needs sufficient torque to push the plunger. Precise control of the scale is required, so the servo must have a minimum rotation angle of 180 degrees.

# Servo Comparison Table

| Servo Model  | Torque (kg·cm) | Rotation Angle (degrees) | Voltage (Volt) |
|--------------|----------------|-------------------------|----------------|
| SG90         | 1.6            | 360                     | 4.8-6.0        |
| MG90S        | 1.8            | 180                     | 4.8-6.0        |
| TS90         | 1.8            | 180                     | 3.3-5.0        |
| Geek Servo   | 2              | 270                     | 3.0-5.0        |

After comparison and testing, some servos were unable to rotate a full 180 degrees, preventing the syringe from fully extending or resetting to zero. Servos with 180 degree rotation offer better precision and control compared to 360 degree servos. As a result, the Geek servo was ultimately chosen.

<b> Stage 3 : Circuit connection </b>
<div style="display: flex; align-items: center;">
  <img src="https://raw.githubusercontent.com/casa0019-group5/casa0019-group5.github.io/refs/heads/main/PhysicalDevice/img/flowchart.png" width="600">
</div>

<div style="display: flex; align-items: center;">
  <img src="https://raw.githubusercontent.com/casa0019-group5/casa0019-group5.github.io/refs/heads/main/PhysicalDevice/img/circuit.png" width="600">
</div>
Arduino and Raspberry Pi based automation system to control the syringe and NeoPixel light to indicate water usage.The system is first connected to WiFi and initialised. Once initialised, data is fetched from the Raspberry Pi and processed to map it to the position of the syringe servo and the state of the NeoPixel light. If the data is less than 500, the system resets the syringe piston to its initial position, otherwise it continues to cycle through data acquisition and processing to maintain a real-time response state.

<b> Stage 4 : Enclosure and test </b>  
A laser cutter was used to make holes in a single wooden board to hold both the syringe and the servo. The sizes and positions of the holes were determined through multiple tests to ensure that the servo could smoothly push and pull the syringe along the scale, reaching both maximum and minimum values. As the servo and NeoPixel are powered by Arduino, the entire circuit was placed in a box under the wooden board to maintain a clean appearance and avoid interference with the power supply. The box is secured with interlocking edges for stability and is easy to open and close, allowing quick access to the Arduino MKR1010's reset button for efficient troubleshooting and maintenance without dismantling the entire system.

#### Design Optimization and Future Improvements
During the design process, several areas for optimisation and improvement were identified to improve both functionality and reliability.

One potential improvement is the use of coloured liquid inside the syringe to provide clearer and more intuitive visual feedback on water usage. This would make it easier to read the syringe's scale and observe the movement of the plunger, improving the user experience. In addition, the use of larger syringes would allow for greater measurement capacity, reducing the need for frequent resets and enabling the system to handle larger volumes of water.

To ensure stability and precision, an advanced holder could be designed to securely hold the servo and its gears. This would help prevent unwanted movement or misalignment. Occasionally, rapid servo movement caused by miscalculation can cause the gears to slip or pop out. A more secure holder design would reduce this risk by holding the gears firmly in place, even during sudden movements.

Another area for improvement is the Arduino MKR1010's network connection, which can sometimes be unstable. To address this, the box design can incorporate an external button connected to the Arduino's reset pin, allowing quick manual resets without opening the case. This ensures that troubleshooting and maintenance can be carried out efficiently while protecting the internal components.


https://github.com/user-attachments/assets/a4a8edd3-a0f4-40e1-bce6-6a52c5a1cce4


#### Reference
1.	https://clipart-library.com/clip-art/syringe-clipart-transparent-21.htm
2.	https://www.freepik.com/icon/servo_6276775
3.	https://clipart-library.com/clip-art/366-3666355_arduino-vector-black-and-white-arduino-icon.htm
4.	https://components101.com/motors/mg90s-metal-gear-servo-motor
5.	https://bc-robotics.com/shop/micro-servo-sg90-360-degrees/
6.	https://www.ebay.com/itm/225446283692?msockid=18393fb6b706665c3be02abfb6c067fb
7.	https://forum.fritzing.org/t/pine-a64-esp8266-12e-f-arduino-zero-and-mkr1000-new-fritzing-parts/1611
8.	https://github.com/adafruit/Fritzing-Library/blob/master/parts/Adafruit%20NeoPixel%20Stick.fzpz package for fritzing Neopixel8


***
## MQTT to Raspberry Pi

One of the main problems we encountered with the MQTT stream was the lack of control we had over the data we received. We could only display data as it came from MQTT. This meant for example we couldn't display specific data such as data from the last 24 hours. After some discussion, we decided that some sort of database was needed for the MQTT stream so we could extract specific information from it. Vineeth diligently worked away at moving the MQTT stream onto an SQL database via Raspberry Pi. After overcoming various challenges Vineeth made us a database with 4 different endpoints:

1. Latest Total Usage by Floor
   Input: FloorNum
   Output: Sum of SAC1 and SAC2 for FloorNum
2. Latest Total Uage by SAC
   Input: SACNum
   Output: Sum of all Floors for SAC#
3. Today's Usage by Floor
   Input: FloorNum
   Output: Difference between midnight and latest reading of endpoint 1
4. Today's Usage by SAC
   Input: SACNum
   Output: Difference between midnight and latest reading of endpoint 2.

## Backend Setup

To improve the run-time of the various systems of this project, as well as make better sense of the data, we've implemented a backend to suppliment the outputs. Since the water data pulled from the OPS MQTT Stream only tells us the total water used by each sensor since the time they were initialized, the `collect_water.js` script takes timestamped readings from the OPS MQTT stream, and saves them to a MariaDB table. By doing this, we now have historical data to understand how water usage changes throughout the day.

The `collect_water.js` script is setup in a PM2 instance, this contains the entire build environment, and therefore allows us to set the environmental variables. This specific script is then restarted every 10 minutes using a crontab job. Furthermore, by using a PM2 instance, we can also provide out `server.js` file with all the appropriate values to be continously running in the background of the pi.

![backend arch](./docs/Backend.drawio.png)

## RaspberryPi Setup

### Install

1. Pull the RpiScript folder onto the raspberry pi.
   It should include:

- `ecosystem.config.cjs` : Contains all the environmental variables to create and run the PM2 Instance. (create your own)
- `collect_water.js` : Script to update the database.
- `server.js` : Script that contains the API.

2. Run `npm init && npm install` to install all the required node packages listed in `package.json`.

3. Install mariadb-server using `apt install mariadb-server`

- Might have to reboot

4. Start up the mariaDB Server using `sudo systemctl start mariadb`

- Verify it's running with `sudo systemctl status mariadb`

- `CTRL-C` exits the status, without stopping the service.

5. Setup the password for mariaDB

- `sudo mariadb`
- `ALTER USER 'root'@'localhost' IDENTIFIED BY '<insert password here>`;
- `FLUSH PRIVILEGES;`
- `exit`

6. Check if you can login

- `mariadb -u root -p`
- enter selected password
- Note: password should match what is listed in the .env file

7. Run `collect_water.js`

- Note: Database is built the first run, but is unaccessible by the script until the second run.

### Check Results

1. Login to MariaDB

- `mariadb -u root -p`
- Enter Password
- `SELECT water_db;`
- `SELECT * FROM water_table;`

***
## Design of Dashboard
![Dashboard](./docs/Dashboard.png)

When establishing the database, we found that the data we could obtain were the water consumption of the east and west sides of each floor and the total water consumption of the floor. Since it contains data on the water consumption of 16 floors from the 4th floor to the 19th floor, we didn't find a suitable table to display so many data at the same time. Therefore, we chose to use the form of numbers to present the data of the total water consumption of each layer more intuitively. In addition to the longitudinal comparison of each layer, we also used the data of different water consumption on the east and west sides of each layer. The bar chart on the dashboard also gives users a clear picture of the difference in water consumption between the east and west sides of the building. Because we chose to display daily water consumption data, it is necessary to display the day of the day. At the same time, we also add some explanatory text, such as unit, title, etc., to help users understand this dashboard. Since we are analyzing the use of water resources, we chose to use the cute image of a kitten to call on everyone to save water together.

## AR of Dashboard
![AR Dashboard](./docs/AR%20Dashboard.jpg)

First, we considered the possibility of bringing the entire dashboard to the screen just by scanning the logo on the physical device. Therefore, we chose to use the AR Tracked Image Manager script and put a photo of the logo in it. However, we found that the image was not always accurately recognized, so the dashboard sometimes did not appear as expected. So we added the Tap To Place script. This way, even if the image is not recognized, we can tap the screen to make the dashboard appear.

