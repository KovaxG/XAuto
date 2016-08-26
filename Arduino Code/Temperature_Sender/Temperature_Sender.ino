/*
 * Arduino termal resistance reader and sender.
 * May not be very accurate, but it works fine.
 * by Kovacs Gyorgy
 * on 2016.08.23
 */

// Two points are defined in the initializeConstants function.
// If the temperature is off, you schould check there.
#define PERIOD        5000
#define ANALOGPIN     A0
#define BAUDRATE      9600
#define WAITFORSERIAL true

struct Point {
  int volt; // The value read from the pin of the arduino (0 - 1023)
  float temp; // The temperature at that reading.
};

float a, b; // Constants used in voltageToCelsius

/*
 * Initializes the constants used in the function 
 * voltageToCelsius. This is done every time the
 * sensor is changed in order to calibrate the 
 * sensor.
 */
void initializeConstants() {
  // \/---------------------------------------------------------Change these values here in order to calculate the constants!
  Point point1 = {508, 25.0}; // reading from analog pin, measured temperature
  Point point2 = {602, 33.8}; // voltage, celsius (int, float)

  /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
   *  Let f(x) be the function that converts a reading to a    *
   *  temperature: (Considering linearity for simplicity.)     *
   *                                                           *
   *             f :: Value -> Temperature                     *
   *             f(x) = a * x + b = y                          *
   *                                                           *
   *  Given two points, one can calculate a and b thusly:      *
   *          f(x1) = y1 and f(x2) = y2                        *
   *                                                           *
   *                   y2 - y1                                 *
   *              a = ---------                                * 
   *                   x2 - x1                                 *
   *                                                           *
   *              b = y1 - x1 * a                              *
   *                                                           *
   * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
  
  a = (point2.temp - point1.temp) / (point2.volt - point1.volt);
  b = point1.temp - (point1.volt * a);
} // End of initializeConstants

/*
 * This runs only once, when the script is uploaded
 * or when the microcontroller is reset.
 */
void setup() {
  
  initializeConstants(); // Calculate constants used in the voltageToCelsius function
  Serial.begin(BAUDRATE); // Begin sending serial data with the specified baud rate (kind of like frequency)
  if (WAITFORSERIAL) {
    while (!Serial) {} // Wait until the PC is ready to receive data
  }
} // End of setup

/*
 * A function that transforms the read voltage into a temperature 
 * based on a and b, the two constants calculated in the setup.
 */
float voltageToCelsius(int voltage) {
  return (a * voltage) + b;
} // End of voltageToCelsius

/*
 * This is executed ad infinitum. It reads a pin, 
 * converts it to celsius and transmits it via serial,
 * after which it stops for a given period.
 */
void loop() {
  int voltage = analogRead(ANALOGPIN);
  float temperature = voltageToCelsius(voltage);
  Serial.print(temperature); // Send that value to the PC
  Serial.println(" C");
  delay(PERIOD);
} // End of loop
