# casa0019-group5.github.io

End of term project for CASA0019. This project tracks and visualizes the water consumption of One Pool Street residences using a physical device, and it's digital twin.

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
