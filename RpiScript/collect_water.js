/**
 * collect_water.js
 * Description: Script that runs repeatedly to update the database with historical data from the OPS MQTT Stream
 * Author: Vineeth Kirandumkara
 * Notes: 
 */
import * as mqtt from "mqtt";
import mariadb from 'mariadb';
import 'dotenv/config';

// MQTT Topics
const mqtt_url = process.env.MQTT_URL;
const base_topic = process.env.BASE_TOPIC;

const mqtt_options = {
  username: process.env.MQTT_USERNAME,
  password: process.env.MQTT_PASSWORD,
};

// Connect MQTT Stream
let mqtt_client;

// Keeps track on all floors
const minFloor = 4;
const maxFloor = 20;
const arrSize = 32;

// Track the different topics, and if they have been updated or not
const value_arr = new Array(arrSize).fill(0);

// Database configuration
const dbConfig = {
  host: process.env.DB_HOST,
  user: process.env.DB_USERNAME,
  password: process.env.DB_PASSWORD,
};

// Database and Table Consts
const databaseName = process.env.DB_NAME;
const tableName = process.env.DB_TABLE;

// Global variable to access dbConnection
let pool;

// Create or connect to the database
async function setupDatabase() {
  try {
    // Create a connection pool
    pool = mariadb.createPool({
      ...dbConfig,
      connectionLimit: 5,
    });

    const connection = await pool.getConnection();
    console.log("Connected to MariaDB server.");

    // Check if the database exists
    const databases = await connection.query('SHOW DATABASES LIKE ?', [databaseName]);
    if (databases.length === 0) {
      console.log(`Database "${databaseName}" does not exist. Creating it...`);
      await connection.query(`CREATE DATABASE \`${databaseName}\``);
      console.log(`Database "${databaseName}" created.`);
    } else {
      console.log(`Database "${databaseName}" already exists.`);
    }

    // Connect to the specific database
    await connection.query(`USE \`${databaseName}\``);
    console.log(`Connected to database "${databaseName}".`);

    // Check if the table exists
    const tables = await connection.query('SHOW TABLES LIKE ?', [tableName]);
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

    connection.release();
  } catch (error) {
    console.error("Error setting up the database:", error.message);
    if (pool) pool.end();
  }
}

async function closeDatabase() {
  if (pool) {
    await pool.end();
    console.log("Connection pool closed.");
  }
}

async function updateDatabase() {
  try {
    const connection = await pool.getConnection();

    for (let i = 0; i < value_arr.length; i++) {
      const value = value_arr[i];
      const sac = (i % 2) ? 2 : 1;
      const floor = Math.floor(i / 2) + 4;
      const timestamp = new Date();

      // Insert data into table
      const query = `INSERT INTO ${tableName} (timestamp, floor, side, value) VALUES (?, ?, ?, ?)`;
      await connection.query(query, [timestamp, floor, sac, value]);
    }

    connection.release();
    await closeDatabase();
  } catch (err) {
    console.error("Error inserting data into database:", err);
  }
}

function checkReadArray() {
  if (value_arr.includes(0)) {
    console.log("Still updating...");
    const leftovers = findIndexes(value_arr, 0);
    console.log(`Leftovers: ${leftovers}`);
  } else {
    mqtt_client.end();
    console.log("All topics have been updated.");
    updateDatabase();
  }
}

function findIndexes(array, targetValue) {
  return array.reduce((indexes, value, index) => {
    if (value === targetValue) {
      indexes.push(index);
    }
    return indexes;
  }, []);
}

// Run the script
await setupDatabase();
console.log("Database Connected...");
mqtt_client = mqtt.connect(mqtt_url, mqtt_options);

// Connect to the MQTT Broker
mqtt_client.on("connect", () => {
  console.log("Connected to MQTT broker");

  // Subscribe to all the Floors and SACs
  mqtt_client.subscribe(base_topic, (err) => {
    if (err) {
      console.error("Failed to subscribe to topic", err);
    } else {
      console.log(`Subscribed to topic: ${base_topic}`);
    }
  });
  console.log("Topics have been set.");
});

// Handle new messages from MQTT
mqtt_client.on("message", (topic, message) => {
  const json_data = JSON.parse(message);

  // Strip topic down to floor and sac values
  let index = topic.lastIndexOf("/");
  topic = topic.slice(0, index);
  index = topic.lastIndexOf("/");
  topic = topic.slice(index);
  const floorNum = Number(topic.slice(3, 5)) - 4;
  index = topic.lastIndexOf("SAC");
  const sacNum = Number(topic.slice(index + 3)) - 1;

  // Check if the value is null, undefined, or an empty object
  if (json_data === null || json_data === undefined || typeof json_data === "object") {
    value_arr[floorNum * 2 + sacNum] = -1;
  } else {
    value_arr[floorNum * 2 + sacNum] = Number(json_data);
  }

  console.log("SAC:", sacNum + 1);
  console.log("Floor:", floorNum + 4);
  console.log("Arr_i:", floorNum * 2 + sacNum);
  console.log("Value:", value_arr[floorNum * 2 + sacNum]);

  checkReadArray();
});

mqtt_client.on("error", (err) => {
  console.error("MQTT client error:", err);
});