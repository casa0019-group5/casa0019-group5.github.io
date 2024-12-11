# CASAMIGOS

<img src="https://github.com/casa0019-group5/casa0019-group5.github.io/blob/servo/PhysicalDevice/src/physical%20device.jpg?raw=true" alt="Image">

## Why did we choose water data?

After we received the initial brief and theme, we started digging for interesting datasets linked to UCL. We then discovered some interesting operational building data about One Pool Street. It was a live MQTT stream of water meter data in One Pool Street since the building first opened. The topic is, UCL/OPSEBOAS/PSW TW20-XX-CE-001/BACnet Interface/Application/Energy Monitoring/Water Meters, for anyone interested in exploring this data themselves! The Water Meter data was only for one tower of One Pool Street and it was split into two sections of the one tower, SAC1 and SAC2. In addition, it showed us data from Floors 4 to 19. Therefore, with SAC1 and SAC2 combined we had 30 individual water meter readings.

## Initial Brainstorm

We then had to decide how we wanted to represent this data. After some brainstorming we were all in agreement that an old-fashioned water gauge was out of the picture, we wanted something a little more unique. Taking inspiration from one of Andy's projects where he had used a syringe to visualise, we took his model of the syringe and repurposed it to show the water usage data, through the syringe pushing water up the device to demonstrate the total daily water usage.

<video src="./docs/physical_device_overview.mp4" controls="controls" style="max-width: 730px;">
</video>

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
