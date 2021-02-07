using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurmaController : MonoBehaviour
{
    // || Inspector References

    [Header("Main Container")]
    [SerializeField] private GameObject turmaContainer;
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
    [SerializeField] private AlunoController alunoController;

    // || Cached References

    private TurmaService turmaService = new TurmaService();
    private AlunoService alunoService = new AlunoService();
    private Escola localEscola = new Escola();
    private Turma localTurma = new Turma();

    private void Awake()
    {
        this.localTurma.Id = 0;
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
                closeButton.onClick.AddListener(() => turmaContainer.SetActive(false));
                newButton.onClick.AddListener(() => ToggleCreateUpdateContainer(null, true));
                discardButton.onClick.AddListener(() => ToggleCreateUpdateContainer(null, false));
                acceptButton.onClick.AddListener(HandleOnClick);
            }
        }
    }

    public void Show(Escola escola)
    {
        if (turmaContainer)
        {
            this.localEscola = escola;
            turmaContainer.SetActive(true);
            LoadTurmasByEscola(escola);
        }
    }

    private void LoadTurmasByEscola(Escola escola)
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

                List<Turma> turmas = turmaService.FindByEscola(escola.Id);
                emptyText.SetActive((turmas.Count == 0));
                scrollView.transform.parent.gameObject.SetActive((turmas.Count != 0));

                if (turmas.Count == 0)
                {
                    return;
                }

                foreach (Turma turma in turmas)
                {
                    GameObject item = Instantiate(itemPrefab) as GameObject;
                    item.transform.SetParent(content.transform, false);

                    // Children
                    Transform nameText = item.transform.Find("NameText");
                    Transform deleteButton = item.transform.Find("DeleteButton");
                    Transform editButton = item.transform.Find("EditButton");
                    Transform studentsButton = item.transform.Find("StudentsButton");

                    if (nameText)
                    {
                        TextMeshProUGUI textMesh = nameText.GetComponent<TextMeshProUGUI>();
                        textMesh.SetText(turma.Nome);
                    }

                    if (deleteButton)
                    {
                        Button button = deleteButton.GetComponent<Button>();
                        button.onClick.AddListener(() => HandleOnDelete(turma, escola));
                    }

                    if (editButton)
                    {
                        Button button = editButton.GetComponent<Button>();
                        button.onClick.AddListener(() => ToggleCreateUpdateContainer(turma, true));
                    }

                    if (studentsButton)
                    {
                        Button button = studentsButton.GetComponent<Button>();
                        button.onClick.AddListener(() => alunoController.Show(turma));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            modalController.Show("Atenção", ex.Message, Color.red);
        }
    }

    private void ToggleCreateUpdateContainer(Turma turma, bool show)
    {
        this.localTurma.Id = (turma != null && show ? turma.Id : 0);
        this.localTurma.Nome = (turma != null && show ? turma.Nome : null);
        string title = (turma == null ? "Nova Turma" : "Atualizar Turma");
        nameInput.text = this.localTurma.Nome;
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

            this.localTurma.Nome = name;
            this.localTurma.Escola = this.localEscola;
            bool hasExecuted = (localTurma.Id == 0 ? turmaService.Insert(localTurma) : turmaService.Update(localTurma));
            string title = (hasExecuted ? "Sucesso" : "Falha");
            string message = (hasExecuted ? "Operação concluída!" : "Erro na operação!");
            modalController.Show(title, message, (hasExecuted ? Color.green : Color.red));
            ToggleCreateUpdateContainer(null, false);
            LoadTurmasByEscola(this.localEscola);
        }
        catch (Exception ex)
        {
            modalController.Show("Atenção", ex.Message, Color.red);
        }
    }

    private void HandleOnDelete(Turma turma, Escola escola)
    {
        confirmModalController.Show(response =>
        {
            if (response)
            {
                try
                {
                    bool hasDeleted = false;
                    List<Aluno> alunos = alunoService.FindByTurma(turma.Id);
                    if (alunos.Count >= 1)
                    {
                        hasDeleted = alunoService.DeleteAllByTurma(turma.Id);
                    }

                    hasDeleted = turmaService.DeleteById(turma.Id);
                    string title = (hasDeleted ? "Sucesso" : "Falha");
                    string message = (hasDeleted ? "Operação concluída!" : "Erro na operação!");
                    modalController.Show(title, message, (hasDeleted ? Color.green : Color.red));

                    if (hasDeleted)
                    {
                        LoadTurmasByEscola(escola);
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