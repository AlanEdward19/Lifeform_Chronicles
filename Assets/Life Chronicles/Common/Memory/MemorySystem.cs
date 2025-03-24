using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MemorySystem : MonoBehaviour
{
    // Referência ao NPC proprietário
    public NpcController owner;
    
    // Todas as memórias do NPC
    private List<Memory> memories = new List<Memory>();
    
    // Memórias de curto prazo (mais recentes e ainda não consolidadas)
    private List<Memory> shortTermMemories = new List<Memory>();
    
    // Memórias de longo prazo (consolidadas)
    private List<Memory> longTermMemories = new List<Memory>();
    
    // Memórias traumáticas (negativas e importantes)
    private List<Memory> traumaticMemories = new List<Memory>();
    
    // Memórias felizes (positivas e importantes)
    private List<Memory> happyMemories = new List<Memory>();
    
    // Memórias relacionadas a NPCs específicos
    private Dictionary<int, List<Memory>> npcRelatedMemories = new Dictionary<int, List<Memory>>();
    
    // Contagem global de memórias para gerar IDs únicos
    private static int globalMemoryCounter = 0;
    
    // Configurações
    [SerializeField] private int shortTermMemoryCapacity = 20;
    [SerializeField] private int longTermMemoryCapacity = 100;
    [SerializeField] private float consolidationInterval = 6f; // Em horas do jogo
    [SerializeField] private float memoryDecayInterval = 24f; // Em horas do jogo
    
    // Temporizadores
    private float consolidationTimer = 0f;
    private float decayTimer = 0f;
    
    private void Start()
    {
        // Garantir que temos uma referência ao NPC proprietário
        if (owner == null)
            owner = GetComponent<NpcController>();
            
        // Registrar para eventos globais de tempo
        WorldSimulationManager.Instance.OnNewHour += OnNewHour;
    }
    
    private void OnDestroy()
    {
        // Desregistrar de eventos
        if (WorldSimulationManager.Instance != null)
            WorldSimulationManager.Instance.OnNewHour -= OnNewHour;
    }
    
    private void OnNewHour(int hour)
    {
        // Avançar temporizadores
        consolidationTimer += 1f;
        decayTimer += 1f;
        
        // Verificar se é hora de consolidar memórias
        if (consolidationTimer >= consolidationInterval)
        {
            ConsolidateMemories();
            consolidationTimer = 0f;
        }
        
        // Verificar se é hora de aplicar decaimento
        if (decayTimer >= memoryDecayInterval)
        {
            ApplyMemoryDecay();
            decayTimer = 0f;
        }
    }
    
    // Adiciona uma nova memória ao sistema
    public void AddMemory(Memory memory)
    {
        // Definir ID único para a memória
        memory.id = Guid.NewGuid();
        
        // Definir timestamp se não tiver
        if (memory.timestamp == default)
            memory.timestamp = DateTime.Now;
            
        // Adicionar à lista geral
        memories.Add(memory);
        
        // Adicionar à lista de curto prazo
        shortTermMemories.Add(memory);
        
        // Limitar tamanho da memória de curto prazo
        if (shortTermMemories.Count > shortTermMemoryCapacity)
        {
            // Remover a memória menos importante da lista de curto prazo
            shortTermMemories = shortTermMemories
                .OrderByDescending(m => m.GetCurrentRelevance(DateTime.Now))
                .Take(shortTermMemoryCapacity)
                .ToList();
        }
        
        // Categorizar memória com base na importância e impacto emocional
        if (memory.importance > 70 && memory.emotionalImpact < -50)
        {
            traumaticMemories.Add(memory);
        }
        else if (memory.importance > 70 && memory.emotionalImpact > 50)
        {
            happyMemories.Add(memory);
        }
        
        // Indexar memórias por NPCs envolvidos
        foreach (int npcID in memory.involvedNPCIDs)
        {
            if (!npcRelatedMemories.ContainsKey(npcID))
                npcRelatedMemories[npcID] = new List<Memory>();
                
            npcRelatedMemories[npcID].Add(memory);
        }
        
        // Notificar o NPC sobre a nova memória para possível reação imediata
        owner.OnNewMemoryAdded(memory);
    }
    
    // Processo que move memórias de curto para longo prazo
    private void ConsolidateMemories()
    {
        // Lista temporária para armazenar memórias a serem movidas
        List<Memory> memoriesToMove = new List<Memory>();
        
        foreach (Memory memory in shortTermMemories)
        {
            // Verificar se a memória é importante o suficiente para ser mantida
            if (memory.importance > 30 || memory.emotionalImpact > 40 || memory.emotionalImpact < -40)
            {
                memoriesToMove.Add(memory);
            }
            else
            {
                // Para memórias menos importantes, há uma chance de esquecimento
                float rememberChance = 0.3f + (memory.importance / 100f * 0.7f);
                if (UnityEngine.Random.value < rememberChance)
                {
                    memoriesToMove.Add(memory);
                }
                else
                {
                    // Memória "esquecida" - não vai para o longo prazo
                    // mas permanece na lista geral para referência
                    Debug.Log($"NPC {owner.name} esqueceu memória: {memory.description}");
                }
            }
        }
        
        // Mover memórias selecionadas para o longo prazo
        foreach (Memory memory in memoriesToMove)
        {
            if (!longTermMemories.Contains(memory))
                longTermMemories.Add(memory);
        }
        
        // Limitar o tamanho da memória de longo prazo
        if (longTermMemories.Count > longTermMemoryCapacity)
        {
            // Ordenar por relevância atual e manter apenas as mais importantes
            longTermMemories = longTermMemories
                .OrderByDescending(m => m.GetCurrentRelevance(DateTime.Now))
                .Take(longTermMemoryCapacity)
                .ToList();
        }
        
        // Limpar memória de curto prazo
        shortTermMemories.Clear();
    }
    
    // Aplica o decaimento natural a todas as memórias
    private void ApplyMemoryDecay()
    {
        DateTime currentTime = DateTime.Now;
        float dayDecayAmount = 1.0f / 365.0f; // Decaimento diário base
        
        foreach (Memory memory in memories)
        {
            memory.UpdateDecay(dayDecayAmount);
        }
    }
    
    // Recupera memórias relacionadas a um NPC específico
    public List<Memory> GetMemoriesAboutNPC(int npcID, float minRelevance = 0)
    {
        if (!npcRelatedMemories.ContainsKey(npcID))
            return new List<Memory>();
            
        DateTime currentTime = DateTime.Now;
        return npcRelatedMemories[npcID]
            .Where(m => m.GetCurrentRelevance(currentTime) >= minRelevance)
            .OrderByDescending(m => m.GetCurrentRelevance(currentTime))
            .ToList();
    }
    
    // Recupera memórias de um tipo específico
    public List<Memory> GetMemoriesByType(MemoryType type, float minRelevance = 0)
    {
        DateTime currentTime = DateTime.Now;
        return memories
            .Where(m => m.type == type && m.GetCurrentRelevance(currentTime) >= minRelevance)
            .OrderByDescending(m => m.GetCurrentRelevance(currentTime))
            .ToList();
    }
    
    // Recupera memórias mais recentes
    public List<Memory> GetRecentMemories(int count = 5)
    {
        return memories
            .OrderByDescending(m => m.timestamp)
            .Take(count)
            .ToList();
    }
    
    // Recupera memórias mais importantes
    public List<Memory> GetMostImportantMemories(int count = 5)
    {
        DateTime currentTime = DateTime.Now;
        return memories
            .OrderByDescending(m => m.GetCurrentRelevance(currentTime))
            .Take(count)
            .ToList();
    }
    
    // Recupera memórias traumáticas ou felizes
    public List<Memory> GetEmotionalMemories(bool positive, int count = 5)
    {
        if (positive)
            return happyMemories.OrderByDescending(m => m.emotionalImpact).Take(count).ToList();
        else
            return traumaticMemories.OrderByDescending(m => -m.emotionalImpact).Take(count).ToList();
    }
    
    // Recupera a "opinião" geral sobre um NPC baseada em todas as memórias
    public float GetOpinionAboutNPC(int npcID)
    {
        if (!npcRelatedMemories.ContainsKey(npcID) || npcRelatedMemories[npcID].Count == 0)
            return 50f; // Opinião neutra por padrão
            
        DateTime currentTime = DateTime.Now;
        float totalRelevance = 0f;
        float weightedImpact = 0f;
        
        foreach (Memory memory in npcRelatedMemories[npcID])
        {
            float relevance = memory.GetCurrentRelevance(currentTime);
            totalRelevance += relevance;
            weightedImpact += memory.emotionalImpact * relevance;
        }
        
        if (totalRelevance == 0)
            return 50f;
            
        // Normalizar para uma escala de 0-100, onde 50 é neutro
        return 50f + (weightedImpact / totalRelevance * 0.5f);
    }
    
    // Método para formar uma narrativa resumida baseada nas memórias
    public string FormNarrative(int maxMemories = 5)
    {
        if (memories.Count == 0)
            return $"{owner.name} não tem memórias significativas.";
            
        List<Memory> significantMemories = GetMostImportantMemories(maxMemories);
        string narrative = $"{owner.name} recorda: ";
        
        for (int i = 0; i < significantMemories.Count; i++)
        {
            Memory m = significantMemories[i];
            narrative += m.description;
            
            if (i < significantMemories.Count - 2)
                narrative += ", ";
            else if (i == significantMemories.Count - 2)
                narrative += " e ";
        }
        
        return narrative;
    }
}