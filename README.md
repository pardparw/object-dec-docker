# Object Detection Docker Server

This project runs a YOLO-based object detection server inside a Docker container. It communicates with clients (like Unity) via TCP for images and UDP for results.

## Prerequisites

1.  **Docker Desktop**: Install Docker Desktop for your OS (Windows, Mac, or Linux).
    *   [Download Docker Desktop](https://www.docker.com/products/docker-desktop/)

## Setup & Run

1.  **Clone/Copy** this folder to your new PC.
2.  Open a terminal inside this folder.
3.  Run the application:

    ```bash
    docker compose up --build
    ```

    *   This command builds the image and starts the container.
    *   It listens on **TCP Port 5001** (default).

## Configuration

You can change settings using environment variables:

| Variable | Default Value | Description |
| :--- | :--- | :--- |
| `PORT` | `5001` | The TCP port to listen on. |
| `TARGET_IP` | `host.docker.internal` | The IP to send UDP results to (usually your host PC). |

### Changing the Port
To run on a different port (e.g., 6000):

```bash
PORT=6000 docker compose up
```

## Troubleshooting

*   **Ultralytics Settings Stuck**: If the logs hang at "Creating settings", just wait a moment. The `Dockerfile` has fixes included (`PYTHONUNBUFFERED=1`) to prevent actual hangs.

## Move to Another PC (Offline Method)

If you cannot build on the new PC (e.g., "Error finding image" or no internet), use this method to copy the working setup from your current PC.

### 1. On THIS PC (Where it works)
Save the docker image to a file:

```bash
# 1. Build the image first to make sure it's latest
docker compose build

# 2. Save it to a file
docker save -o object-dec-app.tar object-dec-docker-app
```

Copy the file `object-dec-app.tar` to your USB drive or transfer it to the new PC.

### 2. On the NEW PC
Load the image and run:

```bash
# 1. Load the image
docker load -i object-dec-app.tar

# 2. Run it (using the image name loaded, usually object-dec-docker-app)
docker run -p 5001:5001 -e TARGET_IP=host.docker.internal object-dec-docker-app
```
