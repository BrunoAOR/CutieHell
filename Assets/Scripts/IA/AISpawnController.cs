﻿// Warning CS0649: Field '#fieldname#' is never assigned to, and will always have its default value null (CS0649) (Assembly-CSharp)
// Warning was raised for the following fields: EnemyTypePrefab::type and EnemyTypePrefab::prefab
// Warning was disabled because these private fields are serialized and assigned through the inspector
#pragma warning disable 0649

using System.Collections.Generic;
using UnityEngine;

public class AISpawnController : MonoBehaviour
{
    [System.Serializable]
    private class EnemyTypePrefab
    {
        public EnemyType type;
        public AIEnemy prefab;
    }

    #region Fields

    [SerializeField]
    ScenarioController scenario;
    //[ShowOnly]
    //[SerializeField]
    //private float roundElapsedTime;
    
    [ShowOnly]
    [SerializeField]
    private int currentRoundIndex = -1;
    [ShowOnly]
    [SerializeField]
    private int currentWaveIndex = -1;
    [SerializeField]
    private RoundInfo[] roundInfos;
    [SerializeField]
    private List<AISpawner> aiSpawners = new List<AISpawner>();

    private int wavesLeftToFinishSpawn = -1;
    private float waveDelayTime = float.MaxValue;
    private float waveDelayLeft = 0;
    private List<WaveInfo> activeWaves = new List<WaveInfo>();
    private List<WaveInfo> wavesToRemove = new List<WaveInfo>();

    public Transform pooledEnemies;
    public Transform activeEnemies;

    [SerializeField]
    private EnemyTypePrefab[] enemyPrefabs;
    private Dictionary<EnemyType, ObjectPool<AIEnemy>> enemyPools;

    private bool validRoundsInfo = true;
    private bool roundRunning = false;

    #endregion

    #region MonoBehaviour Methods

    private void Awake()
    {
        if (!scenario)
        {
            scenario = GetComponentInParent<ScenarioController>();
            UnityEngine.Assertions.Assert.IsNotNull(scenario, "ERROR: Scenario not set for AISpawnController in gameObject '" + gameObject.name + "'");
        }

        UnityEngine.Assertions.Assert.IsNotNull(scenario, "ERROR: enemiesContainer not assigned for AISpawnController in gameObject '" + gameObject.name + "'");
        UnityEngine.Assertions.Assert.IsTrue(enemyPrefabs.Length > 0, "WARNING: No enemyPrefabs have been assigned for AISpawnController in gameObject '" + gameObject.name + "'");

        UnityEngine.Assertions.Assert.IsNotNull(pooledEnemies, "ERROR: pooledEnemies Transform not assigned for AISpawnController in gameObject '" + gameObject.name + "'");
        UnityEngine.Assertions.Assert.IsNotNull(activeEnemies, "ERROR: activeEnemies Transform not assigned for AISpawnController in gameObject '" + gameObject.name + "'");

        enemyPools = new Dictionary<EnemyType, ObjectPool<AIEnemy>>();
        for (int i = 0; i < enemyPrefabs.Length; ++i)
        {
            enemyPools.Add(enemyPrefabs[i].type, new ObjectPool<AIEnemy>(enemyPrefabs[i].prefab, pooledEnemies));
        }
        validRoundsInfo = VerifyRoundInfos();
        currentRoundIndex = -1;
    }

    private void Update()
    {
        if (roundRunning)
        {
            if (waveDelayLeft > 0)
            {
                waveDelayLeft -= Time.deltaTime;
                if (waveDelayLeft < 0)
                {
                    waveDelayLeft = 0;
                    StartNextWave();
                }
                UIManager.instance.SetWaveDelayIndicator(waveDelayLeft / waveDelayTime);
            }
            HandleActiveWaves();
        }
    }

    #endregion

    #region Public Methods
    public AIEnemy GetEnemy(EnemyType enemyType)
    {
        AIEnemy enemy = enemyPools[enemyType].GetObject(false);
        enemy.spawnController = this;
        return enemy;
    }

    public void ReturnEnemy(AIEnemy enemy)
    {
        enemyPools[enemy.enemyType].ReturnToPool(enemy);
    }

    public bool HasNextRound()
    {
        return validRoundsInfo
            && currentRoundIndex >= -1
            && currentRoundIndex < roundInfos.Length - 1;
    }

    public bool StartNextRound()
    {
        if (HasNextRound())
        {
            ++currentRoundIndex;
            RoundInfo roundInfo = roundInfos[currentRoundIndex];
            currentWaveIndex = -1;
            wavesLeftToFinishSpawn = roundInfo.waveInfos.Length;
            waveDelayLeft = roundInfo.waveInfos[0].waveStartDelay;
            roundRunning = true;
            scenario.OnNewRoundStarted();
            Debug.Log("Starting round (index) " + currentRoundIndex + "!");
            return true;
        }
        return false;
    }

    public bool HasNextWaveInRound()
    {
        return validRoundsInfo
            && currentRoundIndex >= 0
            && currentRoundIndex < roundInfos.Length
            && currentWaveIndex >= -1
            && currentWaveIndex < roundInfos[currentRoundIndex].waveInfos.Length - 1;
    }

    public bool StartNextWave()
    {
        if (HasNextWaveInRound())
        {
            ++currentWaveIndex;
            RoundInfo roundInfo = roundInfos[currentRoundIndex];
            WaveInfo waveInfo = roundInfo.waveInfos[currentWaveIndex];
            waveInfo.elapsedTime = 0;
            waveInfo.nextSpawnIndex = 0;
            activeWaves.Add(waveInfo);
            UIManager.instance.SetWaveIndicator(currentWaveIndex + 1, roundInfo.waveInfos.Length);
            Debug.Log("Starting wave (index) " + currentWaveIndex + " in round (index) " + currentRoundIndex + "!");
            return true;
        }
        return false;
    }

    public bool ForceStartRound(int roundIndex)
    {
        if(validRoundsInfo && roundIndex >= 0 && roundIndex < roundInfos.Length)
        {
            scenario.ClearCurrentActiveEnemies();
            foreach (AISpawner spawner in aiSpawners)
                spawner.ClearSpawnInfos();
            activeWaves.Clear();

            currentRoundIndex = roundIndex - 1;
            UIManager.instance.SetWaveDelayIndicator(0);
            return StartNextRound();
        }
        return false;
    }

    public bool ForceStartWave(int waveIndex)
    {
        if (validRoundsInfo && currentRoundIndex >= 0 && currentRoundIndex < roundInfos.Length)
        {
            RoundInfo roundInfo = roundInfos[currentRoundIndex];
            if (validRoundsInfo && waveIndex >= 0 && waveIndex < roundInfo.waveInfos.Length)
            {
                scenario.ClearCurrentActiveEnemies();
                foreach (AISpawner spawner in aiSpawners)
                    spawner.ClearSpawnInfos();
                activeWaves.Clear();

                wavesLeftToFinishSpawn = roundInfo.waveInfos.Length - waveIndex;
                scenario.OnNewRoundStarted();

                currentWaveIndex = waveIndex - 1;
                waveDelayLeft = 0;
                return StartNextWave();
            }
        }
        return false;
    }

    public void RestartCurrentWave()
    {
        ForceStartWave(currentRoundIndex);
    }

    public void WinActiveWaves()
    {
        if (roundRunning)
        {
            scenario.ClearCurrentActiveEnemies();
            foreach (AISpawner spawner in aiSpawners)
                spawner.ClearSpawnInfos();
            wavesToRemove.AddRange(activeWaves);
        }
    }

    public void WinActiveRound()
    {
        if (roundRunning)
        {
            wavesLeftToFinishSpawn = -1;
            roundRunning = false;
            scenario.OnLastEnemySpawned();

            scenario.ClearCurrentActiveEnemies();
            foreach (AISpawner spawner in aiSpawners)
                spawner.ClearSpawnInfos();
            activeWaves.Clear();
        }
    }

    public void ForceNextRound()
    {
        ForceStartRound(currentRoundIndex + 1);
    }

    public void ForcePreviousRound()
    {
        ForceStartRound(currentRoundIndex - 1);
    }

    public void StopRound()
    {
        roundRunning = false;
    }

    public int GetCurrentRoundIndex()
    {
        return currentRoundIndex;
    }

    public int GetCurrentWaveIndex()
    {
        return currentWaveIndex;
    }
    #endregion

    #region Private Methods
    private void HandleActiveWaves()
    {
        for (int i = 0; i < activeWaves.Count; ++i)
        {
            WaveInfo waveInfo = activeWaves[i];
            if (waveInfo.nextSpawnIndex < waveInfo.spawnInfos.Length)
            {
                SpawnInfo spawnInfo = waveInfo.spawnInfos[waveInfo.nextSpawnIndex];
                if (waveInfo.elapsedTime >= spawnInfo.spawnTime)
                {
                    AISpawner spawner = aiSpawners[spawnInfo.spawnerIndex];
                    spawner.Spawn(spawnInfo);
                    ++waveInfo.nextSpawnIndex;
                }
            }
            else if (waveInfo.elapsedTime > waveInfo.lastEnemySpawnTime)
            {
                if (i == activeWaves.Count - 1)
                    OnCurrentWaveFinishedSpawning();

                wavesToRemove.Add(waveInfo);
            }
            waveInfo.elapsedTime += Time.deltaTime;
        }
        ClearFinishedWaves();
    }

    private void OnCurrentWaveFinishedSpawning()
    {
        if (HasNextWaveInRound())
        {
            waveDelayTime = roundInfos[currentRoundIndex].waveInfos[currentWaveIndex + 1].waveStartDelay;
            waveDelayLeft = waveDelayTime;
        }
    }

    private void ClearFinishedWaves()
    {
        foreach (WaveInfo waveInfo in wavesToRemove)
        {
            --wavesLeftToFinishSpawn;
            activeWaves.Remove(waveInfo);
        }
        wavesToRemove.Clear();
        CheckAllWavesSpawned();
    }

    private void CheckAllWavesSpawned()
    {
        if (wavesLeftToFinishSpawn == 0)
        {
            wavesLeftToFinishSpawn = -1;
            roundRunning = false;
            scenario.OnLastEnemySpawned();
        }
    }

    private bool VerifyRoundInfos()
    {
        for (int r = 0; r < roundInfos.Length; ++r)
        {
            RoundInfo roundInfo = roundInfos[r];
            if (roundInfo == null)
            {
                Debug.LogError("ERROR: RoundInfos error on AISpawnController. RoundInfo " + r + " is null!");
                return false;
            }
            if (roundInfo.waveInfos.Length == 0)
            {
                Debug.LogError("ERROR: RoundInfos error on AISpawnController. RoundInfo " + r + " contains an empty array of WaveInfos!");
                return false;
            }
            for (int w = 0; w < roundInfo.waveInfos.Length; ++w)
            {
                WaveInfo waveInfo = roundInfo.waveInfos[w];
                if (waveInfo == null)
                {
                    Debug.LogError("ERROR: RoundInfos error on AISpawnController. RoundInfo " + r + ", WaveInfo " + w + " is null!");
                    return false;
                }
                if (waveInfo.waveStartDelay < 0)
                {
                    Debug.LogError("ERROR: RoundInfos error on AISpawnController. RoundInfo " + r + ", WaveInfo " + w + " has a negative waveStartDelay!");
                    return false;
                }
                if (waveInfo.spawnInfos.Length == 0)
                {
                    Debug.LogError("ERROR: RoundInfos error on AISpawnController. RoundInfo " + r + ", WaveInfo " + w + " contains an empty array of SpawnInfos!");
                    return false;
                }
                float lastSpawnTime = -1;
                waveInfo.lastEnemySpawnTime = -1;
                for (int s = 0; s < waveInfo.spawnInfos.Length; ++s)
                {
                    SpawnInfo spawnInfo = waveInfo.spawnInfos[s];
                    if (spawnInfo == null)
                    {
                        Debug.LogError("ERROR: RoundInfos error on AISpawnController. RoundInfo " + r + ", WaveInfo " + w + ", SpawnInfo " + s + " is null!");
                        return false;
                    }
                    if (spawnInfo.enemiesToSpawn == null)
                    {
                        Debug.LogError("ERROR: RoundInfos error on AISpawnController. RoundInfo " + r + ", WaveInfo " + w + ", SpawnInfo " + s + ", enemiesToSpawn is null!");
                        return false;
                    }
                    if (spawnInfo.enemiesToSpawn == null)
                    {
                        Debug.LogError("ERROR: RoundInfos error on AISpawnController. RoundInfo " + r + ", WaveInfo " + w + ", SpawnInfo " + s + ", contains an empty array of enemiesToSpawn!");
                        return false;
                    }
                    if (spawnInfo.spawnerIndex < 0 || spawnInfo.spawnerIndex >= aiSpawners.Count)
                    {
                        Debug.LogError("ERROR: RoundInfos error on AISpawnController. RoundInfo " + r + ", WaveInfo " + w + ", SpawnInfo " + s + ", spawnerIndex is out of the range defined in aiSpawners!");
                        return false;
                    }
                    if (spawnInfo.spawnTime < 0)
                    {
                        Debug.LogError("ERROR: RoundInfos error on AISpawnController. RoundInfo " + r + ", WaveInfo " + w + ", SpawnInfo " + s + ", has a negative spawnTime!");
                        return false;
                    }
                    if (spawnInfo.spawnDuration < 0)
                    {
                        Debug.LogError("ERROR: RoundInfos error on AISpawnController. RoundInfo " + r + ", WaveInfo " + w + ", SpawnInfo " + s + ", has a negative spawnDuration!");
                        return false;
                    }
                    if (spawnInfo.spawnTime < lastSpawnTime)
                    {
                        Debug.LogError("ERROR: RoundInfos error on AISpawnController. RoundInfo " + r + " WaveInfo " + w + ", SpawnInfo " + s + ", has a spawn time that is less than the previous SpawnInfo!");
                        return false;
                    }
                    float lastEnemySpawnTime = spawnInfo.spawnTime + spawnInfo.spawnDuration;
                    if (lastEnemySpawnTime > waveInfo.lastEnemySpawnTime)
                    {
                        waveInfo.lastEnemySpawnTime = lastEnemySpawnTime;
                    }
                }
            }
        }
        return true;
    }
    #endregion
}
