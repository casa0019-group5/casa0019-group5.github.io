# CasaMigos

website: https://casa0019-group5.github.io/

<img src="./docs/physical device.jpg" alt="Image">

---

## Why did we choose water data?

After recieving the initial assignment brief, we started digging through datasources linked with UCL. One source that caught our eye early on was the MQTT stream for One Pool Street. Going further, we setteled on water meter data from the building. Working with William Markiewicz, we came to understand the MQTT stream showed the total water usage from the time the sensors were first powered on, and that the data was only for floors 4 through 19 in the west tower. Furthermore, the MQTT topics were split by both floor and SAC. SAC is a term used to describe one half of a floor, in the below image, the floor is split through the middle, where the top half would be represented by SAC1, and the bottom half would be represented by SAC2. To anyone interested in looking at the existing datastream, the topic is: `UCL/OPSEBOAS/PSW TW20-XX-CE-001/BACnet Interface/Application/Energy Monitoring/Water Meters`.

<!--
After we received the initial brief and theme, we started digging for interesting datasets linked to UCL. We discovered some interesting operational building data about One Pool Street. It was a live MQTT stream of water meter data in One Pool Street since the building first opened. The topic is, , for anyone interested in exploring this data themselves! The Water Meter data was only for the west tower; in the below image, the floor is split through the middle, where the top half is SAC1, and the bottom half is SAC2. In addition, it showed us data from Floors 4 to 19. Therefore, with SAC1 and SAC2 combined we had 30 individual water meter readings. -->

![Tower West Breakdown](./docs/TowerWestBreakdown.png)

_Tower West SACs_

---

## Initial Brainstorm

We then had to decide how we wanted to represent this data. After some brainstorming we were all in agreement that an old-fashioned water gauge was out of the picture, we wanted something a little more unique. Taking inspiration from one of Andy's projects where he had used a syringe to visualise, we took his model of the syringe and repurposed it to show the water usage data, through the syringe pushing water up the device to demonstrate the total daily water usage.

<video width="400" controls autoplay>
    <source src="./docs/physical_device_overview.MOV" type="video/mp4">
</video>

---

## Physical Device -- Jiaying Shen

#### Main idea:

The Arduino queries a PiCloud API to retrieve water data, and the servo pushes the syringe. The total amount of water used by the OPS for the day is displayed via a scale and NeoPixel lights.

#### Design flow:

<div style="display: flex; align-items: center;">
  <img src="https://raw.githubusercontent.com/casa0019-group5/casa0019-group5.github.io/refs/heads/main/PhysicalDevice/img/img1.png" width="600">
</div>

<b> Stage 1: Syringe attachment method </b>
Thanks to Andy for supplying us with the following materials: 10ml syringes, 10ml cylinder, and plastic tubing, as well as the 3D models for the syringe holder, and rack and pinion gears. Some modifications have been made to address project complications.

When designing the physical device, I tried three different methods. The first method was to use a U-shaped tube with a hole at the bottom connected to a syringe. By pushing or pulling the syringe with a servo, water was injected or extracted to display the water level. However, due to the principle of communicating vessels, the water level on both sides of the U-tube remained the same, making it impossible to accurately reflect the movement of the syringe. This approach was eventually abandoned.

The second method used a soft tube to connect the syringe to a measuring cylinder. By pushing the syringe, water was injected into the cylinder to indicate water usgae, and by pulling back, water was withdrawn. However, during testing, it was found that some water remained in the cylinder after each reset, causing it to build up over time, leading to inaccurate readings and water loss. As a result, this method was not adopted.

The final method successfully implemented was to connect two syringes with a plastic tube to form a sealed hydraulic system. Squeezing one syringe forced water into the other, moving the plunger to indicate water consumption. This solution prevented water residue, allowed full water recirculation and ensured accurate resets, providing a relatively stable and accurate measurement system.

<b> stage2: Servo selection and syringe connection </b>
As the syringe is filled with water, the servo needs sufficient torque to push the plunger. Precise control of the scale is required, so the servo must have a minimum rotation angle of 180 degrees.

# Servo Comparison Table

| Servo Model | Torque (kg·cm) | Rotation Angle (degrees) | Voltage (Volt) |
| ----------- | -------------- | ------------------------ | -------------- |
| SG90        | 1.6            | 360                      | 4.8-6.0        |
| MG90S       | 1.8            | 180                      | 4.8-6.0        |
| TS90        | 1.8            | 180                      | 3.3-5.0        |
| Geek Servo  | 2              | 270                      | 3.0-5.0        |

After comparison and testing, some servos were unable to rotate a full 180 degrees, preventing the syringe from fully extending or resetting to zero. Servos with 180 degree rotation offer better precision and control compared to 360 degree servos. As a result, the Geek servo was ultimately chosen.

<b> Stage 3 : Circuit connection </b>

<div style="display: flex; align-items: center;">
  <img src="https://raw.githubusercontent.com/casa0019-group5/casa0019-group5.github.io/refs/heads/main/PhysicalDevice/img/flowchart.png" width="600">
</div>

<div style="display: flex; align-items: center;">
  <img src="https://raw.githubusercontent.com/casa0019-group5/casa0019-group5.github.io/refs/heads/main/PhysicalDevice/img/circuit.png" width="600">
</div>

The Arduino controls the syringe and NeoPixel light to indicate water usage. First, the system connects to WiFi and initializes. Then, data is fetched from the Raspberry Pi and processed to map it to the position of the syringe servo and the state of the NeoPixel light. If the data is less than 500, the system resets the syringe piston to its initial position, otherwise it continues to cycle through data acquisition and processing to maintain a real-time response state.

<b> Stage 4 : Enclosure and test </b>  
A laser cutter was used to make holes in a single wooden board to hold both the syringe and the servo. The sizes and positions of the holes were determined through multiple tests to ensure that the servo could smoothly push and pull the syringe along the scale, reaching both maximum and minimum values. As the servo and NeoPixel are powered by Arduino, the entire circuit was placed in a box under the wooden board to maintain a clean appearance and avoid interference with the power supply. The box is secured with interlocking edges for stability and is easy to open and close, allowing quick access to the Arduino MKR1010's reset button for efficient troubleshooting and maintenance without dismantling the entire system.

#### Design Optimization and Future Improvements

During the design process, several areas for optimisation and improvement were identified to improve both functionality and reliability.

One potential improvement is the use of coloured liquid inside the syringe to provide clearer and more intuitive visual feedback on water usage. This would make it easier to read the syringe's scale and observe the movement of the plunger, improving the user experience. In addition, the use of larger syringes would allow for greater measurement capacity, reducing the need for frequent resets and enabling the system to handle larger volumes of water.

To ensure stability and precision, an advanced holder could be designed to securely hold the servo and its gears. This would help prevent unwanted movement or misalignment. Occasionally, rapid servo movement caused by miscalculation can cause the gears to slip or pop out. A more secure holder design would reduce this risk by holding the gears firmly in place, even during sudden movements.

Another area for improvement is the Arduino MKR1010's network connection, which can sometimes be unstable. To address this, the box design can incorporate an external button connected to the Arduino's reset pin, allowing quick manual resets without opening the case. This ensures that troubleshooting and maintenance can be carried out efficiently while protecting the internal components.

https://github.com/user-attachments/assets/a4a8edd3-a0f4-40e1-bce6-6a52c5a1cce4.mp4

---

## MQTT to Raspberry Pi Database -- Vineeth Kirandumkara

One of the main problems encountered with the data stream was that the values we recieved was the total amount of water used since the time the sensors were powered on. This meant for example we couldn't display specific data such as data from the last 24 hours. To solve this, I created a javascript (`collect_water.js`) that ran every 10 minutes, to store the values from the MQTT stream in a SQL database along with a timestamp, the floor number and SAC. Using this database, I created an API (`server.js`) with 6 endpoints:

| Endpoint           | Description                     | Output                                                               |
| ------------------ | ------------------------------- | -------------------------------------------------------------------- |
| `/total/floor/:id` | Current Total Usage for a Floor | `{<LATEST TOTAL USAGE SAC1 + SAC2>}`                                 |
| `/total/SAC`       | Current Total Usage by SAC      | `{sac1: <LATEST TOTAL USAGE SAC1>, sac2: <LATEST TOTAL USAGE SAC2>}` |
| `/total/SAC/:id`   | Current Total Usage for an SAC  | `{<TOTAL USAGE FOR :id SAC>}`                                        |
| `/today/floor/:id` | Todays Usage for a Floor        | `{:id, <USAGE FOR :id OVER THE CURRENT DAY}`                         |
| `/today/SAC`       | The Difference in Today's Usage | `{<THE DIFFERENCE BETWEEN USAGE OF SAC1 AND SAC2}`                   |
| `/today/SAC/:id`   | Todays Usage by SAC             | `{:id, <TODAYS USAGE BY SAC>}`                                       |

## Backend Setup

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

---

## Design of Dashboard

![Dashboard](./docs/Dashboard.png)

When establishing the database, we found that the data we could obtain were the water consumption of the east and west sides of each floor and the total water consumption of the floor. Since it contains data on the water consumption of 16 floors from the 4th floor to the 19th floor, we didn't find a suitable table to display so many data at the same time. Therefore, we chose to use the form of numbers to present the data of the total water consumption of each layer more intuitively. In addition to the longitudinal comparison of each layer, we also used the data of different water consumption on the east and west sides of each layer. The bar chart on the dashboard also gives users a clear picture of the difference in water consumption between the east and west sides of the building. Because we chose to display daily water consumption data, it is necessary to display the day of the day. At the same time, we also add some explanatory text, such as unit, title, etc., to help users understand this dashboard. Since we are analyzing the use of water resources, we chose to use the cute image of a kitten to call on everyone to save water together.

## AR of Dashboard

![AR Dashboard](./docs/AR%20Dashboard.jpg)

First, we considered the possibility of bringing the entire dashboard to the screen just by scanning the logo on the physical device. Therefore, we chose to use the AR Tracked Image Manager script and put a photo of the logo in it. However, we found that the image was not always accurately recognized, so the dashboard sometimes did not appear as expected. So we added the Tap To Place script. This way, even if the image is not recognized, we can tap the screen to make the dashboard appear.

#### Reference

1. https://clipart-library.com/clip-art/syringe-clipart-transparent-21.htm
2. https://www.freepik.com/icon/servo_6276775
3. https://clipart-library.com/clip-art/366-3666355_arduino-vector-black-and-white-arduino-icon.htm
4. https://components101.com/motors/mg90s-metal-gear-servo-motor
5. https://bc-robotics.com/shop/micro-servo-sg90-360-degrees/
6. https://www.ebay.com/itm/225446283692?msockid=18393fb6b706665c3be02abfb6c067fb
7. https://forum.fritzing.org/t/pine-a64-esp8266-12e-f-arduino-zero-and-mkr1000-new-fritzing-parts/1611
8. https://github.com/adafruit/Fritzing-Library/blob/master/parts/Adafruit%20NeoPixel%20Stick.fzpz package for fritzing Neopixel8
