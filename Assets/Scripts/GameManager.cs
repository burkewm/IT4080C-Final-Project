using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public GameObject playerPrefab;
    public GameObject spawnPoints;

    private int spawnIndex = 0;
    private List<Vector3> availiableSpawns = new List<Vector3>();
    

    public void Awake()
    {
        RefreshSpawns();
    }

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            SpawnPlayers();
        }
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void RefreshSpawns()
    {
        Transform[] allPoints = spawnPoints.GetComponentsInChildren<Transform>();
        availiableSpawns.Clear();
        foreach (Transform point in allPoints)
        {
            if (point != spawnPoints.transform)
            {
                availiableSpawns.Add(point.position);
            }
        }
    }

    public Vector3 GetNextSpawnLocation()
    {
        var newPosition = availiableSpawns[spawnIndex];
        spawnIndex += 1;
        if (spawnIndex > availiableSpawns.Count - 1)
        {
            spawnIndex = 0;
        }
        return newPosition;
    }
    private void SpawnPlayers()
    {
        foreach (PlayerInfo pi in GameData.Instance.allPlayers)
        {
            GameObject playerSpawn = Instantiate(playerPrefab, GetNextSpawnLocation(), Quaternion.identity);
            playerSpawn.GetComponent<NetworkObject>().SpawnAsPlayerObject(pi.clientId);
            Debug.Log("Spawned Player " + pi.clientId);
            var pb = playerSpawn.GetComponentInChildren<PlayerBall>();
            pb.PlayerColor.Value = pi.color;
            pb.PlayerID.Value = (int)pi.clientId;
        }
    }
    
}