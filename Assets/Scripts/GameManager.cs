using System.Collections;
using System.Linq;
using Components;
using Managers;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkManager
{
    public static bool CanInput = true;
    
    [SerializeField] private float reloadCooldown = 5f;

    public override void OnServerSceneChanged(string sceneName)
    {
        CanInput = true;
        
        if (networkSceneName == sceneName)
        {
            Debug.Log("Poop");
            startPositions = FindObjectsOfType<SpawnPoint>()
                .Select<SpawnPoint, Transform>(s => s.transform)
                .ToList<Transform>();
            
            FindObjectOfType<PlayerScoreManager>().OnWin.AddListener(OnGameEnded);
        }
    }

    public void Connect(string host)
    {
        networkAddress = host;
        StartClient(); 
    }

    public void Host()
    {
        StartHost();
    }

    public void Disconnect()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            StopClient();
        }
    }

    [Server]
    public void OnGameEnded(uint winner)
    {
        StartCoroutine(ReloadScene());
    }

    private IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(reloadCooldown);
        
        ServerChangeScene(SceneManager.GetActiveScene().name);
    }
}
