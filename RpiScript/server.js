/**
 * server.js
 * Description: Server file, contains all the endpoints for users to access the water_table
 * Author: Vineeth Kirandumkara
 * Notes:
 */
import mariadb from 'mariadb';
import express from 'express';
import 'dotenv/config';

// Database configuration
const dbConfig = {
    host: process.env.DB_HOST,
    user: process.env.DB_USERNAME,
    password: process.env.DB_PASSWORD,
    database: process.env.DB_NAME,
};
  
// Database and Table Consts
const databaseName = process.env.DB_NAME;
const tableName = process.env.DB_TABLE;

// Global variable to access dbConnection
let pool;

// Setup Express App
    // TODO: Figure out how to set this to a specific IP Address
const app = express();
const port = 3000;

// Open MariaDB
async function startDatabase() {
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

        if (pool)
            pool.end();
    }
}

await startDatabase();
console.log("Database initialized.");

/**
 * ENDPOINTS
 */

/**
 * Testing endpoint
 */
app.get('/', (req, res) => {
    res.send('Hello, World!');
});

// Endpoint to get the total water usage for a specific Floor Num
app.get('/total/floor/:id', async (req, res) => {
    const floorId = req.params.id;

    // Check input
    if(!floorId || isNaN(floorId)){
        return res.status(400).json({
            error: 'Not a Number'
        });
    }

    if((floorId >= 20) || (floorId < 4)){
        return res.status(400).json({
            error: 'Invalid Floor Number'
        });
    }

    try{
        // Grab a Connection from the pool
        const connection = await pool.getConnection();

        // Create the SQL Query
        const query = `
            SELECT SUM(value) AS total_value
            FROM ${tableName}
            WHERE floor = ${floorId}
                AND timestamp = (
                    SELECT MAX(timestamp)
                    FROM ${tableName}
                    WHERE floor = ${floorId}
                );`;

        const sum = await connection.query(query);
        const total = sum[0]?.total_value || 0;

        connection.release();

        console.log(`FloorNum: ${floorId}, Sum: ${total}`);
        res.json(total);
    } catch(error){
        console.error('Error fetching data: ', error);
        res.status(500).json({
            error: 'An Error occurred when fetching data.'
        });
    }
});

// Gets the total water usage for a specific SAC ID
app.get('/total/SAC/:id', async (req, res) => {
    const sacId = req.params.id;

    // Check input
    if(!sacId || isNaN(sacId)){
        return res.status(400).json({
            error: 'Not a Number'
        });
    }

    if((sacId > 2) || (sacId < 1)){
        return res.status(400).json({
            error: 'Invalid SAC Number'
        });
    }

    try{
        // Grab a Connection from the pool
        const connection = await pool.getConnection();

        // Create the SQL Query
        const query = `
            SELECT SUM(value) AS total_value
            FROM ${tableName}
            WHERE side = ${sacId}
                AND timestamp = (
                    SELECT MAX(timestamp)
                    FROM ${tableName}
                    WHERE side = ${sacId}
                );`;

        const sum = await connection.query(query);
        const total = sum[0]?.total_value || 0;

        connection.release();

        console.log(`SACNum: ${sacId}, Sum: ${total}`);
        res.json(total);
    } catch(error){
        console.error('Error fetching data: ', error);
        res.status(500).json({
            error: 'An Error occurred when fetching data.'
        });
    }
});

app.get('/today/floor/:id', async (req, res) => {
    const floorId = req.params.id;

    // Check input
    if(!floorId || isNaN(floorId)){
        return res.status(400).json({
            error: 'Not a Number'
        });
    }

    if((floorId >= 20) || (floorId < 4)){
        return res.status(400).json({
            error: 'Invalid Floor Number'
        });
    }

    try{
        const connection = await pool.getConnection();

        // Query to find the latest and midnight sums
        const query = `
          SELECT 
              (latest.sum_value - midnight.sum_value) AS value_difference
          FROM 
              (SELECT 
                   SUM(value) AS sum_value
               FROM ${tableName}
               WHERE floor = ${floorId}
                 AND timestamp = (SELECT MAX(timestamp) FROM ${tableName} WHERE floor = ${floorId})
              ) AS latest,
              (SELECT 
                   SUM(value) AS sum_value
               FROM ${tableName}
               WHERE floor = ${floorId}
                 AND DATE(timestamp) = CURDATE()
                 AND TIME(timestamp) BETWEEN '00:00:00' AND '00:05:00'
              ) AS midnight;
        `;
    
        // Execute the query
        const rows = await connection.query(query);
    
        // Close the connection
        connection.release();
    
        // Return the result
        const valueDifference = rows[0]?.value_difference || 0;
        res.json({ floorId, valueDifference });
    }catch(error){
        console.error('Error fetching data: ', error);
        res.status(500).json({
            error: 'An Error occurred when fetching data.'
        });
    }
});

app.get('/today/SAC/:id', async (req, res) => {
    const sacId = req.params.id;


});

app.listen(port, () => {
    console.log(`Server running at http://localhost:${port}`);
});