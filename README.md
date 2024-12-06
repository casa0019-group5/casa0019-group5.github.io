# casa0019-group5.github.io

End of term project for CASA0019. This project tracks and visualizes the water consumption of One Pool Street residences using a physical device, and it's digital twin.

## RaspberryPi Setup

### Install

1. Pull the RpiScript folder onto the raspberry pi.
   It should include:

- `.env` : Contains all URLs and User/Pass
- `collect_water.js` : Script to update the database.
- `server.js` : Script that contains the API
- `package.json` : Contains all the Node libraries required.
- `package-lock.json` : ???

2. Run `npm install` to install all the required node packages listed in `package.json`.

3. Install mariadb-server using `sudo apt install mariadb-server`

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
