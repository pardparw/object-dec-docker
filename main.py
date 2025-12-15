import socket
import struct
import cv2
import numpy as np
from ultralytics import YOLO
import pandas as pd
import json

model = YOLO("yolo11s.pt")#Auto Download
labels = pd.read_csv("labels.csv")
import os

# Get Target IP from environment variable (default for Docker Desktop on Mac/Windows)
TARGET_IP = os.getenv("TARGET_IP", "host.docker.internal")
print(f"Target IP for UDP: {TARGET_IP}")

# Get Port from environment variable (default 5001)
PORT = int(os.getenv("PORT", 5001))

# Create UDP socket for sending data (port 5010)
udp_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
udp_address = (TARGET_IP, 5010)  # Send to Unity on port 5010

# TCP Server for image transfer (dynamic port)
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.bind(("0.0.0.0", PORT))
server_socket.listen(1)


try:
    while True:
        print(f"Waiting for connection on port {PORT}...")
        client_socket, client_address = server_socket.accept()
        print(f"Connected to {client_address}")

        try:
            while True:
                # Receive Image Data from Unity
                length_data = client_socket.recv(4)
                if not length_data:
                    print("Client disconnected (Recv 0 bytes)")
                    break
                length = struct.unpack('I', length_data)[0]
                
                image_data = b""
                while len(image_data) < length:
                    packet = client_socket.recv(length - len(image_data))
                    if not packet:
                        break
                    image_data += packet
                
                if len(image_data) < length:
                    print("Incomplete image data, disconnecting...")
                    break

                # Convert the received image into OpenCV format
                np_arr = np.frombuffer(image_data, np.uint8)
                img = cv2.imdecode(np_arr, cv2.IMREAD_COLOR)

                if img is None:
                    print("Failed to decode image")
                    continue

                results = model(img ,show=False)  # predict on an image
                detection_results = {"data": []}
                for result in results:
                    for box in result.boxes:
                        x1, y1, x2, y2 = box.xyxy[0]  # Bounding box coordinates
                        conf = box.conf[0]  # Confidence score
                        cls = int(box.cls[0])  # Class ID
                        # print(f"Class: {cls}, Confidence: {conf:.2f}, Box: ({x1:.0f}, {y1:.0f}, {x2:.0f}, {y2:.0f})")
                        X1 = int(f'{x1:.0f}')
                        X2 = int(f'{x2:.0f}')
                        Y1 = int(f'{y1:.0f}')
                        Y2 = int(f'{y2:.0f}')
                            
                                        
                                
                        cv2.rectangle(img, (X1, Y1), (X2, Y2),(255, 0, 0), 2)
                        cv2.rectangle(img, (X1, (Y1-30)), ((X1+100), (Y1)),(255, 255, 255), -1)
                        cv2.putText(img, (f"{labels['LABELS'][cls]} {conf:.2f}"),(X1, (Y1-10)), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 180, 0), 1, cv2.LINE_AA)
                        detection_results["data"].append({"Class": (labels['LABELS'][cls]), "x": X1, "y": Y1, "w": X2, "h": Y2},)
                
                
            
                # Example Detection Results (Modify with YOLO detections)
            

            
                # Encode and send the processed image back to Unity
                # img = cv2.rotate(img, cv2.ROTATE_90_COUNTERCLOCKWISE) #For Mobile Camera
                _, img_encoded = cv2.imencode('.jpg', img)
                processed_data = img_encoded.tobytes()

                client_socket.send(struct.pack('I', len(processed_data)))
                client_socket.send(processed_data)
                # Convert results to JSON format and send via UDP
                if(len(detection_results) > 0):
                    json_data = json.dumps(detection_results)
                    udp_socket.sendto(json_data.encode(), udp_address)

        except Exception as e:
            print(f"Error during connection: {e}")
        finally:
            print(f"Closing connection to {client_address}")
            client_socket.close()

except KeyboardInterrupt:
    print("Server stopping...")
finally:
    server_socket.close()
    udp_socket.close()
    print("Sockets closed.")




