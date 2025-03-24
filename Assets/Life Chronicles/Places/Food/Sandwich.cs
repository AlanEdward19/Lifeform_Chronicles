using System.Collections.Generic;
using System.Linq;

public class Sandwich : InteractableObject
{
    public Sandwich()
    {
        objectName = "Sandwich";
        value = 25;
        durability = 100f;
        maxDurability = 100f;

        Dictionary<EInteractionType, NeedsEffect> interactions = new();
        Dictionary<EInteractionType, float> interactionDurations = new();
            
        NeedsEffect eatEffect = new NeedsEffect {
            hunger = 5f,
            fun = 2f
        };
                
        interactions[EInteractionType.Eat] = eatEffect;
        
        ConfigureFurnitureEffects(interactions, interactionDurations);
    }

    public override void Interact(NpcController npc, EInteractionType EInteractionType)
    {
        base.Interact(npc, EInteractionType);
        
        WorldSimulationManager.Instance.RemoveInteractableObject(this);
        WorldSimulationManager.Instance.allBuildings.ForEach(x => x.RemoveInteractable(this));
        
        Destroy(gameObject);
    }
}