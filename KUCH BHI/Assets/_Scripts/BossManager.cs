using UnityEngine;

[System.Serializable]
public struct LaserPhaseSettings
{
    public float minRandomInterval;
    public float maxRandomInterval;
}

public class BossManager : MonoBehaviour
{
    public static BossManager Instance { get; private set; }

    [Header("Chain Tracking")]
    [SerializeField] private int totalChains = 4;
    private int remainingChains;

    [Header("Boss Subsystems")]
    [SerializeField] private RockSpawner rockSpawner;
    [SerializeField] private BossLaser bossLaser;

    [Header("Laser Random Timing Ranges per Phase")]
    [Tooltip("Index 0 = 4 chains left, Index 1 = 3 chains left, etc.")]
    [SerializeField]
    private LaserPhaseSettings[] laserPhases = new LaserPhaseSettings[]
    {
        new LaserPhaseSettings { minRandomInterval = 5.0f, maxRandomInterval = 8.0f }, // Phase 1 (Slower)
        new LaserPhaseSettings { minRandomInterval = 3.5f, maxRandomInterval = 6.0f }, // Phase 2
        new LaserPhaseSettings { minRandomInterval = 2.0f, maxRandomInterval = 4.0f }, // Phase 3
        new LaserPhaseSettings { minRandomInterval = 1.0f, maxRandomInterval = 2.5f }  // Phase 4 (Aggressive)
    };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        remainingChains = totalChains;
    }

    private void Start()
    {
        UpdatePhaseDifficulty();
    }

    public void BreakChain()
    {
        remainingChains--;

        if (remainingChains <= 0)
        {
            DefeatBoss();
        }
        else
        {
            UpdatePhaseDifficulty();
        }
    }

    private void UpdatePhaseDifficulty()
    {
        int phaseIndex = Mathf.Clamp(totalChains - remainingChains, 0, laserPhases.Length - 1);
        LaserPhaseSettings currentPhase = laserPhases[phaseIndex];

        // Start random laser cycle for current phase
        bossLaser.StartRandomLaserCycle(currentPhase.minRandomInterval, currentPhase.maxRandomInterval);
    }

    private void DefeatBoss()
    {
        if (bossLaser != null) bossLaser.StopLaser();
        if (rockSpawner != null) rockSpawner.StopSpawning();
        Debug.Log("Boss Defeated!");
    }
}
