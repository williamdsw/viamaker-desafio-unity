using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlunoController : MonoBehaviour
{
    // || Inspector References

    [Header("Main Container")]
    [SerializeField] private GameObject alunoContainer;
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private GameObject content;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button newButton;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject emptyText;

    [Header("Create | Update Container")]
    [SerializeField] private GameObject createUpdateContainer;
    [SerializeField] private TextMeshProUGUI headerTitle;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button discardButton;
    [SerializeField] private Button acceptButton;

    [Header("Other Controllers")]
    [SerializeField] private MessageModalController modalController;
    [SerializeField] private ConfirmModalController confirmModalController;

    // || Cached References

    private TurmaService turmaService = new TurmaService();
    private AlunoService alunoService = new AlunoService();
    private Turma localTurma = new Turma();
    private Aluno localAluno = new Aluno();

    private void Awake()
    {
        this.localAluno.Id = 0;
    }

    private void Start()
    {
        BindEventListeners();
    }

    private void BindEventListeners()
    {
        if (createUpdateContainer)
        {
            if (closeButton && newButton && discardButton && acceptButton && headerTitle && nameInput)
            {
                closeButton.onClick.AddListener(() => alunoContainer.SetActive(false));
                newButton.onClick.AddListener(() => ToggleCreateUpdateContainer(null, true));
                discardButton.onClick.AddListener(() => ToggleCreateUpdateContainer(null, false));
                acceptButton.onClick.AddListener(HandleOnClick);
            }
        }
    }

    public void Show(Turma turma)
    {
        if (alunoContainer)
        {
            this.localTurma = turma;
            alunoContainer.SetActive(true);
            LoadAlunosByTurma(turma);
        }
    }

    private void LoadAlunosByTurma(Turma turma)
    {
        try
        {
            if (content && itemPrefab && emptyText)
            {
                for (int index = 0; index < content.transform.childCount; index++)
                {
                    Transform child = content.transform.GetChild(index);
                    Destroy(child.gameObject);
                }

                List<Aluno> alunos = alunoService.FindByTurma(turma.Id);
                emptyText.SetActive((alunos.Count == 0));
                scrollView.transform.parent.gameObject.SetActive((alunos.Count != 0));

                if (alunos.Count == 0)
                {
                    return;
                }

                foreach (Aluno aluno in alunos)
                {
                    GameObject item = Instantiate(itemPrefab) as GameObject;
                    item.transform.SetParent(content.transform, false);

                    // Children
                    Transform nameText = item.transform.Find("NameText");
                    Transform deleteButton = item.transform.Find("DeleteButton");
                    Transform editButton = item.transform.Find("EditButton");

                    if (nameText)
                    {
                        TextMeshProUGUI textMesh = nameText.GetComponent<TextMeshProUGUI>();
                        textMesh.SetText(aluno.Nome);
                    }

                    if (deleteButton)
                    {
                        Button button = deleteButton.GetComponent<Button>();
                        button.onClick.AddListener(() => HandleOnDelete(aluno, turma));
                    }

                    if (editButton)
                    {
                        Button button = editButton.GetComponent<Button>();
                        button.onClick.AddListener(() => ToggleCreateUpdateContainer(aluno, true));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            modalController.Show("Atenção", ex.Message, Color.red);
        }
    }

    private void ToggleCreateUpdateContainer(Aluno aluno, bool show)
    {
        this.localAluno.Id = (aluno != null && show ? aluno.Id : 0);
        this.localAluno.Nome = (aluno != null && show ? aluno.Nome : null);
        string title = (aluno == null ? "Novo Aluno" : "Atualizar Aluno");
        nameInput.text = this.localAluno.Nome;
        headerTitle.SetText(title);
        createUpdateContainer.SetActive(show);
    }

    private void HandleOnClick()
    {
        try
        {
            string name = nameInput.text;
            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
            {
                modalController.Show("Atenção", "É necessário informar o Nome!", Color.gray);
                return;
            }

            this.localAluno.Nome = name;
            this.localAluno.Turma = this.localTurma;
            bool hasExecuted = (localAluno.Id == 0 ? alunoService.Insert(localAluno) : alunoService.Update(localAluno));
            string title = (hasExecuted ? "Sucesso" : "Falha");
            string message = (hasExecuted ? "Operação concluída!" : "Erro na operação!");
            modalController.Show(title, message, (hasExecuted ? Color.green : Color.red));
            ToggleCreateUpdateContainer(null, false);
            LoadAlunosByTurma(this.localTurma);
        }
        catch (Exception ex)
        {
            modalController.Show("Atenção", ex.Message, Color.red);
        }
    }

    private void HandleOnDelete(Aluno aluno, Turma turma)
    {
        confirmModalController.Show(response =>
        {
            if (response)
            {
                try
                {
                    bool hasDeleted = alunoService.DeleteById(aluno.Id);
                    string title = (hasDeleted ? "Sucesso" : "Falha");
                    string message = (hasDeleted ? "Operação concluída!" : "Erro na operação!");
                    modalController.Show(title, message, (hasDeleted ? Color.green : Color.red));

                    if (hasDeleted)
                    {
                        LoadAlunosByTurma(turma);
                    }
                }
                catch (Exception ex)
                {
                    modalController.Show("Atenção", ex.Message, Color.red);
                }
            }
        });
    }
}