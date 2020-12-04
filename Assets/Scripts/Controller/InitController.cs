using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InitController : MonoBehaviour
{
    [SerializeField] private GameObject progressModal;
    [SerializeField] private TextMeshProUGUI progressText;

    private EscolaController escolaController;

    private RequestService service = new RequestService();
    private DatabaseService database = new DatabaseService();
    private EscolaService escolaService = new EscolaService();
    private TurmaService turmaService = new TurmaService();
    private AlunoService alunoService = new AlunoService();

    private void Awake () 
    {
        escolaController = this.GetComponent<EscolaController>();
    }

    private void Start()
    {
        if (FileService.CheckIfFileExists(Environment.databaseFilePath))
        {
            StartCoroutine(GotoMain());
        }
        else
        {
            FetchData();
        }
    }

    private IEnumerator GotoMain()
    {
        yield return new WaitForSecondsRealtime(2f);
        progressModal.SetActive(false);
        escolaController.Show();
    }

    private void FetchData()
    {
        try
        {
            SetMessage("Conectando ao servidor");
            int numberOfExecutedCoroutines = 0;
            StartCoroutine(service.GetRequest(Environment.FindSchoolUrl, (response) =>
            {
                SetMessage("Dados recuperados!");

                EscolaResponse escolaResponse = JsonUtility.FromJson<EscolaResponse>(response);
                Escola escola = new Escola(escolaResponse.Retorno.Id, escolaResponse.Retorno.Nome);
                List<Turma> turmas = new List<Turma>();
                List<Aluno> alunos = new List<Aluno>();
                for (int index = 0; index < escolaResponse.Retorno.Turmas.Count; index++)
                {
                    var apiTurma = escolaResponse.Retorno.Turmas[index];
                    Dictionary<string, string> keyValues = new Dictionary<string, string>();
                    keyValues.Add("turmaId", apiTurma.Id.ToString());
                    StartCoroutine(service.PostRequest(Environment.GetStudentsByClassUrl, keyValues, (otherResponse) =>
                    {
                        AlunosResponse alunosResponse = JsonUtility.FromJson<AlunosResponse>(otherResponse);
                        Turma turma = new Turma(apiTurma.Id, apiTurma.Nome, escola);
                        turmas.Add(turma);

                        foreach (var apiAluno in alunosResponse.Retorno)
                        {
                            alunos.Add(new Aluno(apiAluno.Id, apiAluno.Nome, turma));
                        }

                        numberOfExecutedCoroutines++;
                        if (numberOfExecutedCoroutines == (escolaResponse.Retorno.Turmas.Count))
                        {
                            CreateDatabase();
                            FillDatabase(escola, turmas, alunos);
                        }
                    }));
                }
            }));
        }
        catch (Exception ex)
        {
            Debug.Log("ex: " + ex.Message);
            SetMessage("Ocorreu algum erro junto ao servidor.");
            CreateDatabase();
        }
    }

    private void CreateDatabase()
    {
        try
        {
            SetMessage("Criando base de dados");
            database.CreateTable(Environment.CreateTableEscola);
            database.CreateTable(Environment.CreateTableTurma);
            database.CreateTable(Environment.CreateTableAluno);
        }
        catch (Exception ex)
        {
            Debug.Log("ex: " + ex.Message);
            SetMessage("Erro ao criar a base de dados!");
            StartCoroutine(GotoMain());
        }
    }

    private void FillDatabase(Escola escola, List<Turma> turmas, List<Aluno> alunos)
    {
        try
        {
            if (escolaService.Insert(escola))
            {
                if (turmaService.InsertMultiples(turmas))
                {
                    if (alunoService.InsertMultiples(alunos))
                    {
                        SetMessage("Dados importados!");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log("ex: " + ex.Message);
            SetMessage("Erro ao importar os dados!");
        }
        finally
        {
            StartCoroutine(GotoMain());
        }
    }

    private void SetMessage(string message)
    {
        if (progressText)
        {
            progressText.SetText(message);
        }
    }
}
