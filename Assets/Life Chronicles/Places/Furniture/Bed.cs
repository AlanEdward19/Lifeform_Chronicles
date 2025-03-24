using System.Collections.Generic;

public class Bed : InteractableObject
{
    public Bed()
    {
        objectName = "Bed";
        value = 200f;
        durability = 100f;
        maxDurability = 100f;

        Dictionary<EInteractionType, NeedsEffect> interactions = new();
        Dictionary<EInteractionType, float> interactionDurations = new();
            
        NeedsEffect sleepEffect = new NeedsEffect {
            energy = 50f,
            fun = 5f
        };
                
        interactions[EInteractionType.Sleep] = sleepEffect;
        
        ConfigureFurnitureEffects(interactions, interactionDurations);
    }
}