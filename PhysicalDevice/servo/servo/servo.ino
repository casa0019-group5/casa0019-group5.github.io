#include <Servo.h>

Servo myServo; // Create a servo object

void setup() {
  myServo.attach(2);  // Connect the servo signal pin to digital pin 2
  myServo.write(0);   // Initialize the position to 0°
  delay(1000);        // Wait for the servo to reach the initial position
}

void loop() {
  // Step 1: Rotate 180° clockwise
  for (int angle = 0; angle <= 180; angle += 36) { // Increment angle by 10° each time
    myServo.write(angle); // Set the current angle
    delay(1000);           // Delay to ensure smooth movement
  }

  delay(1000); // Stay at 180° for a moment

  // Step 2: Rotate 180° counterclockwise
  for (int angle = 180; angle >= 0; angle -= 36) { // Decrement angle by 10° each time
    myServo.write(angle); // Set the current angle
    delay(1000);           // Delay to ensure smooth movement
  }

  delay(1000); // Stay at 0° for a moment

  // Repeat the above steps
}
