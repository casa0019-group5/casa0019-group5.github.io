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

app.get('/', (req, res) => {
    res.send('Hello, World!');
});



app.listen(port, () => {
    console.log(`Server running at http://localhost:${port}`);
});