using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Memory
{
    public Guid id;
    public DateTime timestamp;
    public MemoryType type;
    public float importance; // 0-100, quanto maior mais importante
    public float emotionalImpact; // -100 (negativo) a 100 (positivo)
    public List<int> involvedNPCIDs = new List<int>(); // IDs dos NPCs relacionados
    public int? locationID; // Local onde o evento ocorreu
    public Dictionary<string, object> details = new Dictionary<string, object>(); // Detalhes flexíveis
    public string description; // Descrição legível para humanos

    // Nível de decaimento da memória (algumas memórias decaem mais rápido que outras)
    public float decayRate = 0.1f;
    
    // Quanto a memória já decaiu (0-1)
    public float decayLevel = 0;
    
    // Quanto mais a memória é acessada, mais forte ela fica
    public int accessCount = 1;
    
    // Calcula a relevância atual da memória com base em importância, tempo e decaimento
    public float GetCurrentRelevance(DateTime currentTime)
    {
        // Tempo decorrido em dias
        float daysSince = (float)(currentTime - timestamp).TotalDays;
        
        // Fator de tempo - memórias recentes são mais relevantes
        float timeFactor = Mathf.Exp(-0.05f * daysSince);
        
        // Fator de acesso - memórias acessadas com frequência são reforçadas
        float accessFactor = Mathf.Log10(accessCount + 1);
        
        // Nível de decaimento ajustado pelo acesso
        float effectiveDecay = decayLevel / (1 + 0.2f * accessFactor);
        
        // Relevância final
        return importance * timeFactor * (1 - effectiveDecay) * (1 + 0.5f * accessFactor);
    }
    
    // Método para quando a memória é acessada/recordada
    public void Access()
    {
        accessCount++;
        // Reforça a memória retardando seu decaimento
        decayLevel = Mathf.Max(0, decayLevel - 0.05f);
    }
    
    // Atualiza o decaimento natural da memória
    public void UpdateDecay(float deltaTime)
    {
        // Taxa de decaimento varia conforme a importância
        float adjustedDecayRate = decayRate * (1 - importance / 200); // Memórias importantes decaem mais lentamente
        decayLevel = Mathf.Min(1, decayLevel + adjustedDecayRate * deltaTime);
    }
}