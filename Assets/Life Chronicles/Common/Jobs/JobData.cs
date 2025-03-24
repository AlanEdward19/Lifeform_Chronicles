public class JobData
{
    public float baseSalary;
    public int requiredStrength;
    public int requiredIntelligence;
    public int requiredCharisma;
    
    public JobData(float salary, int strength, int intelligence, int charisma)
    {
        baseSalary = salary;
        requiredStrength = strength;
        requiredIntelligence = intelligence;
        requiredCharisma = charisma;
    }
}