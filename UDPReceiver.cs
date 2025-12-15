using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;



public class UDPReceiver : MonoBehaviour
{
    UdpClient udpClient;
    IPEndPoint remoteEndPoint;

    //Json
    [System.Serializable]
    public class Data
    {
        public string Class;
        public int x;
        public int y;
        public int w;
        public int h;

    }
    [System.Serializable]
    public class DataList
    {
        public Data[] data;
    }

    [System.Serializable]
    public class ListOfWord
    {
        public string key;
        public List<string> words;

    }

    [System.Serializable]

    public class Word
    {
        public List<ListOfWord> AllWord;
    }

    public DataList dataList = new DataList();
    public Word ListOfAllWords = new Word();

    //Find Words

    void Start()
    {
        udpClient = new UdpClient(5010);  // Listen on port 5010
        remoteEndPoint = new IPEndPoint(IPAddress.Any, 5010);

        // Start listening on a separate thread
        udpClient.BeginReceive(ReceiveData, null);



    }
    bool find;

    void ReceiveData(IAsyncResult result)
    {
        try
        {
            byte[] receivedData = udpClient.EndReceive(result, ref remoteEndPoint);
            string jsonString = Encoding.UTF8.GetString(receivedData);

            if (string.IsNullOrWhiteSpace(jsonString))  // Check for empty data
            {
                Debug.LogError("Received empty UDP message.");
                return;
            }
            // Debug.Log($"Received UDP Data: {jsonString}");  // Debug output
            try
            {
                dataList = JsonUtility.FromJson<DataList>(jsonString);
                if (dataList.data.Length > 0)
                {

                   

                    foreach (var values in dataList.data)
                    {

                        for (int i = 0; i < ListOfAllWords.AllWord.Count; i++)
                        {
                            if (ListOfAllWords.AllWord[i].key == values.Class)
                            {
                                if (ListOfAllWords.AllWord[i].words.Count < 5)
                                {
                                    ListOfAllWords.AllWord[i].words.Add(values.Class);
                                }

                                find = true;
                            }
                        }
                        if (!find)
                        {
                            ListOfAllWords.AllWord.Add(new ListOfWord { key = values.Class, words = new List<string> { values.Class } });
                        }

                        find = false;

                    }

                }


            }
            catch (Exception ex)
            {
                Debug.LogError($"JSON Parsing Error: {ex.Message}\nReceived Data: {jsonString}");
            }

            // Continue listening
            udpClient.BeginReceive(ReceiveData, null);
        }
        catch (Exception ex)
        {
            Debug.LogError($"UDP Error: {ex.Message}");
        }
    }




    void OnApplicationQuit()
    {
        udpClient.Close();
    }
}