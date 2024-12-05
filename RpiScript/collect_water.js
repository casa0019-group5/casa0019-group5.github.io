/**
 * collect_water.js
 * Description: Script that runs repeatedly to update the database with historical data from the OPS MQTT Stream
 * Author: Vineeth Kirandumkara
 * Notes: 
 */

//import { createConnection, execute } from 'mysql2/promise';
import * as mqtt from "mqtt";
import pkg from 'mysql2/promise';
const { createConnection, execute } = pkg; // Destructure what you need

// MQTT Topics
const mqtt_url = "mqtt://mqtt.cetools.org:1883";
const base_topic = "UCL/OPSEBOAS/PSW TW20-XX-CE-001/BACnet Interface/Application/Energy Monitoring/Water Meters/";

// Connect MQTT Stream
let mqtt_client;

// Track the different topics, and if they have been updated or not
const topic_array = [];
const read_array = [];

// Keeps track on all floors
const minFloor = 4;
const maxFloor = 20;

// Database configuration
const dbConfig = {
  host: 'localhost',
  user: 'root',
  password: 'testing123', // Replace with your MySQL password
};

// Database and Table Consts
const databaseName = 'water_db'; // Replace with your desired database name
const tableName = 'water_table'; // Replace with your desired table name

// Global Var to access dbConnection
let connection;

let updateFlag = false;

// Create all the topics
async function startUp(){

  console.log("Adding Topics");

  // Iterate through the floors
  for(let i = minFloor; i < maxFloor; i++){

    // Iterate through the SACs
    for(let j = 1; j <= 2; j++){

      // Build the topic
      const topic = (i < 10) ? `${base_topic}TW0${i}-01-CE-001 BoostCWSMtrSAC${j}` : `${base_topic}TW${i}-01-CE-001 BoostCWSMtrSAC${j}`;
      console.log(`${topic}`);
      topic_array.push(topic);
      read_array.push(0);
    }
  }
}

// Create or connect to the database
async function setupDatabase() {

  try {
    // Step 1: Connect to MySQL server
    connection = await createConnection(dbConfig);
    console.log("Connected to MySQL server.");

    // Step 2: Check if the database exists
    const [databases] = await connection.query('SHOW DATABASES LIKE ?', [databaseName]);
    if (databases.length === 0) {
      console.log(`Database "${databaseName}" does not exist. Creating it...`);
      await connection.query(`CREATE DATABASE \`${databaseName}\``);
      console.log(`Database "${databaseName}" created.`);
    } else {
      console.log(`Database "${databaseName}" already exists.`);
    }

    // Step 3: Connect to the specific database
    await connection.changeUser({ database: databaseName });
    console.log(`Connected to database "${databaseName}".`);

    // Step 4: Check if the table exists
    const [tables] = await connection.query('SHOW TABLES LIKE ?', [tableName]);

    if (tables.length === 0) {
      console.log(`Table "${tableName}" does not exist. Creating it...`);
      await connection.query(`
        CREATE TABLE \`${tableName}\` (
          \`timestamp\` DATETIME DEFAULT CURRENT_TIMESTAMP,
          \`floor\` INT NOT NULL,
          \`side\` VARCHAR(50) NOT NULL,
          \`value\` FLOAT NOT NULL
        )
      `);
      console.log(`Table "${tableName}" created.`);
    } else {
      console.log(`Table "${tableName}" already exists.`);
    }

  } catch (error) {
    console.error("Error setting up the database:", error.message);
  }
}

async function closeDatabase(){
  // Step 5: Close the connection
  if (connection) {
    await connection.end();
    console.log("Connection closed.");
  }
}

function updateDatabase(data, floorSpec){

  const {value} = data;
  const timestamp = new Date();
  const floor = Math.floor(floorSpec / 2) + minFloor;  // Floor Num
  const side = floorSpec % 2;   // SAC

  // Insert data into table
  const query = 'INSERT INTO tableName (timestamp, floor, side, value) VALUES (?, ?, ?, ?)';
  execute(query, [timestamp, floor, side, value], (err, results) => {
    if(err){
      console.error('Error inserting data into mySQL: ', err);
    }
    else{
      console.log('Data added to database: ', results);
      read_array[floorSpec] = 1;  // Track which floors have gotten their update
    }
  });
}

function checkReadArray(){
  if(read_array.includes(0)){
    console.log("still updating...");
  }
  else{
    updateFlag = true;
    mqtt_client.end();
    console.log("All topics have been updated.");
  }
}

// Run the script
await startUp();
await setupDatabase();
console.log("Database Connected...");
mqtt_client = await mqtt.connect(mqtt_url);
console.log("finished connect?");

// Connect to the MQTT Broker
mqtt_client.on("connect", () => {
  console.log('Connected to MQTT broker');

  // Subscribe to all the Floors and SACs
  for(let i = 0; i < topic_array.length; i++){
    mqtt_client.subscribe(topic_array[i], err => {
      if(err){
        console.error('Failed to subscribe to topic', err);
      }
      else{
        console.log(`Subscribed to topic: ${topic_array[i]}`);
      }
    });
  }
  console.log("topics have been set.");
});

// Handle new messages from MQTT
mqtt_client.on('message', (topic, message) => {
  console.log("New Message!");
  for(let i = 0; i < topic_array.length; i++){
    if(topic == topic_array[i]){
      // Found the right topic
      console.log(`Topic: ${topic}`);
      // Parse message
      const data = JSON.parse(message);
      console.log(`Received Data: ${data}`);
      updateDatabase(data, i);
      checkReadArray();
      break;
    }
  }
});

mqtt_client.on('error', (err) => {
  console.error('MQTT client error:', err);
});