using System;
using System.Collections.Generic;
using Unity.Jobs.LowLevel.Unsafe;

public static class MemoryFactory
{
    // Cria uma memória de interação entre NPCs
    public static Memory CreateInteractionMemory(int actorNPCID, int targetNPCID, string action, float impact, float importance)
    {
        string targetName = WorldSimulationManager.Instance.GetNPCName(targetNPCID);
        string actorName = WorldSimulationManager.Instance.GetNPCName(actorNPCID);
        
        Memory memory = new Memory
        {
            type = MemoryType.Interaction,
            involvedNPCIDs = new List<int> { actorNPCID, targetNPCID },
            emotionalImpact = impact,  // -100 a 100
            importance = importance,   // 0 a 100
            timestamp = DateTime.Now,
            description = $"{actorName} {action} {targetName}",
            details = new Dictionary<string, object>
            {
                { "action", action },
                { "actorID", actorNPCID },
                { "targetID", targetNPCID }
            }
        };
        
        return memory;
    }
    
    // Cria uma memória de evento significativo
    public static Memory CreateEventMemory(string eventDescription, float impact, float importance, List<int> involvedNPCs = null)
    {
        Memory memory = new Memory
        {
            type = MemoryType.Event,
            involvedNPCIDs = involvedNPCs ?? new List<int>(),
            emotionalImpact = impact,
            importance = importance,
            timestamp = DateTime.Now,
            description = eventDescription,
            details = new Dictionary<string, object>
            {
                { "eventType", "generic" }
            }
        };
        
        return memory;
    }
    
    // Cria uma memória de trabalho/profissão
    public static Memory CreateWorkMemory(int npcID, EJobType job, string specificEvent, float impact, float importance)
    {
        string npcName = WorldSimulationManager.Instance.GetNPCName(npcID);
        
        Memory memory = new Memory
        {
            type = MemoryType.Experience,
            involvedNPCIDs = new List<int> { npcID },
            emotionalImpact = impact,
            importance = importance,
            timestamp = DateTime.Now,
            description = $"{npcName} {specificEvent} como {job}",
            details = new Dictionary<string, object>
            {
                { "job", job },
                { "event", specificEvent }
            }
        };
        
        return memory;
    }
    
    // Cria uma memória de relacionamento
    public static Memory CreateRelationshipMemory(int npcID1, int npcID2, string relationshipChange, float impact, float importance)
    {
        string name1 = WorldSimulationManager.Instance.GetNPCName(npcID1);
        string name2 = WorldSimulationManager.Instance.GetNPCName(npcID2);
        
        Memory memory = new Memory
        {
            type = MemoryType.Relationship,
            involvedNPCIDs = new List<int> { npcID1, npcID2 },
            emotionalImpact = impact,
            importance = importance,
            timestamp = DateTime.Now,
            description = $"Relacionamento entre {name1} e {name2}: {relationshipChange}",
            details = new Dictionary<string, object>
            {
                { "change", relationshipChange }
            }
        };
        
        return memory;
    }
    
    // Cria uma memória de nascimento de filho
    public static Memory CreateChildbirthMemory(int motherID, int fatherID, int childID, string childName)
    {
        string motherName = WorldSimulationManager.Instance.GetNPCName(motherID);
        string fatherName = WorldSimulationManager.Instance.GetNPCName(fatherID);
        
        Memory memory = new Memory
        {
            type = MemoryType.Event,
            involvedNPCIDs = new List<int> { motherID, fatherID, childID },
            emotionalImpact = 90f,  // Muito positivo
            importance = 95f,       // Extremamente importante
            timestamp = DateTime.Now,
            description = $"{motherName} e {fatherName} tiveram um filho chamado {childName}",
            details = new Dictionary<string, object>
            {
                { "eventType", "childbirth" },
                { "motherID", motherID },
                { "fatherID", fatherID },
                { "childID", childID },
                { "childName", childName }
            }
        };
        
        return memory;
    }
    
    // Cria uma memória de morte
    public static Memory CreateDeathMemory(int deceasedID, EDeathCause cause, List<int> witnessIDs)
    {
        string deceasedName = WorldSimulationManager.Instance.GetNPCName(deceasedID);
        
        Memory memory = new Memory
        {
            type = MemoryType.Trauma,
            involvedNPCIDs = new List<int>(witnessIDs) { deceasedID },
            emotionalImpact = -90f, // Muito negativo
            importance = 95f,       // Extremamente importante
            timestamp = DateTime.Now,
            description = $"{deceasedName} morreu. Causa: {cause}",
            details = new Dictionary<string, object>
            {
                { "eventType", "death" },
                { "deceasedID", deceasedID },
                { "cause", cause }
            }
        };
        
        return memory;
    }
    
    // Cria uma memória de aprendizado
    public static Memory CreateLearningMemory(int npcID, string skill, int level, float importance)
    {
        string npcName = WorldSimulationManager.Instance.GetNPCName(npcID);
        
        Memory memory = new Memory
        {
            type = MemoryType.Learning,
            involvedNPCIDs = new List<int> { npcID },
            emotionalImpact = 40f,  // Moderadamente positivo
            importance = importance,
            timestamp = DateTime.Now,
            description = $"{npcName} aprendeu {skill} (nível {level})",
            details = new Dictionary<string, object>
            {
                { "skill", skill },
                { "level", level }
            }
        };
        
        return memory;
    }
    
    // Cria uma memória de construção
    public static Memory CreateConstructionMemory(int npcID, EBuildingType buildingType, int buildingID, string location)
    {
        string npcName = WorldSimulationManager.Instance.GetNPCName(npcID);
        
        Memory memory = new Memory
        {
            type = MemoryType.Achievement,
            involvedNPCIDs = new List<int> { npcID },
            emotionalImpact = 60f,  // Bastante positivo
            importance = 70f,       // Importante
            timestamp = DateTime.Now,
            description = $"{npcName} construiu {buildingType} em {location}",
            details = new Dictionary<string, object>
            {
                { "buildingType", buildingType },
                { "buildingID", buildingID },
                { "location", location }
            }
        };
        
        return memory;
    }
    
    // Cria uma memória de crime
    public static Memory CreateCrimeMemory(int perpetratorID, int victimID, string crimeType, List<int> witnessIDs)
    {
        string perpetratorName = WorldSimulationManager.Instance.GetNPCName(perpetratorID);
        string victimName = victimID > 0 ? WorldSimulationManager.Instance.GetNPCName(victimID) : "ninguém em particular";
        
        Memory memory = new Memory
        {
            type = MemoryType.Observation,
            involvedNPCIDs = new List<int>(witnessIDs) { perpetratorID },
            emotionalImpact = -70f, // Bastante negativo
            importance = 75f,       // Muito importante
            timestamp = DateTime.Now,
            description = $"{perpetratorName} cometeu {crimeType} contra {victimName}",
            details = new Dictionary<string, object>
            {
                { "crimeType", crimeType },
                { "perpetratorID", perpetratorID },
                { "victimID", victimID }
            }
        };
        
        if (victimID > 0)
            memory.involvedNPCIDs.Add(victimID);
            
        return memory;
    }
}