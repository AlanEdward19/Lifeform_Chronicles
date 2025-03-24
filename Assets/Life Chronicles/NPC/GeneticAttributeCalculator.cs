using UnityEngine;

public class GeneticAttributeCalculator
{
        // Constante para controlar a variação genética
    private const float MUTATION_CHANCE = 0.1f;
    private const int MUTATION_RANGE = 2;
    
    // Método principal para calcular atributos do filho baseado nos pais
    public static void CalculateChildAttributes(NpcController father, NpcController mother, NpcController child)
    {
        // Gerar atributos baseados em genética dos pais
        int strength = CalculateAttribute(father.Strength, mother.Strength);
        int intelligence = CalculateAttribute(father.Intelligence, mother.Intelligence);
        int wisdom = CalculateAttribute(father.Wisdom, mother.Wisdom);
        int constitution = CalculateAttribute(father.Constitution, mother.Constitution);
        int dexterity = CalculateAttribute(father.Dexterity, mother.Dexterity);
        int charisma = CalculateAttribute(father.Charisma, mother.Charisma);
        
        // Atribuir valores ao filho (usando reflexão para acessar campos privados)
        var strengthField = typeof(NpcController).GetField("strength", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        strengthField.SetValue(child, strength);
        
        var intelligenceField = typeof(NpcController).GetField("intelligence", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        intelligenceField.SetValue(child, intelligence);
        
        var wisdomField = typeof(NpcController).GetField("wisdom", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        wisdomField.SetValue(child, wisdom);
        
        var constitutionField = typeof(NpcController).GetField("constitution", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        constitutionField.SetValue(child, constitution);
        
        var dexterityField = typeof(NpcController).GetField("dexterity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        dexterityField.SetValue(child, dexterity);
        
        var charismaField = typeof(NpcController).GetField("charisma", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        charismaField.SetValue(child, charisma);
        
        // Definir gênero aleatoriamente
        var EGenderField = typeof(NpcController).GetField("EGender", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        EGender childEGender = (EGender)Random.Range(0, System.Enum.GetValues(typeof(EGender)).Length);
        EGenderField.SetValue(child, childEGender);
    }
    
    // Método para calcular um atributo individual
    private static int CalculateAttribute(int fatherAttribute, int motherAttribute)
    {
        // Determinar qual gene será dominante (50% de chance para cada pai)
        bool useFatherDominant = Random.value < 0.5f;
        
        // Calcular o valor base combinando os valores dos pais
        // O valor dominante tem 70% de peso, o recessivo 30%
        float baseValue;
        if (useFatherDominant)
        {
            baseValue = (fatherAttribute * 0.7f) + (motherAttribute * 0.3f);
        }
        else
        {
            baseValue = (motherAttribute * 0.7f) + (fatherAttribute * 0.3f);
        }
        
        // Arredondar para o inteiro mais próximo
        int attribute = Mathf.RoundToInt(baseValue);
        
        // Chance de mutação (variação aleatória)
        if (Random.value < MUTATION_CHANCE)
        {
            // Adicionar ou subtrair um valor pequeno aleatório
            attribute += Random.Range(-MUTATION_RANGE, MUTATION_RANGE + 1);
        }
        
        // Garantir que o atributo fique dentro dos limites razoáveis (1-20)
        attribute = Mathf.Clamp(attribute, 1, 20);
        
        return attribute;
    }
}