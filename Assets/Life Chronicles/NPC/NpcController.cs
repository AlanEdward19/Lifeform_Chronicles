using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;

public class NpcController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    #region Identificação

    public string name;
    public int id;
    public Building Home;

    #endregion

    #region Atributos básicos

    [SerializeField] private int age = 20;
    [SerializeField] private EGender gender;
    private Personality personality;

    #endregion

    #region Informações de Familia

    [SerializeField] private int fatherID = -1; // -1 indica que não tem pai
    [SerializeField] private int motherID = -1; // -1 indica que não tem mãe
    public List<int> childrenIDs = new List<int>();

    #endregion
    
    #region Necessidades

    [SerializeField] private float hunger = 100f;
    [SerializeField] private float thirst = 100f;
    [SerializeField] private float energy = 100f;
    [SerializeField] private float social = 100f;
    [SerializeField] private float hygiene = 100f;
    [SerializeField] private float bladder = 100f;
    [SerializeField] private float fun = 100f;

    #endregion
    
    #region Atributos

    [SerializeField] private int strength = 10;
    [SerializeField] private int intelligence = 10;
    [SerializeField] private int wisdom = 10;
    [SerializeField] private int constitution = 10;
    [SerializeField] private int dexterity = 10;
    [SerializeField] private int charisma = 10;

    #endregion

    #region Sistema de moral e reputação

    [SerializeField] private float morality = 50f; // 0-100, onde 0 é mal e 100 é bom
    public Dictionary<int, float> relationships = new Dictionary<int, float>(); // ID do NPC -> valor do relacionamento

    #endregion

    #region Trabalho

    [SerializeField] private EJobType currentJob = EJobType.Unemployed;
    [SerializeField] private float money = 100f;
    [SerializeField] private int jobPerformance = 50; // 0-100

    #endregion

    #region GOAP
    
    private bool isPerformingAction = false;

    #endregion
    
    #region Memória

    public MemorySystem memorySystem { get; set; }

    #endregion

    #region Propriedades publicas

    public int Age { get => age; set => age = value; }
    public EGender Gender { get => gender; }
    public int FatherID { get => fatherID; }
    public int MotherID { get => motherID; }
    public EJobType Job { get => currentJob; }
    public int Strength { get => strength; }
    public int Intelligence { get => intelligence; }
    public int Wisdom { get => wisdom; }
    public int Constitution { get => constitution; }
    public int Dexterity { get => dexterity; }
    public int Charisma { get => charisma; }

    #endregion
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeNPC();
        WorldSimulationManager.Instance.RegisterNPC(this);
        
        // Inicializar sistema de memória
        memorySystem = GetComponent<MemorySystem>();
        
        navMeshAgent = GetComponent<NavMeshAgent>();

        Building home = WorldSimulationManager.Instance.FindNpcHome(this);

        if (home == null)
        {
            if (FatherID != -1)
            {
                var father = WorldSimulationManager.Instance.GetNPCById(FatherID);
                
                if (father != null)
                    Home = father.Home;
            }
            else if (MotherID != -1)
            {
                var mother = WorldSimulationManager.Instance.GetNPCById(MotherID);

                if (mother != null)
                    Home = mother.Home;
            }
        }
        else
            Home = home;
    }

    #region Ações

    private IEnumerator MoveToCoroutine(Vector3 destination)
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.SetDestination(destination);
            navMeshAgent.isStopped = false;

            while (navMeshAgent.pathPending || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                yield return null;
            }

            navMeshAgent.isStopped = true;
        }
    }

    private void MoveTo(Vector3 destination)
    {
        StartCoroutine(MoveToCoroutine(destination));
    }
    
    public void StopMoving()
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true;
        }
    }
    
    private void FellAsleep()
    {
        // Implementação para dormir
        Debug.Log("Fell Asleep");
    }
    
    private void PeeInPants()
    {
        // Implementação para urinar nas calças
        Debug.Log("Pee In Pants");
    }
    
    private void LookForWork()
    {
        EJobType job = JobSystem.FindSuitableJob(this);

        Building? building = WorldSimulationManager.Instance.GetBuildingByJob(job);
        
        if (building != null)
        {
            MoveTo(building.transform.position);
            building.Hire(this, job);
        }
    }
    
    private void LookForHome()
    {
        Building? building = WorldSimulationManager.Instance.FindAvailableHome(money);
        
        if (building != null)
        {
            MoveTo(building.transform.position);
            building.Hire(this, Job);
            Home = building;
        }
    }
    
    private void HandleHunger()
    {
        // Implementação para lidar com a fome
        Debug.Log("Handling Hunger");
    }

    private void HandleThirst()
    {
        // Implementação para lidar com a sede
        Debug.Log("Handling Thirst");
    }

    private void HandleEnergy()
    {
        // Implementação para lidar com a energia
        Debug.Log("Handling Energy");
    }

    private void HandleBladder()
    {
        // Implementação para lidar com a bexiga
        Debug.Log("Handling Bladder");
    }

    private void HandleHygiene()
    {
        // Implementação para lidar com a higiene
        Debug.Log("Handling Hygiene");
    }

    private void HandleSocial()
    {
        // Implementação para lidar com a socialização
        Debug.Log("Handling Social");
    }

    private void HandleFun()
    {
        // Implementação para lidar com a diversão
        Debug.Log("Handling Fun");
    }
    
    public void AssignJob(EJobType job)
    {
        currentJob = job;
    }

    #endregion

    // Update is called once per frame
    void Update()
    {
        // Atualizar necessidades ao longo do tempo
        UpdateNeeds(Time.deltaTime);
        
        DecideNewGoal();
    }
    
    private void InitializeNPC()
    {
        // Definir idade inicial e gênero
        gender = (EGender)UnityEngine.Random.Range(0, Enum.GetValues(typeof(EGender)).Length);
        
        // Se não tem pais (primeiros NPCs), usar atributos aleatórios
        if (fatherID == -1 && motherID == -1)
        {
            // Gerar atributos aleatórios entre 8 e 12
            strength = UnityEngine.Random.Range(8, 13);
            intelligence = UnityEngine.Random.Range(8, 13);
            wisdom = UnityEngine.Random.Range(8, 13);
            constitution = UnityEngine.Random.Range(8, 13);
            dexterity = UnityEngine.Random.Range(8, 13);
            charisma = UnityEngine.Random.Range(8, 13);
        }
        
        // Inicializar personalidade
        personality = new Personality();
        personality.GeneratePersonality();
        
        // Inicializar estado como adulto jovem se for primeira geração
        if (fatherID == -1 && motherID == -1)
        {
            age = UnityEngine.Random.Range(18, 30);
        }
        // Bebês começam com idade 0
        else
        {
            age = 0;
        }
    }
    
    private void RegisterAvailableActions()
    {
        // Adicionar todas as ações que este NPC pode realizar
        // Exemplos: Comer, Dormir, Trabalhar, Socializar, Construir, etc.
    }
    
    private void UpdateNeeds(float deltaTime)
    {
        // Diminuir todas as necessidades com o tempo
        hunger -= 0.5f * deltaTime;
        thirst -= 0.7f * deltaTime;
        energy -= 0.3f * deltaTime;
        social -= 0.2f * deltaTime;
        hygiene -= 0.4f * deltaTime;
        bladder -= 0.6f * deltaTime;
        fun -= 0.4f * deltaTime;
        
        // Limitar valores entre 0 e 100
        hunger = Mathf.Clamp(hunger, 0f, 100f);
        thirst = Mathf.Clamp(thirst, 0f, 100f);
        energy = Mathf.Clamp(energy, 0f, 100f);
        social = Mathf.Clamp(social, 0f, 100f);
        hygiene = Mathf.Clamp(hygiene, 0f, 100f);
        bladder = Mathf.Clamp(bladder, 0f, 100f);
        fun = Mathf.Clamp(fun, 0f, 100f);

        if (hunger == 0)
        {
            StopMoving();
            Die(EDeathCause.Starvation);
        }
            
        else if (thirst == 0)
        {
            StopMoving();
            Die(EDeathCause.Dehydration);
        }
        else if (energy == 0)
        {
            StopMoving();
            FellAsleep();
        }
        else if (bladder == 0)
        {
            StopMoving();
            PeeInPants();
        }
    }
    
    private void DecideNewGoal()
    {
        if (!isPerformingAction)
        {
            if (hunger < 20f)
            {
                HandleHunger();
            }
            else if (thirst < 20f)
            {
                HandleThirst();
            }
            else if (energy < 20f)
            {
                HandleEnergy();
            }
            else if (bladder < 20f)
            {
                HandleBladder();
            }
            else if (hygiene < 20f)
            {
                HandleHygiene();
            }
            else if (social < 20f)
            {
                HandleSocial();
            }
            else if (fun < 20f)
            {
                HandleFun();
            }
            else if (Home == null)
            {
                LookForHome();
            }
            else if (Job == EJobType.Unemployed)
            {
                LookForWork();
            }
        }
    }
    
    // Método para reprodução
    public bool CanReproduce()
    {
        // Verificar se é mulher e está em idade reprodutiva (entre 18 e 45 anos)
        return gender == EGender.Female && age >= 18 && age <= 45;
    }

    public void OnNewMemoryAdded(Memory memory)
    {
        //Reação
    }

    public void ChangeNeed(ENeed need, float value)
    {
        switch (need)
        {
            case ENeed.Hunger:
                hunger += value;
                break;
            case ENeed.Thirst:
                thirst += value;
                break;
            case ENeed.Energy:
                energy += value;
                break;
            case ENeed.Hygiene:
                hygiene += value;
                break;
            case ENeed.Bladder:
                bladder += value;
                break;
            case ENeed.Social:
                social += value;
                break;
            case ENeed.Fun:
                fun += value;
                break;
        }
    }
    
    // Método para criar um filho
    public NpcController HaveChild(NpcController otherParent)
    {
        if (!CanReproduce())
            return null;
        
        // Determinar qual é o pai e qual é a mãe
        NpcController mother = this;
        NpcController father = otherParent;
        
        if (gender != EGender.Female)
        {
            mother = otherParent;
            father = this;
            
            if (mother.gender != EGender.Female)
                return null; // Ambos não são mulheres, não podem ter filhos
        }
        
        // Criar um novo NPC
        GameObject childObject = new GameObject("NPC Child");
        NpcController child = childObject.AddComponent<NpcController>();
        
        // Configurar IDs dos pais
        int childID = WorldSimulationManager.Instance.GetNextid();
        child.id = childID;
        child.fatherID = father.id;
        child.motherID = mother.id;
        
        // Adicionar o ID do filho à lista de filhos dos pais
        mother.childrenIDs.Add(childID);
        father.childrenIDs.Add(childID);
        
        // Gerar atributos usando algoritmo genético
        GeneticAttributeCalculator.CalculateChildAttributes(father, mother, child);
        
        // Gerar nome (pode ser implementado depois)
        child.name = "Child of " + father.name + " and " + mother.name;
        
        // Criar memória do nascimento
        Memory birthMemory =MemoryFactory.CreateChildbirthMemory(mother.id, father.id, child.id, child.name);
        
        // Adicionar a memória aos pais
        mother.memorySystem.AddMemory(birthMemory);
        father.memorySystem.AddMemory(birthMemory);
        
        WorldSimulationManager.Instance.RegisterNPC(child);
        
        return child;
    }
    
    // Métodos para relacionamentos
    public void UpdateRelationship(int otherid, float delta)
    {
        if (!relationships.ContainsKey(otherid))
            relationships[otherid] = 50f; // Valor neutro inicial
            
        relationships[otherid] = Mathf.Clamp(relationships[otherid] + delta, 0f, 100f);
    }
    
    // Métodos para profissão
    public void GoToWork()
    {
        // Lógica de trabalho
    }
    
    public void GetPaid()
    {
        money += JobDatabase.GetSalary(currentJob, jobPerformance);
    }

    public void AddMoney(float amount)
    {
        money += amount;
    }
    
    // Método para envelhecer o NPC
    public void Age1Year()
    {
        age++;
        
        // Aplicar efeitos da idade
        if (age > 60)
        {
            // Diminuir gradualmente alguns atributos com a idade avançada
            if (UnityEngine.Random.value < 0.1f)
            {
                strength = Mathf.Max(strength - 1, 1);
            }
            if (UnityEngine.Random.value < 0.1f)
            {
                constitution = Mathf.Max(constitution - 1, 1);
            }
        }
        
        // Verificar morte por velhice (chance aumenta após 70 anos)
        if (age > 70)
        {
            float deathChance = (age - 70) * 0.05f;
            if (UnityEngine.Random.value < deathChance)
            {
                Die(EDeathCause.OldAge);
            }
        }
    }
    
    // Método para morte do NPC
    public void Die(EDeathCause cause)
    {
        // Criar memória para todos os NPCs relacionados
        Memory deathMemory = MemoryFactory.CreateDeathMemory(id, cause, new List<int>());
        
        // Notificar familiares
        foreach (NpcController npc in WorldSimulationManager.Instance.allNPCs)
        {
            if (npc.fatherID == id || npc.motherID == id || 
                childrenIDs.Contains(npc.id) || npc.id == fatherID || npc.id == motherID)
            {
                npc.memorySystem.AddMemory(deathMemory);
                // Afetar relacionamentos e estado emocional
            }
        }
        
        // Processar herança, propriedades, etc.
        
        // Remover do mundo
        WorldSimulationManager.Instance.allNPCs.Remove(this);
        Destroy(gameObject);
    }
}
