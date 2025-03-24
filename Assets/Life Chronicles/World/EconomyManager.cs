using System.Collections.Generic;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    // Singleton
    public static EconomyManager Instance;
    
    // Registros de propriedades e valores
    private Dictionary<int, float> propertyValues = new Dictionary<int, float>();
    
    // Taxa de imposto
    public float taxRate = 0.1f;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    public void RegisterPropertyValue(int propertyID, float value)
    {
        propertyValues[propertyID] = value;
    }
    
    public float GetPropertyValue(int propertyID)
    {
        if (propertyValues.ContainsKey(propertyID))
            return propertyValues[propertyID];
        return 0f;
    }
    
    public float CalculateTaxes(int npcID, float income)
    {
        // Fórmula simples de imposto
        return income * taxRate;
    }
}