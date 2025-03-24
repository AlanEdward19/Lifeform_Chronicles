using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public bool beingInteracted = false;
    public string objectName;
    public float value = 100f; // Valor monetário do objeto
    public float durability = 100f; // 0-100, representa quanto o objeto está desgastado
    public float maxDurability = 100f;
    
    // Efeitos nas necessidades para cada interação
    public Dictionary<EInteractionType, NeedsEffect> possibleInteractions = new Dictionary<EInteractionType, NeedsEffect>();
    
    // Tempo necessário para cada interação (em segundos simulados)
    public Dictionary<EInteractionType, float> interactionDurations = new Dictionary<EInteractionType, float>();
    
    // Método para realizar interação
    public virtual void Interact(NpcController npc, EInteractionType EInteractionType)
    {
        // Verificar se a interação é possível
        if (!possibleInteractions.ContainsKey(EInteractionType))
        {
            Debug.LogWarning($"NPC {npc.name} tentou {EInteractionType} com {objectName}, mas essa interação não é possível.");
            return;
        }
        
        // Verificar se o objeto está funcionando (durabilidade > 0)
        if (durability <= 0)
        {
            Debug.LogWarning($"{objectName} está quebrado e precisa de reparos.");
            // Pedir reparo ao invés de interagir
            RequestRepair(npc);
            return;
        }
        
        // Aplicar efeitos nas necessidades do NPC
        if (possibleInteractions.ContainsKey(EInteractionType))
        {
            beingInteracted = true;
            NeedsEffect effect = possibleInteractions[EInteractionType];
            
            // Diminuir a durabilidade com o uso
            float usageFactor = GetUsageFactor(EInteractionType);
            durability -= usageFactor;
            durability = Mathf.Max(0f, durability);
            
            // Aplicar efeitos nas necessidades, considerando a durabilidade
            float effectMultiplier = durability / maxDurability;
            ApplyNeedsEffect(npc, effect, effectMultiplier);
            
            // Animar o NPC (em uma implementação real)
            AnimateInteraction(npc, EInteractionType);
            beingInteracted = false;
        }
    }
    
    // Método para solicitar reparo do objeto
    private void RequestRepair(NpcController npc)
    {
        // Verificar se o NPC pode reparar
        if (npc.Job == EJobType.Craftsman || npc.Intelligence > 10)
        {
            // O NPC pode tentar reparar
            RepairObject(npc);
        }
        else
        {
            // O NPC não pode reparar, deve chamar alguém que possa
            NpcController repairNPC = WorldSimulationManager.Instance.FindWorker(EJobType.Craftsman);
            if (repairNPC != null)
            {
                // Aqui seria implementado o GOAP para o repairNPC vir e consertar
                Debug.Log($"NPC {repairNPC.name} foi chamado para reparar {objectName}");
            }
        }
    }
    
    // Método para reparar o objeto
    public void RepairObject(NpcController npc)
    {
        float repairAmount = 10f + (npc.Intelligence * 2f);
        durability += repairAmount;
        durability = Mathf.Min(durability, maxDurability);
    }
    
    private float GetUsageFactor(EInteractionType EInteractionType)
    {
        // Diferentes interações causam desgaste diferente
        switch (EInteractionType)
        {
            case EInteractionType.Sleep: return 0.5f;
            case EInteractionType.Eat: return 1.0f;
            case EInteractionType.Cook: return 2.0f;
            case EInteractionType.Exercise: return 3.0f;
            default: return 1.0f;
        }
    }
    
    private void ApplyNeedsEffect(NpcController npc, NeedsEffect effect, float multiplier)
    {
        // Aplicar efeitos nas necessidades do NPC
        if (effect.hunger != 0) 
            npc.ChangeNeed(ENeed.Hunger, effect.hunger * multiplier);
        if (effect.thirst != 0) 
            npc.ChangeNeed(ENeed.Thirst, effect.thirst * multiplier);
        if (effect.energy != 0) 
            npc.ChangeNeed(ENeed.Energy, effect.energy * multiplier);
        if (effect.social != 0) 
            npc.ChangeNeed(ENeed.Social, effect.social * multiplier);
        if (effect.hygiene != 0) 
            npc.ChangeNeed(ENeed.Hygiene, effect.hygiene * multiplier);
        if (effect.bladder != 0) 
            npc.ChangeNeed(ENeed.Bladder, effect.bladder * multiplier);
        if (effect.fun != 0) npc.
            ChangeNeed(ENeed.Fun, effect.fun * multiplier);
    }
    
    public float GetEffectValue(EInteractionType EInteractionType, ENeed need)
    {
        if (possibleInteractions.ContainsKey(EInteractionType))
        {
            NeedsEffect effect = possibleInteractions[EInteractionType];
            switch (need)
            {
                case ENeed.Hunger: return effect.hunger;
                case ENeed.Thirst: return effect.thirst;
                case ENeed.Energy: return effect.energy;
                case ENeed.Social: return effect.social;
                case ENeed.Hygiene: return effect.hygiene;
                case ENeed.Bladder: return effect.bladder;
                case ENeed.Fun: return effect.fun;
            }
        }
        return 0f;
    }
    
    private void AnimateInteraction(NpcController npc, EInteractionType EInteractionType)
    {
        // Em uma implementação real, aqui seriam disparadas animações
        // Por exemplo:
        // npc.PlayAnimation(EInteractionType.ToString());
        
        Debug.Log($"NPC {npc.name} está {EInteractionType} com {objectName}");
    }
    
    // Método para obter o valor atual (considera a durabilidade)
    public float GetCurrentValue()
    {
        float durabilityFactor = durability / maxDurability;
        return value * durabilityFactor;
    }

    protected void ConfigureFurnitureEffects(Dictionary<EInteractionType, NeedsEffect> interactions, Dictionary<EInteractionType, float> interactionDurations)
    {
        possibleInteractions = interactions;
        this.interactionDurations = interactionDurations;
    }
}