using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EscolaController : MonoBehaviour
{
    // || Inspector References

    [Header("Main Container")]
    [SerializeField] private GameObject mainContainer;
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private GameObject content;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button newButton;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject emptyText;

    [Header("Create | Update Escola Container")]
    [SerializeField] private GameObject createUpdateContainer;
    [SerializeField] private TextMeshProUGUI headerTitle;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button discardButton;
    [SerializeField] private Button acceptButton;

    [Header("Other Controllers")]
    [SerializeField] private MessageModalController modalController;
    [SerializeField] private ConfirmModalController confirmModalController;
    [SerializeField] private TurmaController turmaController;

    // || Cached References

    private EscolaService escolaService = new EscolaService();
    private TurmaService turmaService = new TurmaService();
    private AlunoService alunoService = new AlunoService();
    private Escola localEscola = new Escola();

    private void Awake()
    {
        localEscola.Id = 0;
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
                closeButton.onClick.AddListener(() => Application.Quit());
                newButton.onClick.AddListener(() => ToggleCreateUpdateContainer(null, true));
                discardButton.onClick.AddListener(() => ToggleCreateUpdateContainer(null, false));
                acceptButton.onClick.AddListener(() => HandleOnClick());
            }
        }
    }

    public void Show()
    {
        if (mainContainer)
        {
            mainContainer.SetActive(true);
            LoadEscolas();
        }
    }

    private void LoadEscolas()
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

                List<Escola> escolas = escolaService.FindAll();
                emptyText.SetActive((escolas.Count == 0));
                scrollView.transform.parent.gameObject.SetActive((escolas.Count != 0));

                if (escolas.Count == 0)
                {
                    return;
                }

                foreach (Escola escola in escolas)
                {
                    GameObject item = Instantiate(itemPrefab) as GameObject;
                    item.transform.SetParent(content.transform, false);

                    // Children
                    Transform nameText = item.transform.Find("NameText");
                    Transform deleteButton = item.transform.Find("DeleteButton");
                    Transform editButton = item.transform.Find("EditButton");
                    Transform classesButton = item.transform.Find("ClassesButton");

                    if (nameText)
                    {
                        TextMeshProUGUI textMesh = nameText.GetComponent<TextMeshProUGUI>();
                        textMesh.SetText(escola.Nome);
                    }

                    if (deleteButton)
                    {
                        Button button = deleteButton.GetComponent<Button>();
                        button.onClick.AddListener(() => HandleOnDelete(escola));
                    }

                    if (editButton)
                    {
                        Button button = editButton.GetComponent<Button>();
                        button.onClick.AddListener(() => ToggleCreateUpdateContainer(escola, true));
                    }

                    if (classesButton)
                    {
                        Button button = classesButton.GetComponent<Button>();
                        button.onClick.AddListener(() => turmaController.Show(escola));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            modalController.Show("Atenção", ex.Message, Color.red);
        }
    }

    private void ToggleCreateUpdateContainer(Escola escola, bool show)
    {
        this.localEscola.Id = (escola != null && show ? escola.Id : 0);
        this.localEscola.Nome = (escola != null && show ? escola.Nome : null);
        string title = (escola == null ? "Nova Escola" : "Atualizar Escola");
        nameInput.text = this.localEscola.Nome;
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

            this.localEscola.Nome = name;
            bool hasExecuted = (localEscola.Id == 0 ? escolaService.Insert(localEscola) : escolaService.Update(localEscola));
            string title = (hasExecuted ? "Sucesso" : "Falha");
            string message = (hasExecuted ? "Operação concluída!" : "Erro na operação!");
            modalController.Show(title, message, (hasExecuted ? Color.green : Color.red));
            ToggleCreateUpdateContainer(null, false);
            LoadEscolas();
        }
        catch (Exception ex)
        {
            modalController.Show("Atenção", ex.Message, Color.red);
        }
    }

    private void HandleOnDelete(Escola escola)
    {
        confirmModalController.Show(response =>
        {
            try
            {
                if (response)
                {
                    bool hasDeleted = false;
                    List<Turma> turmas = turmaService.FindByEscola(escola.Id);
                    if (turmas.Count >= 1)
                    {
                        foreach (Turma turma in turmas)
                        {
                            hasDeleted = alunoService.DeleteAllByTurma(turma.Id);
                        }

                        hasDeleted = turmaService.DeleteAllByEscola(escola.Id);
                    }

                    hasDeleted = escolaService.DeleteById(escola.Id);
                    string title = (hasDeleted ? "Sucesso" : "Falha");
                    string message = (hasDeleted ? "Operação concluída!" : "Erro na operação!");
                    modalController.Show(title, message, (hasDeleted ? Color.green : Color.red));

                    if (hasDeleted)
                    {
                        LoadEscolas();
                    }
                }
            }
            catch (Exception ex)
            {
                modalController.Show("Atenção", ex.Message, Color.red);
            }
        });
    }
}