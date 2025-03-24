using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class WorldSimulationManager : MonoBehaviour
{
    // Singleton para acesso global
    public static WorldSimulationManager Instance { get; private set; }
    
    // Gerencia tempo, eventos globais e progressão do mundo
    [SerializeField] private float timeScale = 1f;
    [SerializeField] private int currentDay = 0;
    [SerializeField] private int currentHour = 0;
    
    // Listas de todos os NPCs, edifícios e recursos
    public List<NpcController> allNPCs = new();
    public List<Building> allBuildings = new List<Building>();
    
    // Eventos principais
    public event Action<int> OnNewDay;
    public event Action<int> OnNewHour;
    
    private void Awake()
    {
        if (Instance == null) 
            Instance = this;
        else 
            Destroy(gameObject);
    }
    
    private void Update()
    {
        // Lógica de progressão de tempo
        // Disparo de eventos como OnNewHour, OnNewDay
    }

    #region NPC

    public void RegisterNPC(NpcController npc)
    {
        allNPCs.Add(npc);
    }
    
    public string GetNPCName(int id)
    {
        return allNPCs.FirstOrDefault(x => x.id == id)?.name;
    }

    public NpcController GetNPCById(int id)
    {
        return allNPCs.FirstOrDefault(x => x.id == id);
    }
    
    public NpcController FindWorker(EJobType jobType)
    {
        return allNPCs.FirstOrDefault(x => x.Job == jobType);
    }

    public int GetNextid()
    {
        return allNPCs.Any() ? allNPCs.OrderBy(x => x.id).Last().id + 1 : 1;
    }

    #endregion

    #region Building

    public void RegisterBuilding(Building building)
    {
        allBuildings.Add(building);
    }

    public Building GetBuildingById(int id)
    {
        return allBuildings.FirstOrDefault(x => x.buildingID == id);
    }
    
    [CanBeNull]
    public Building GetBuildingByJob(EJobType jobType)
    {
        return allBuildings
            .Where(x => x.suitableJobs.Contains(jobType))
            .OrderBy(x => Guid.NewGuid()).FirstOrDefault();
    }
    
    [CanBeNull]
    public Building FindNpcHome(NpcController npc)
    {
        return allBuildings.FirstOrDefault(x => x.type == EBuildingType.House && x.employees.Contains(npc));
    }
    
    [CanBeNull]
    public Building FindAvailableHome(float moneyAvailable)
    {
        var emptyHouses = allBuildings.Where(x => x.type == EBuildingType.House && x.employees.Count == 0).ToList();
        
        return emptyHouses.FirstOrDefault(x => x.value <= moneyAvailable);
    }

    #endregion
}