using System;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public EBuildingType type;
    public int buildingID;
    public int ownerID; // ID do NPC ou jogador proprietário (0 se for público)
    public List<EJobType> suitableJobs = new(); // Tipos de trabalho adequados para este edifício
    
    public List<NpcController> employees = new();
    public int maxEmployees;
    public List<Room> rooms = new();
    public List<InteractableObject> interactables = new();
    
    public DateTime OpenTime;
    public DateTime CloseTime;
    
    // Estado do edifício
    public bool isConstructed = false;
    public float constructionProgress = 0f;
    public float health = 100f; // "Vida" do edifício
    public float maxHealth = 100f;
    
    // Economia
    public float value;
    public float constructionCost;
    public float maintenanceCost;
    public float repairCost;
    public float incomeGenerated;
    
    // Sistema de reparo
    private bool needsRepair = false;
    private NpcController repairNPC;

    public void Hire(NpcController npc, EJobType jobType)
    {
        if(employees.Count < maxEmployees)
        {
            employees.Add(npc);
            npc.AssignJob(jobType);
        }
    }

    private void Start()
    {
        if (isConstructed)
            OnConstructionComplete();
    }

    private void Update()
    {
        // Verificar saúde do edifício
        if (health < maxHealth * 0.3f && !needsRepair)
        {
            needsRepair = true;
            RequestRepair();
        }
        
        // Atualizar renda gerada, se aplicável
        if (isConstructed && type != EBuildingType.House)
        {
            GenerateIncome();
        }
    }
    
    public void AdvanceConstruction(float amount)
    {
        if (isConstructed) return;
        
        constructionProgress += amount;
        if (constructionProgress >= 100f)
        {
            isConstructed = true;
            OnConstructionComplete();
        }
    }
    
    private void OnConstructionComplete()
    {
        // Notificar sistemas relevantes
        WorldSimulationManager.Instance.RegisterBuilding(this);
        
        // Atualizar a economia
        EconomyManager.Instance.RegisterPropertyValue(buildingID, value);
        
        // Habilitar todos os quartos e objetos interativos
        foreach (Room room in rooms)
        {
            room.Enable();
        }
        
        Debug.Log($"Construção concluída: {type} (ID: {buildingID})");
    }
    
    public void TakeDamage(float amount)
    {
        health -= amount;
        health = Mathf.Max(0f, health);
        
        // Se a saúde chegar a 0, o edifício fica inutilizável até ser reparado
        if (health <= 0f)
        {
            DisableBuilding();
        }
    }
    
    private void DisableBuilding()
    {
        // Desabilitar todos os quartos e objetos interativos
        foreach (Room room in rooms)
        {
            room.Disable();
        }
        
        needsRepair = true;
        RequestRepair();
    }
    
    private void RequestRepair()
    {
        // Procurar um NPC com habilidades de reparo
        repairNPC = WorldSimulationManager.Instance.FindWorker(EJobType.Craftsman);
        if (repairNPC != null)
        {
            // Código para chamar o NPC de reparo (seria implementado usando GOAP)
            Debug.Log($"NPC {repairNPC.name} foi chamado para reparar o edifício {buildingID}");
        }
    }
    
    public void Repair(float amount)
    {
        health += amount;
        if (health >= maxHealth)
        {
            health = maxHealth;
            needsRepair = false;
            
            // Reabilitar o edifício se estava totalmente danificado
            if (!rooms[0].isEnabled)
            {
                foreach (Room room in rooms)
                {
                    room.Enable();
                }
            }
        }
    }
    
    private void GenerateIncome()
    {
        // Gerar renda com base no tipo de edifício e condições
        if (ownerID != 0)
        {
            // Encontrar o dono
            NpcController owner = WorldSimulationManager.Instance.GetNPCById(ownerID);
            if (owner != null)
            {
                // Calcular renda com base no estado do edifício
                float healthFactor = health / maxHealth;
                float actualIncome = incomeGenerated * healthFactor * Time.deltaTime / 86400f; // Dividir por segundos em um dia
                
                // Adicionar renda ao dono
                owner.AddMoney(actualIncome);
            }
        }
    }
    
    public float CalculateRepairCost()
    {
        float damageFactor = (maxHealth - health) / maxHealth;
        return repairCost * damageFactor;
    }
    
    // Métodos para adicionar e remover quartos
    public void AddRoom(Room room)
    {
        rooms.Add(room);
        room.parentBuilding = this;
        
        // Atualizar valor do edifício
        UpdateBuildingValue();
    }
    
    public void RemoveRoom(Room room)
    {
        if (rooms.Contains(room))
        {
            rooms.Remove(room);
            room.parentBuilding = null;
            
            // Atualizar valor do edifício
            UpdateBuildingValue();
        }
    }
    
    private void UpdateBuildingValue()
    {
        // Calcular valor com base nos quartos e objetos
        float baseValue = 0f;
        
        foreach (Room room in rooms)
        {
            baseValue += room.CalculateValue();
        }
        
        // Aplicar multiplicador baseado no tipo de edifício
        float multiplier = GetEBuildingTypeMultiplier();
        value = baseValue * multiplier;
    }
    
    private float GetEBuildingTypeMultiplier()
    {
        switch (type)
        {
            case EBuildingType.House: return 1.0f;
            case EBuildingType.Store: return 1.2f;
            case EBuildingType.Restaurant: return 1.3f;
            case EBuildingType.Hospital: return 1.5f;
            case EBuildingType.PoliceStation: return 1.4f;
            case EBuildingType.Factory: return 1.6f;
            case EBuildingType.Farm: return 1.1f;
            case EBuildingType.Park: return 0.8f;
            case EBuildingType.School: return 1.3f;
            case EBuildingType.Entertainment: return 1.4f;
            case EBuildingType.Government: return 1.5f;
            case EBuildingType.SecretBase: return 2.0f;
            default: return 1.0f;
        }
    }
}