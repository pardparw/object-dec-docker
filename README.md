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

*   **"Waiting for connection..."**: The server is ready. Connect your client (Unity) to the local IP of the machine running Docker (or `localhost` if on the same machine) on port `5001`.
*   **Ultralytics Settings Stuck**: If the logs hang at "Creating settings", just wait a moment. The `Dockerfile` has fixes included (`PYTHONUNBUFFERED=1`) to prevent actual hangs.
