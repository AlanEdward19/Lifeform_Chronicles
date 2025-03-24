using System;
using System.Collections.Generic;

public class Bench : InteractableObject
{
    public Bench()
    {
        objectName = "Bench";
        value = 200f;
        durability = 100f;
        maxDurability = 100f;

        Dictionary<EInteractionType, NeedsEffect> interactions = new();
        Dictionary<EInteractionType, float> interactionDurations = new();
            
        NeedsEffect restEffect = new NeedsEffect {
            energy = 10f,
        };
                
        interactions[EInteractionType.Rest] = restEffect;
        
        ConfigureFurnitureEffects(interactions, interactionDurations);
    }

    private void Start()
    {
        WorldSimulationManager.Instance.RegisterInteractableObject(this);
    }
}