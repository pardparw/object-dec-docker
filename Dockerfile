FROM python:3.9-slim

WORKDIR /app

# 1. Install system dependencies (OpenCV fix)
RUN apt-get update && apt-get install -y \
    libgl1 \
    libglib2.0-0 \
    && rm -rf /var/lib/apt/lists/*

# 2. COPY requirements.txt FIRST (Critical Step)
COPY requirements.txt .
COPY yolo11s.pt .
COPY labels.csv .

# 3. THEN run pip install
RUN pip install --no-cache-dir -r requirements.txt

# 4. FINALLY copy the rest of your code
COPY . .




EXPOSE 5001
ENV TARGET_IP=host.docker.internal
ENV PYTHONUNBUFFERED=1
ENV PORT=5001

# Initialize Ultralytics settings during build to prevent runtime hangs
RUN yolo settings version_check=False 

CMD ["python", "main.py"]