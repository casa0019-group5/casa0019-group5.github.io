'''
WaterHistory App
Description: Takes a reading at midnight every night to serve as a basis for the Physical Device to display the water usage throughout the day.
Name: Vineeth Kirandumkara
Date: 3/12/24

Notes:
Connect and Disconnect code provided by:
https://www.emqx.com/en/blog/how-to-use-mqtt-in-python
'''

import logging
import random
import time
from paho.mqtt import client as mqtt_client

broker = "mqtt.cetools.org"
port = 1884
topic = "UCL/OPSEBOAS/PSW TW20-XX-CE-001/BACnet Interface/Application/Energy Monitoring/Water Meters"
client_id = f'python-mqtt-{random.randint(0, 1000)}'
file = "/RpiScript/cred.txt"
username = ""
password = ""
FIRST_RECONNECT_DELAY = 1
RECONNECT_RATE = 2
MAX_RECONNECT_COUNT = 12
MAX_RECONNECT_DELAY = 60

delay = 0.5

def run():
    client = connect_mqtt()
    client.loop_start()
    christmasTree(client)
    client.loop_stop()

def connect_mqtt():
    def on_connect(client, userdata, flags, rc):
        if rc == 0:
            print("Connected to MQTT Broker!")
        else:
            print("Failed to connect, return code %d\n", rc)

    client = mqtt_client.Client(client_id)
    client.username_pw_set(username, password)
    client.on_connect = on_connect
    client.connect(broker, port)
    return client

def on_disconnect(client, userdata, rc):
    logging.info("Disconnected with result code: %s", rc)
    reconnect_count, reconnect_delay = 0, FIRST_RECONNECT_DELAY
    while reconnect_count < MAX_RECONNECT_COUNT:
        logging.info("Reconnecting in %d seconds...", reconnect_delay)
        time.sleep(reconnect_delay)

        try:
            client.reconnect()
            logging.info("Reconnected successfully!")
            return
        except Exception as err:
            logging.error("%s. Reconnect failed. Retrying...", err)

        reconnect_delay *= RECONNECT_RATE
        reconnect_delay = min(reconnect_delay, MAX_RECONNECT_DELAY)
        reconnect_count += 1
    logging.info("Reconnect failed after %s attempts. Exiting...", reconnect_count)

def subscribe(client: mqtt_client):
    def on_message(client, userdata, msg):
        print(f"Received `{msg.payload.decode()}` from `{msg.topic}` topic")

    client.subscribe(topic)
    client.on_message = on_message

if __name__ == '__main__':
    print("Starting LightWall App...")

    # Pull MQTT username and password
    doc = open(file, "r")
    username = doc.readline().strip()
    password = doc.readline().strip()
    print("username: " + username)
    print("password: " + password)
    doc.close()
    run()