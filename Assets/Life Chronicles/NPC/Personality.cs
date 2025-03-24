using System;
using UnityEngine;

[Serializable]
public class Personality
{
    // Traços de personalidade que afetam decisões e interações
    public float agreeableness = 50f; // 0-100
    public float conscientiousness = 50f;
    public float extraversion = 50f;
    public float neuroticism = 50f;
    public float openness = 50f;
    
    public void GeneratePersonality()
    {
        // Inicializar personalidade com valores aleatórios
        agreeableness = UnityEngine.Random.Range(20f, 80f);
        conscientiousness = UnityEngine.Random.Range(20f, 80f);
        extraversion = UnityEngine.Random.Range(20f, 80f);
        neuroticism = UnityEngine.Random.Range(20f, 80f);
        openness = UnityEngine.Random.Range(20f, 80f);
    }
    
    // Método para herdar traços de personalidade dos pais com variação
    public static Personality InheritFrom(Personality father, Personality mother)
    {
        Personality child = new Personality();
        
        // Herdar cada traço com 50% de chance de cada pai, mais variação aleatória
        child.agreeableness = BlendPersonalityTrait(father.agreeableness, mother.agreeableness);
        child.conscientiousness = BlendPersonalityTrait(father.conscientiousness, mother.conscientiousness);
        child.extraversion = BlendPersonalityTrait(father.extraversion, mother.extraversion);
        child.neuroticism = BlendPersonalityTrait(father.neuroticism, mother.neuroticism);
        child.openness = BlendPersonalityTrait(father.openness, mother.openness);
        
        return child;
    }
    
    private static float BlendPersonalityTrait(float fatherTrait, float motherTrait)
    {
        // Combinar os traços dos pais com um pouco de aleatoriedade
        float baseValue = UnityEngine.Random.value < 0.5f ? fatherTrait : motherTrait;
        
        // Adicionar variação (até 20% para mais ou para menos)
        float variation = UnityEngine.Random.Range(-20f, 20f);
        float result = baseValue + variation;
        
        // Limitar entre 0 e 100
        return Mathf.Clamp(result, 0f, 100f);
    }
    
    // Métodos para influenciar decisões com base na personalidade
    public bool IsLikelyToHelp(NpcController other)
    {
        // Alta concordância (agreeableness) aumenta a chance de ajudar
        float helpChance = agreeableness * 0.01f;
        
        // Se o NPC for familiar ou amigo, aumentar a chance
        if (other != null && other.relationships.ContainsKey(other.id))
        {
            float relationshipValue = other.relationships[other.id];
            helpChance += (relationshipValue * 0.005f); // Bonus de até +50%
        }
        
        return UnityEngine.Random.value < helpChance;
    }
    
    public bool IsLikelyToTakeRisks()
    {
        // Baixo neuroticismo e alta abertura à experiência aumentam chances de tomar riscos
        float riskChance = ((100f - neuroticism) * 0.003f) + (openness * 0.007f);
        return UnityEngine.Random.value < riskChance;
    }
    
    public bool IsLikelyToBeHonest()
    {
        // Alta conscienciosidade e concordância aumentam chances de honestidade
        float honestyChance = (conscientiousness * 0.006f) + (agreeableness * 0.004f);
        return UnityEngine.Random.value < honestyChance;
    }
    
    public bool IsLikelyToSocialize()
    {
        // Alta extroversão aumenta chances de socialização
        float socializeChance = extraversion * 0.01f;
        return UnityEngine.Random.value < socializeChance;
    }
}