using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TesteController : MonoBehaviour
{
    private RequestService service = new RequestService();
    private DatabaseService database = new DatabaseService();
    private EscolaService escolaService = new EscolaService();
    private TurmaService turmaService = new TurmaService();
    private AlunoService alunoService = new AlunoService();

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            if (FileService.CheckIfFileExists(Environment.databaseFilePath))
            //if (false)
            {
                Debug.Log("Database exists...");
            }
            else
            {
                InitialConfig();
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Captured ex: " + ex);
        }
    }

    private void InitialConfig()
    {
        try
        {
            Debug.Log("Database doesn't exists");
            FetchData();
        }
        catch (System.Exception ex)
        {
            Debug.Log("Captured ex: " + ex);
        }
    }

    private void FetchData()
    {
        try
        {
            int numberOfExecutedCoroutines = 0;
            StartCoroutine(service.GetRequest(Environment.FindSchoolUrl, (response) =>
            {
                EscolaResponse escolaResponse = JsonUtility.FromJson<EscolaResponse>(response);
                Escola escola = new Escola(escolaResponse.Retorno.Nome);
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
                        Turma turma = new Turma(apiTurma.Nome, escola);
                        turmas.Add(turma);

                        foreach (var apiAluno in alunosResponse.Retorno)
                        {
                            alunos.Add(new Aluno(apiAluno.Nome, turma));
                        }

                        numberOfExecutedCoroutines++;
                        if (numberOfExecutedCoroutines == (escolaResponse.Retorno.Turmas.Count))
                        {
                            CreateDatabaseWithData(escola, turmas, alunos);
                        }
                    }));
                }
            }));
        }
        catch (System.Exception ex)
        {
            Debug.Log("Erro SQL!");
            Debug.Log(ex.Message);
        }
    }

    private void CreateDatabaseWithData(Escola escola, List<Turma> turmas, List<Aluno> alunos)
    {
        try
        {
            database.CreateTable(Environment.CreateTableEscola);
            database.CreateTable(Environment.CreateTableTurma);
            database.CreateTable(Environment.CreateTableAluno);

            if (escolaService.Insert(escola))
            {
                if (turmaService.InsertMultiples(turmas))
                {
                    alunoService.InsertMultiples(alunos);
                    Debug.Log("Done?");
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Erro SQL!");
            Debug.Log(ex.Message);
        }
    }



}
