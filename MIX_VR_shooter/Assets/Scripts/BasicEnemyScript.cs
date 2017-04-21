using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyScript : MonoBehaviour {

    public GameObject NetworkManager;
    public GameObject Board;
    private NetworkHandler _handler;

	// Use this for initialization
	void Start () {
        _handler = NetworkManager.GetComponent<NetworkHandler>();
        if (_handler != null)
        {
            _handler.Register("marker_update", OnMove);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (_handler == null)
        {
            _handler = NetworkManager.GetComponent<NetworkHandler>();
            if (_handler != null)
            {
                _handler.Register("marker_update", OnMove);
            }
        }

	}

    private void OnMove(string message)
    {
        MarkerUpdate marker = JsonUtility.FromJson<MarkerUpdate>(message);
        print("Moving to " + marker.Column + " " + marker.Row);
        Vector3 newPosition = Board.GetComponent<Board>().GetAnchor(marker.Column, marker.Row);
        transform.position = newPosition;
    }
}

[Serializable]
class MarkerUpdate
{
    public string Type;
    public int Id;
    public int Column;
    public int Row;
}

