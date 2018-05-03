/*
    Based on Neil Kolban example for IDF: https://github.com/nkolban/esp32-snippets/blob/master/cpp_utils/tests/BLE%20Tests/SampleServer.cpp
    Ported to Arduino ESP32 by Evandro Copercini
*/

#include <BLEDevice.h>
#include <BLEUtils.h>
#include <BLEServer.h>

// See the following for generating UUIDs:
// https://www.uuidgenerator.net/

#define SERVICE_UUID        "4fafc201-1fb5-459e-8fcc-c5c9c331914b"
#define CHARACTERISTIC_UUID "beb5483e-36e1-4688-b7f5-ea07361b26a8"

class MyCallbacks: public BLECharacteristicCallbacks {
    void onWrite(BLECharacteristic *pCharacteristic) {
      std::string value = pCharacteristic->getValue();

     /* if(value.length()> 0) {
            for (int i = 0; i < value.length(); i++)
              Serial.print(value[i]);
        }*/
      digitalWrite(2,1);
      delay(1000);
      digitalWrite(2,0);
    }
};

void setup() {
  //Serial.begin(115200);
  pinMode(0, INPUT_PULLUP);
  pinMode(2, OUTPUT);
  BLEDevice::init("Gwintech-Dev");
  BLEServer *pServer = BLEDevice::createServer();
  BLEService *pService = pServer->createService(SERVICE_UUID);
  BLECharacteristic *pCharacteristic = pService->createCharacteristic(
                                         CHARACTERISTIC_UUID,
                                         BLECharacteristic::PROPERTY_READ |
                                         BLECharacteristic::PROPERTY_WRITE|
                                         BLECharacteristic::PROPERTY_NOTIFY 
                                       );

  pCharacteristic->setValue("This is a test");
  pCharacteristic->setCallbacks(new MyCallbacks());
  pService->start();
  BLEAdvertising *pAdvertising = pServer->getAdvertising();
  pAdvertising->start();
  
}

void loop() {
  // put your main code here, to run repeatedly:
      static uint8_t lastPinState = 1;
    uint8_t pinState = digitalRead(0);
    if(!pinState && lastPinState){
        Serial.println("Button Pressed");
        
    }
    lastPinState = pinState;
    while(Serial.available()) Serial.write(Serial.read());
}
