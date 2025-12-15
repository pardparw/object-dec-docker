using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.IO;
using System;



public class ObjectDec : MonoBehaviour
{
    // Start is called before the first frame update
    public RawImage rawImage;   // Unity RawImage to display the processed image
    private WebCamTexture webcamTexture;
    private TcpClient client;
    private NetworkStream stream;
    private BinaryWriter writer;
    private BinaryReader reader;




    public string serverAddress = "127.0.0.1";
    public int serverPort = 5000;

    Texture2D frame;
    Texture2D processedTexture;
    private void Awake()
    {
        // Process.Start("E:/DEC/run.bat");

    }

    void Start()
    {


        // Start the webcam
        WebCamDevice[] devices = WebCamTexture.devices;
        webcamTexture = new WebCamTexture(devices[0].name);
        processedTexture = new Texture2D(2, 2);
        rawImage.texture = webcamTexture;
        webcamTexture.Play();
        frame = new Texture2D(webcamTexture.width, webcamTexture.height);


        // Connect to the Python server
        client = new TcpClient(serverAddress, serverPort);
        stream = client.GetStream();
        writer = new BinaryWriter(stream);
        reader = new BinaryReader(stream);
    }

    void Update()
    {

        // Proceed with the rest of the Update() method

        if (webcamTexture.isPlaying)
        {

            frame.SetPixels(webcamTexture.GetPixels());
            frame.Apply();

            // Convert frame to JPG byte array
            byte[] bytes = frame.EncodeToJPG();

            if (writer != null)
            {
                writer.Write(bytes.Length);   // Write the length of the frame data
                writer.Write(bytes);          // Send the frame data
                writer.Flush();
            }
            else
            {
                // Debug.LogError("Writer is not initialized.");
                return;
            }

            // Wait for processed frame from Python
            try
            {
                if (reader != null)
                {


                    int processedLength = reader.ReadInt32();
                    byte[] processedData = reader.ReadBytes(processedLength);
                    // processedTexture= new Texture2D(2, 2);

                    // Convert the received byte array back to a texture
                    // processedTexture = new Texture2D(2, 2);
                    processedTexture.LoadImage(processedData);
                   
                    // rawImage.texture = texture;


                    processedTexture.Apply();
                    rawImage.texture = processedTexture;
                    // Destroy(frame);

                }
                else
                {
                    UnityEngine.Debug.LogError("Reader is not initialized.");
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Error while reading processed data: {ex.Message}");
            }

            // Optionally, handle received detection results (landmarks, etc.)

        }
    }
   

    void OnApplicationQuit()
    {
        // Clean up and close connections
        writer.Close();
        reader.Close();
        stream.Close();
        client.Close();
    }

}
