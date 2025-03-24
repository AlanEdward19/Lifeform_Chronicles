using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public ERoomType type;
    public int roomID;
    public Building parentBuilding;
    
    // Estado do quarto
    public bool isEnabled = false;
    public float cleanliness = 100f; // 0 = extremamente sujo, 100 = perfeitamente limpo
    public float temperature = 22f; // em Celsius
    
    // Móveis e objetos interativos no quarto
    public List<InteractableObject> furniture = new List<InteractableObject>();
    
    // Valor do quarto e influências
    public float baseValue = 100f;
    
    private float cleanlinessDecayRate = 0.5f; // Diminuição de limpeza por dia simulado
    
    private void Start()
    {
        // Adicionar móveis específicos com base no tipo do quarto
        AddSpecificFurniture();
    }
    
    private void Update()
    {
        if (isEnabled)
        {
            // Reduzir limpeza gradualmente com o tempo
            float decayAmount = cleanlinessDecayRate * Time.deltaTime / 86400f; // Converter para dia simulado
            cleanliness -= decayAmount;
            cleanliness = Mathf.Max(0f, cleanliness);
        }
    }
    
    public void Enable()
    {
        isEnabled = true;
        
        // Ativar todos os móveis
        foreach (InteractableObject furniture in furniture)
        {
            furniture.gameObject.SetActive(true);
        }
    }
    
    public void Disable()
    {
        isEnabled = false;
        
        // Desativar todos os móveis
        foreach (InteractableObject furniture in furniture)
        {
            furniture.gameObject.SetActive(false);
        }
    }
    
    public void Clean(float amount)
    {
        cleanliness += amount;
        cleanliness = Mathf.Min(100f, cleanliness);
    }
    
    public float CalculateValue()
    {
        // Calcular valor com base na limpeza, móveis, etc.
        float cleanlinessMultiplier = 0.5f + (cleanliness / 100f) * 0.5f;
        
        float furnitureValue = 0f;
        foreach (InteractableObject item in furniture)
        {
            furnitureValue += item.value;
        }
        
        return (baseValue + furnitureValue) * cleanlinessMultiplier;
    }
    
    private void AddSpecificFurniture()
    {
        // Criar e adicionar móveis específicos para cada tipo de quarto
        switch (type)
        {
            case ERoomType.Bedroom:
                AddFurnitureItem("Bed", 200f);
                AddFurnitureItem("Dresser", 100f);
                AddFurnitureItem("Nightstand", 50f);
                break;
                
            case ERoomType.Kitchen:
                AddFurnitureItem("Refrigerator", 300f);
                AddFurnitureItem("Stove", 250f);
                AddFurnitureItem("Counter", 100f);
                AddFurnitureItem("Sink", 150f);
                break;
                
            case ERoomType.Bathroom:
                AddFurnitureItem("Toilet", 150f);
                AddFurnitureItem("Shower", 200f);
                AddFurnitureItem("Sink", 100f);
                break;
                
            case ERoomType.LivingRoom:
                AddFurnitureItem("Sofa", 300f);
                AddFurnitureItem("TV", 250f);
                AddFurnitureItem("CoffeeTable", 100f);
                break;
                
            // Casos para outros tipos de quartos podem ser adicionados aqui
            
            default:
                // Adicionar móveis genéricos
                AddFurnitureItem("Chair", 50f);
                AddFurnitureItem("Table", 100f);
                break;
        }
    }
    
    private void AddFurnitureItem(string itemName, float itemValue)
    {
        // Criar um objeto de móvel (em uma implementação real, você carregaria um prefab)
        GameObject furnitureObj = new GameObject(itemName);
        furnitureObj.transform.SetParent(transform);
        
        // Adicionar componente InteractableObject
        InteractableObject interactable = furnitureObj.AddComponent<InteractableObject>();
        interactable.objectName = itemName;
        interactable.value = itemValue;
        
        // Configurar efeitos do móvel nas necessidades dos NPCs
        ConfigureFurnitureEffects(interactable, itemName);
        
        // Adicionar à lista de móveis
        furniture.Add(interactable);
    }
    
    private void ConfigureFurnitureEffects(InteractableObject furniture, string itemName)
    {
        // Definir possíveis interações e seus efeitos com base no tipo de móvel
        switch (itemName)
        {
            case "Bed":
                furniture.possibleInteractions = new EInteractionType[] { 
                    EInteractionType.Sleep, 
                    EInteractionType.Rest 
                };
                
                // Efeito de dormir
                NeedsEffect sleepEffect = new NeedsEffect {
                    energy = 50f,
                    fun = 5f
                };
                furniture.interactionEffects[EInteractionType.Sleep] = sleepEffect;
                
                // Efeito de descansar
                NeedsEffect restEffect = new NeedsEffect {
                    energy = 10f,
                    fun = 2f
                };
                furniture.interactionEffects[EInteractionType.Rest] = restEffect;
                break;
                
            case "Sofa":
                furniture.possibleInteractions = new EInteractionType[] { 
                    EInteractionType.Rest, 
                    EInteractionType.Watch 
                };
                
                // Efeito de descansar
                NeedsEffect sofaRestEffect = new NeedsEffect {
                    energy = 5f,
                    fun = 5f
                };
                furniture.interactionEffects[EInteractionType.Rest] = sofaRestEffect;
                
                // Efeito de assistir (TV presumivelmente)
                NeedsEffect watchEffect = new NeedsEffect {
                    fun = 15f,
                    social = 0f
                };
                furniture.interactionEffects[EInteractionType.Watch] = watchEffect;
                break;
                
            case "Refrigerator":
                furniture.possibleInteractions = new EInteractionType[] { 
                    EInteractionType.Eat,
                    EInteractionType.Drink
                };
                
                // Efeito de comer
                NeedsEffect eatEffect = new NeedsEffect {
                    hunger = 40f,
                    thirst = 5f
                };
                furniture.interactionEffects[EInteractionType.Eat] = eatEffect;
                
                // Efeito de beber
                NeedsEffect drinkEffect = new NeedsEffect {
                    thirst = 40f
                };
                furniture.interactionEffects[EInteractionType.Drink] = drinkEffect;
                break;
                
            case "Toilet":
                furniture.possibleInteractions = new EInteractionType[] { 
                    EInteractionType.UseBathroom 
                };
                
                // Efeito de usar o banheiro
                NeedsEffect bathroomEffect = new NeedsEffect {
                    bladder = 100f
                };
                furniture.interactionEffects[EInteractionType.UseBathroom] = bathroomEffect;
                break;
                
            case "Shower":
                furniture.possibleInteractions = new EInteractionType[] { 
                    EInteractionType.Shower 
                };
                
                // Efeito de tomar banho
                NeedsEffect showerEffect = new NeedsEffect {
                    hygiene = 100f,
                    fun = 5f
                };
                furniture.interactionEffects[EInteractionType.Shower] = showerEffect;
                break;
                
            // Outros casos podem ser adicionados conforme necessário
            
            default:
                // Interação genérica para móveis não especificados
                furniture.possibleInteractions = new EInteractionType[] { 
                    EInteractionType.Use 
                };
                
                // Efeito genérico
                NeedsEffect useEffect = new NeedsEffect {
                    fun = 2f
                };
                furniture.interactionEffects[EInteractionType.Use] = useEffect;
                break;
        }
    }
}