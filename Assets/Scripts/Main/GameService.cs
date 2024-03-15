using ServiceLocator.Player;
using ServiceLocator.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameService : GenericMonoSingleton<GameService>
{
    [SerializeField] PlayerScriptableObject playerScriptableObject;

    public PlayerService PlayerService { get; private set; }

    private void Start()
    {
        PlayerService = new PlayerService(playerScriptableObject);
    }

    private void Update()
    {
        PlayerService.UpdatePlayerService();
    }
}
