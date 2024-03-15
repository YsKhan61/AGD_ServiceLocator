using NUnit.Framework;
using UnityEngine;
using ServiceLocator.Player;
using ServiceLocator.Player.Projectile;
using System.Collections.Generic;

public class PlayerService_UnitTest 
{
    private PlayerService playerService;

    [SetUp]
    public void Setup()
    {
        PlayerScriptableObject playerSO = CreateDummyPlayerSO();
        // playerService = new GameObject().AddComponent<PlayerService>();
        // playerService.playerScriptableObject = playerSO;
    }

    private PlayerScriptableObject CreateDummyPlayerSO()
    {
        PlayerScriptableObject playerScriptableObject = ScriptableObject.CreateInstance<PlayerScriptableObject>();
        playerScriptableObject.ProjectilePrefab = new GameObject().AddComponent<ProjectileView>();
        playerScriptableObject.ProjectileScriptableObjects = new List<ProjectileScriptableObject>();
        playerScriptableObject.Health = 10;
        playerScriptableObject.Money = 0;
        return playerScriptableObject;
    }

    [Test]
    public void GetReward_AddsMoney()
    {
        // Arrange
        var initialMoney = playerService.Money;

        // Act
        playerService.GetReward(100);

        // Assert
        Assert.AreEqual(initialMoney + 100, playerService.Money);
    }
}
