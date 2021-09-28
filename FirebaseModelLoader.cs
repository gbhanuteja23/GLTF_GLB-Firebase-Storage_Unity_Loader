//Script for loading 3D Models(GLTF) from Firebase into Unity at runtime using GLTF Model Loader

using System.Collections;
using System.Collections.Generic;
using System;
using System.IO; 
using UnityEngine;
using UnityEngine.Networking;
using Siccity.GLTFUtility;
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;

public class FirebaseModelLoader : MonoBehaviour
{
    string filePath;  

    GameObject spaceShip;   
    string url;               //URL of the 3D model stored in Firebase Storage 

    FirebaseStorage storage;
    StorageReference storageReference;
    StorageReference spaceShipModel; 

    // Start is called before the first frame update
    void Start()
    {
        //Application.persistDataPath stores the data at runtime to a specific location as per the platform
        //More details at:- https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html

        filePath = $"{Application.persistentDataPath}/Files/spaceShip.glb";

        spaceShip = new GameObject
        {
            name = "spaceShip"
        };

        //initialize storage reference
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("Your Firebase Storage Bucket URL");                                                                                                                                             

        spaceShipModel = storageReference.Child("spaceShip.glb"); 

    }

    // Update is called once per frame
    void Update()
    {
        
    }
   
    public void DownloadFile(string url)
    {
        string path = filePath;      //GetFilePath(url); 

        if(File.Exists(path))
        {
            Debug.Log("Found the same file locally, Loading!!!");

            LoadModel(path);

            return; 
        }

        StartCoroutine(GetFileRequest(url, (UnityWebRequest req) =>
        {
            if (req.isNetworkError || req.isHttpError)
            {
                //Logging any errors that may happen
                Debug.Log($"{req.error} : {req.downloadHandler.text}");
            }

            else
            {
                //Save the model fetched from firebase into spaceShip 
                LoadModel(path); 

            }
        }

        )); 
    }    

    string GetFilePath(string url)
    {
        string[] pieces = url.Split('/');
        string filename = pieces[pieces.Length - 1];

        return $"{filePath}{filename}"; 
    }

    void LoadModel(string path)
    {
        ResetSpaceShip();

        GameObject model = Importer.LoadFromFile(filePath);

        model.transform.SetParent(spaceShip.transform);
    }

    IEnumerator GetFileRequest(string url, Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.downloadHandler = new DownloadHandlerFile(filePath);

            yield return req.SendWebRequest(); 

            callback(req);

        }
    }

    void ResetSpaceShip()
    {
        if(spaceShip != null)
        {
            foreach(Transform trans in spaceShip.transform)
            {
                Destroy(trans.gameObject); 
            }
        }

    }

}
