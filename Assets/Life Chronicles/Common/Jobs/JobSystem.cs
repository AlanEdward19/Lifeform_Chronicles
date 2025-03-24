using System.Collections.Generic;
using UnityEngine;

public class JobSystem
{
    public static EJobType FindSuitableJob(NpcController npc)
    {
        List<EJobType> suitableJobs = new List<EJobType>();
        
        // Verificar todos os tipos de trabalho
        foreach (EJobType EJobType in System.Enum.GetValues(typeof(EJobType)))
        {
            if (EJobType != EJobType.Unemployed && JobDatabase.CanTakeJob(npc, EJobType))
            {
                suitableJobs.Add(EJobType);
            }
        }
        
        // Se não houver empregos adequados, ficar desempregado
        if (suitableJobs.Count == 0)
        {
            return EJobType.Unemployed;
        }
        
        // Escolher aleatoriamente entre os trabalhos adequados
        return suitableJobs[Random.Range(0, suitableJobs.Count)];
    }
}