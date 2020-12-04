using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EscolaController : MonoBehaviour
{
    [Header ("Main Container")]
    [SerializeField] private GameObject mainContainer;
    [SerializeField] private ScrollRect mainScrollView;
    [SerializeField] private GameObject itemsContent;
    [SerializeField] private Button newSchoolButton;
    [SerializeField] private GameObject escolaItemPrefab;
    [SerializeField] private GameObject emptyText;

    [Header ("Create | Update Escola Container")]
    [SerializeField] private GameObject createUpdateContainer;
    [SerializeField] private TextMeshProUGUI headerTitle;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button discardButton;
    [SerializeField] private Button acceptButton;

    private MessageModalController modalController;
    private ConfirmModalController confirmModalController;
    private TurmaController turmaController;

    private EscolaService escolaService = new EscolaService();
    private TurmaService turmaService = new TurmaService();
    private AlunoService alunoService = new AlunoService();
    private Escola localEscola = new Escola();

    private void Start() 
    {
        modalController = this.GetComponent<MessageModalController>();
        confirmModalController = this.GetComponent<ConfirmModalController>();
        turmaController = this.GetComponent<TurmaController>();
        BindEventListeners();
        LoadEscolaItems();
    }

    private void BindEventListeners()
    {
        if (createUpdateContainer)
        {
            if (newSchoolButton && discardButton && headerTitle && nameInput)
            {
                newSchoolButton.onClick.AddListener(() => ToggleCreateUpdateContainer(null, true));
                discardButton.onClick.AddListener(() => ToggleCreateUpdateContainer(null, false));
            }
        }
    }

    private void LoadEscolaItems()
    {
        try
        {
            if (itemsContent && escolaItemPrefab && emptyText)
            {
                for (int index = 0; index < itemsContent.transform.childCount; index++)
                {
                    Transform child = itemsContent.transform.GetChild(index);
                    Destroy(child.gameObject);
                }

                List<Escola> escolas = escolaService.FindAll();
                emptyText.SetActive((escolas.Count == 0));
                mainScrollView.transform.parent.gameObject.SetActive((escolas.Count != 0));

                if (escolas.Count == 0)
                {
                    return;
                }

                foreach (var escola in escolas)
                {
                    GameObject item = Instantiate(escolaItemPrefab) as GameObject;
                    item.transform.SetParent(itemsContent.transform, false);
                    
                    // Children
                    Transform nameText = item.transform.Find("NameText");
                    Transform deleteButton = item.transform.Find("DeleteButton");
                    Transform editButton = item.transform.Find("EditButton");
                    Transform classesButton = item.transform.Find("ClassesButton");

                    if (nameText)
                    {
                        var textMesh = nameText.GetComponent<TextMeshProUGUI>();
                        textMesh.SetText(escola.Nome);
                    }

                    if (deleteButton)
                    {
                        var button = deleteButton.GetComponent<Button>();
                        button.onClick.AddListener(() =>
                        {
                            confirmModalController.Show(response => 
                            {
                                if (response)
                                {
                                    bool hasDeleted = false;
                                    var turmas = turmaService.FindByEscola(escola.Id);
                                    if (turmas.Count >= 1)
                                    {
                                        foreach (var turma in turmas)
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
                                        LoadEscolaItems();
                                    }
                                }
                            });
                        });
                    }

                    if (editButton)
                    {
                        var button = editButton.GetComponent<Button>();
                        button.onClick.AddListener(() => ToggleCreateUpdateContainer(escola, true));
                    }

                    if (classesButton)
                    {
                        var button = classesButton.GetComponent<Button>();
                        button.onClick.AddListener(null);
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
        this.localEscola.Id = 0;
        this.localEscola = (escola != null ? escola : new Escola());

        if (show)
        {
            string title = (escola == null ? "Nova Escola" : "Atualizar Escola");
            headerTitle.SetText(title);
        }

        nameInput.text = (escola != null && show ? escola.Nome : null);
        acceptButton.onClick.AddListener(HandleOnClick);
        createUpdateContainer.SetActive(show);
    }

    private void HandleOnClick()
    {
        try
        {
            string name = nameInput.text;
            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
            {
                modalController.Show("Atenção", "É necessário informar o Nome!", Color.blue);
                return;
            }

            this.localEscola.Nome = name;
            bool hasExecuted = (localEscola.Id == 0 ? escolaService.Insert(localEscola) : escolaService.Update(localEscola));
            string title = (hasExecuted ? "Sucesso" : "Falha");
            string message = (hasExecuted ? "Operação concluída!" : "Erro na operação!");
            modalController.Show(title, message, (hasExecuted ? Color.green : Color.red));
            ToggleCreateUpdateContainer(null, false);
            LoadEscolaItems();
        }
        catch (Exception ex)
        {
            modalController.Show("Atenção", ex.Message, Color.red);
        }
    }
}
