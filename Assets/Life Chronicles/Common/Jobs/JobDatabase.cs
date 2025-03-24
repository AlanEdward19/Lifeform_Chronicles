using System.Collections.Generic;

public static class JobDatabase
{
    private static Dictionary<EJobType, JobData> jobDataDictionary = new()
    {
        { EJobType.Unemployed, new JobData(0f, 0, 0, 0) },
        { EJobType.Farmer, new JobData(30f, 12, 8, 12) },
        { EJobType.Merchant, new JobData(50f, 8, 10, 12) },
        { EJobType.Guard, new JobData(40f, 12, 10, 8) },
        { EJobType.Doctor, new JobData(70f, 8, 15, 12) },
        { EJobType.Craftsman, new JobData(45f, 12, 12, 10) },
        { EJobType.Teacher, new JobData(40f, 6, 14, 12) },
        { EJobType.Criminal, new JobData(60f, 10, 12, 10) },
        { EJobType.Mayor, new JobData(80f, 8, 12, 14) },
        { EJobType.Superhero, new JobData(20f, 14, 12, 12) },
        { EJobType.Villain, new JobData(100f, 12, 14, 10) }
    };

    // Obter requisitos para um trabalho
    public static JobData GetJobData(EJobType EJobType)
    {
        if (jobDataDictionary.ContainsKey(EJobType))
        {
            return jobDataDictionary[EJobType];
        }
        return jobDataDictionary[EJobType.Unemployed];
    }

    // Calcular salário com base no tipo de trabalho e desempenho
    public static float GetSalary(EJobType EJobType, int performance)
    {
        float baseSalary = GetJobData(EJobType).baseSalary;
        
        // Ajustar com base no desempenho (50% a 150% do salário base)
        return baseSalary * (0.5f + performance / 100f * 1.0f);
    }
    
    // Verificar se um NPC pode realizar um trabalho específico
    public static bool CanTakeJob(NpcController npc, EJobType EJobType)
    {
        JobData data = GetJobData(EJobType);
        
        // Verificar requisitos mínimos
        if (npc.Strength < data.requiredStrength || 
            npc.Intelligence < data.requiredIntelligence || 
            npc.Charisma < data.requiredCharisma)
        {
            return false;
        }
        
        // Verificar idade (não pode trabalhar antes dos 16 anos)
        if (npc.Age < 16)
        {
            return false;
        }
        
        return true;
    }
}