#include <Servo.h>
#include <Adafruit_NeoPixel.h>
#include <WiFiNINA.h>
// WiFi credentials
const char* ssid = "CE-Hub-Student";
const char* password = "casa-ce-gagarin-public-service";

// Raspberry Pi server details
const char* serverIP = "10.129.111.13";  // Replace with Raspberry Pi's IP address
const int serverPort = 3001;             // Replace with your server port

// NeoPixel and Servo configuration
#define PIXEL_PIN 1    // NeoPixel data pin
#define NUM_PIXELS 8   // Number of NeoPixel LEDs
#define BRIGHTNESS 50  // Global brightness (0-255)

Servo myServo; // Create a Servo object
Adafruit_NeoPixel strip(NUM_PIXELS, PIXEL_PIN, NEO_GRB + NEO_KHZ800);
bool isFetching = false;
int currentPosition = 1850; // Servo's initial position (10ml)
unsigned long previousMillis = 0; // Timer tracking for HTTP requests
int greenCount = 0;

WiFiClient client;

void setup() {
  // Servo initialization
  myServo.attach(2);
  myServo.writeMicroseconds(1850); // Initial position (10ml)
  delay(1000);

  // NeoPixel initialization
  strip.begin();
  strip.clear();
  strip.setBrightness(BRIGHTNESS);
  strip.show(); // Turn off all LEDs initially

  // WiFi connection
  Serial.begin(115200);
  connectWiFi();

  // Initial HTTP request to get data
  fetchDataFromRaspberryPi();
}

void loop() {
  unsigned long currentMillis = millis();

  // Send HTTP request every 10 minutes (600000 ms)
  if (currentMillis - previousMillis >= 600000 || previousMillis == 0) {
    previousMillis = currentMillis;
    fetchDataFromRaspberryPi();
  }
  Serial.print("current time");
  Serial.println(currentMillis);
}

// Function to connect to WiFi
void connectWiFi() {
  Serial.print("Connecting to WiFi...");
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.print(".");
  }
  Serial.println("\nConnected to WiFi");
  Serial.print("IP Address: ");
  Serial.println(WiFi.localIP());
}

void fetchDataFromRaspberryPi() {
  if (isFetching) {
    Serial.println("Already fetching, skipping...");
    return;
  }

  isFetching = true; // Set fetching flag

  if (client.connect(serverIP, serverPort)) {
    Serial.println("Connected to server");

    // Send HTTP GET request
    client.println("GET /today/sac HTTP/1.1");
    client.print("Host: ");
    client.println(serverIP);
    client.println("Connection: close");
    client.println();

    long parsedValue = 0;
    bool isParsing = false;
    long lastParsedValue = 0;

    while (client.available() || client.connected()) {
      char c = client.read();

      if (c >= '0' && c <= '9') {
        parsedValue = parsedValue * 10 + (c - '0');
        isParsing = true;
      } else if (isParsing) {
        lastParsedValue = parsedValue;
        parsedValue = 0;
        isParsing = false;
      }
    }

    if (isParsing) {
      lastParsedValue = parsedValue;
    }

    Serial.print("Final Parsed Data: ");
    Serial.println(lastParsedValue);
    processData(lastParsedValue);

    client.stop();
  } else {
    Serial.println("Failed to connect to server");
  }

  isFetching = false; // Reset fetching flag
}



// Function to process data and update LEDs and servo
void processData(int data) {
  // Map data to servo position
  int remain = data % 100;
  int targetPosition = map(remain, 0, 100, 1850, 500); // Map remainder to servo
  moveServo(targetPosition);

  // Update NeoPixel LEDs
  greenCount = data / 100; // Number of green LEDs to light up
  
  Serial.print("data: ");
  Serial.println(data);
  Serial.print("remain: ");
  Serial.println(remain);
  Serial.print("targetPosition: ");
  Serial.println(targetPosition);
  // Serial.print("greencount: ");
  // Serial.println(greenCount);
  updateNeoPixel(greenCount);
}

// Function to move servo smoothly
void moveServo(int targetPosition) {
  if (currentPosition >= 500 && currentPosition <= 520) {
    for (int pos = currentPosition; pos <= 1850; pos += 50) {
      myServo.writeMicroseconds(pos);
      delay(50);
    }
    greenCount++;
    updateNeoPixel(greenCount);
  } else {
    myServo.writeMicroseconds(targetPosition);
    delay(50);
  }
  currentPosition = targetPosition;
}

void updateNeoPixel(int greenCount) {
  // Determine the range of greenCount and light up the corresponding color
  if (greenCount >= 0 && greenCount <= 8) {
      // Light up green LEDs
      for (int i = 0; i < greenCount; i++) {
          strip.setPixelColor(i, strip.Color(0, 255, 0)); // Green
      }
  } else if (greenCount > 8 && greenCount <= 16) {
      // Light up yellow LEDs
      for (int i = 0; i < greenCount - 8; i++) {
          strip.setPixelColor(i, strip.Color(255, 255, 0)); // Yellow
      }
  } else if (greenCount > 16 && greenCount <= 24) {
      // Light up blue LEDs
      for (int i = 0; i < greenCount - 16; i++) {
          strip.setPixelColor(i, strip.Color(0, 0, 255)); // Blue
      }
  } else if (greenCount > 24 && greenCount < 32){
       for (int i = 0; i < greenCount - 16; i++) {
          strip.setPixelColor(i, strip.Color(255, 0, 0)); // Red
      }
  } else {
    strip.rainbow();
  }

  // Refresh the LED display
  strip.show();
}


