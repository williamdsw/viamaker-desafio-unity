using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InitController : MonoBehaviour
{
    // || Inspector References

    [Header("UI Elements")]
    [SerializeField] private GameObject progressModal;
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("Other Controllers")]
    [SerializeField] private EscolaController escolaController;

    // || State

    private int numberOfExecutedCoroutines = 0;

    // || Cached References

    private RequestService service = new RequestService();
    private DatabaseService database = new DatabaseService();
    private EscolaService escolaService = new EscolaService();
    private TurmaService turmaService = new TurmaService();
    private AlunoService alunoService = new AlunoService();

    private void Start()
    {
        if (FileService.CheckIfFileExists(Environment.DatabaseFilePath))
        {
            StartCoroutine(GotoMain());
        }
        else
        {
            TryFetchData();
        }
    }

    private IEnumerator GotoMain()
    {
        yield return new WaitForSecondsRealtime(2f);
        progressModal.SetActive(false);
        escolaController.Show();
    }

    private void TryFetchData()
    {
        try
        {
            SetMessage("Conectando ao servidor");
            numberOfExecutedCoroutines = 0;
            StartCoroutine(service.GetRequest(Environment.FindSchoolUrl, (response) => CheckEscolaResponse(response)));
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("Exception ex: {0}", ex.Message);
            SetMessage("Ocorreu algum erro junto ao servidor.");
            CreateDatabase(true);
        }
    }

    private void CheckEscolaResponse(string response)
    {
        try
        {
            if (string.IsNullOrEmpty(response) || response.Contains("<html>"))
            {
                throw new Exception(response);
            }

            SetMessage("Dados recuperados!");

            EscolaResponse escolaResponse = JsonUtility.FromJson<EscolaResponse>(response);
            Escola escola = new Escola(escolaResponse.Retorno.Id, escolaResponse.Retorno.Nome);
            List<Turma> turmas = new List<Turma>();
            List<Aluno> alunos = new List<Aluno>();
            for (int index = 0; index < escolaResponse.Retorno.Turmas.Count; index++)
            {
                Turma apiTurma = escolaResponse.Retorno.Turmas[index];
                Dictionary<string, string> keyValues = new Dictionary<string, string>();
                keyValues.Add("turmaId", apiTurma.Id.ToString());
                StartCoroutine(service.PostRequest(Environment.GetStudentsByClassUrl, keyValues, (otherResponse) => 
                {
                    CheckTurmaResponse(otherResponse, apiTurma, escola, turmas, alunos, escolaResponse);
                }));
            }
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("Exception ex: {0}", ex.Message);
            SetMessage("Ocorreu algum erro ao tratar os dados.");
            CreateDatabase(true);
        }
    }

    private void CheckTurmaResponse(string response, Turma apiTurma, Escola escola, 
                                    List<Turma> turmas, List<Aluno> alunos, EscolaResponse escolaResponse)
    {
        try
        {
            if (string.IsNullOrEmpty(response) || response.Contains("<html>"))
            {
                throw new Exception(response);
            }

            AlunosResponse alunosResponse = JsonUtility.FromJson<AlunosResponse>(response);
            Turma turma = new Turma(apiTurma.Id, apiTurma.Nome, escola);
            turmas.Add(turma);

            foreach (Aluno apiAluno in alunosResponse.Retorno)
            {
                alunos.Add(new Aluno(apiAluno.Id, apiAluno.Nome, turma));
            }

            numberOfExecutedCoroutines++;
            if (numberOfExecutedCoroutines == (escolaResponse.Retorno.Turmas.Count))
            {
                CreateDatabase(false);
                FillDatabase(escola, turmas, alunos);
            }
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("Exception ex: {0}", ex.Message);
            SetMessage("Ocorreu algum erro ao tratar os dados.");
            CreateDatabase(true);
        }
    }

    private void CreateDatabase(bool gotoMain)
    {
        try
        {
            SetMessage("Criando base de dados");
            database.CreateTable(Queries.Escola.CreateTable);
            database.CreateTable(Queries.Turma.CreateTable);
            database.CreateTable(Queries.Aluno.CreateTable);

            if (gotoMain)
            {
                StartCoroutine(GotoMain());
            }
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("Exception ex: {0}", ex.Message);
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
            Debug.LogErrorFormat("Exception ex: {0}", ex.Message);
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
