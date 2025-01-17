using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using ProjectModels;

public class WorldEvents : MonoBehaviour
{
    private ActionData actionData;
    private UIDocument uiDoc;
    private VisualElement _loadingBlock;
    private VisualElement _mainBlock;
    private Button _actionButton;
    private Button _messageBlock;
    private VisualElement _compassBlock;
    private Button _compassButton;
    private Label _locationText;
    private VisualElement _bottomBar;
    private Button _menuButton;
    private Button _inventoryButton;
    private VisualElement _menuBlock;
    private Button _logoutButton;
    private GameObject _inventoryObject;

    private void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        _loadingBlock = uiDoc.rootVisualElement.Q<VisualElement>("LoadingBlock");
        _mainBlock = uiDoc.rootVisualElement.Q<VisualElement>("MainBlock");
        _messageBlock = uiDoc.rootVisualElement.Q<Button>("MessageBlock");
        _compassBlock = uiDoc.rootVisualElement.Q<VisualElement>("CompassBlock");
        _compassButton = uiDoc.rootVisualElement.Q<Button>("CompassButton");
        _locationText = uiDoc.rootVisualElement.Q<Label>("LocationText");
        _actionButton = uiDoc.rootVisualElement.Q<Button>("ActionButton");
        _bottomBar = uiDoc.rootVisualElement.Q<VisualElement>("BottomBar");
        _menuButton = uiDoc.rootVisualElement.Q<Button>("MenuButton");
        _inventoryButton = uiDoc.rootVisualElement.Q<Button>("InventoryButton");
        _menuBlock = uiDoc.rootVisualElement.Q<VisualElement>("MenuBlock");
        _logoutButton = uiDoc.rootVisualElement.Q<Button>("LogoutButton");
        _inventoryObject = GameObject.Find("InterfaceWorld").transform.Find("Canvas").Find("Inventory").gameObject;

        _bottomBar.style.bottom = -220f;
        _compassBlock.style.top = -500f;
    }

    private void Start()
    {
        _loadingBlock.style.display = DisplayStyle.None;
        _mainBlock.style.display = DisplayStyle.Flex;
        _logoutButton.clicked += Logout;
        _menuButton.clicked += HandleMenu;
        _inventoryButton.clicked += HandleInventory;
        _compassButton.clicked += ResetCompass;
        _actionButton.clicked += HandleAction;
        _messageBlock.clicked += CloseActionMessage;
        DisplayBottomBar(true);
        DisplayCompass(true);
        ShowLocationName(delay: 0.5f);

    }

    Coroutine DisplayBottomBarCoroutine;

    public void DisplayBottomBar(bool show = true)
    {
        if (DisplayBottomBarCoroutine != null) StopCoroutine(DisplayBottomBarCoroutine);
        
        if (show)
        {
            DisplayBottomBarCoroutine = StartCoroutine(ShowBottomBarEnum());
        }
        else
        {
            DisplayBottomBarCoroutine = StartCoroutine(HideBottomBarEnum());
        }
    }

    private IEnumerator ShowBottomBarEnum()
    {
        yield return new WaitForSeconds(.2f);
        _bottomBar.style.bottom = 0f;
    }

    private IEnumerator HideBottomBarEnum()
    {
        DisplayMenu(false);
        yield return new WaitForSeconds(.2f);
        _bottomBar.style.bottom = -220f;
    }

    private bool IsMenuOpen = false;

    public void HandleMenu()
    {
        if (IsInventoryOpen) HandleInventory();
        DisplayMenu(!IsMenuOpen);
    }

    public void DisplayMenu(bool show = true)
    {
        if (show)
        {
            StartCoroutine(ShowMenuBlockEnum());
        }
        else
        {
            StartCoroutine(HideMenuBlockEnum());
        }
    }

    private IEnumerator ShowMenuBlockEnum()
    {
        if (!IsMenuOpen)
        {
            IsMenuOpen = true;
            _menuBlock.style.display = DisplayStyle.Flex;
            _menuBlock.style.bottom = 220f;
            _menuButton.AddToClassList("menu-button-active");
            yield return null;
        }
    }

    private IEnumerator HideMenuBlockEnum()
    {
        if (IsMenuOpen)
        {
            IsMenuOpen = false;
            _menuBlock.style.bottom = -400f;
            yield return new WaitForSeconds(.2f);
            _menuBlock.style.display = DisplayStyle.None;
            _menuButton.RemoveFromClassList("menu-button-active");
        }
    }

    public bool IsInventoryOpen = false;

    public void HandleInventory()
    {
        if (IsMenuOpen) HandleMenu();
        DisplayInventory(!IsInventoryOpen);
    }

    Coroutine DisplayInventoryBlockCoroutine;

    public void DisplayInventory(bool show = true)
    {
        if (DisplayInventoryBlockCoroutine != null) StopCoroutine(DisplayInventoryBlockCoroutine);

        if (show)
        {
            DisplayInventoryBlockCoroutine = StartCoroutine(ShowInventoryBlockEnum());
        }
        else
        {
            DisplayInventoryBlockCoroutine = StartCoroutine(HideInventoryBlockEnum());
        }
    }

    private IEnumerator ShowInventoryBlockEnum()
    {
        if (!IsInventoryOpen)
        {
            IsInventoryOpen = true;
            _inventoryButton.AddToClassList("menu-button-active");
            _inventoryObject.SetActive(true);
            PlayerManager.instance._playerWorld.GetComponent<Animator>().Play("SpotLightOn");
            DisplayCompass(false);
            yield return null;
            DisplayInventoryBlockCoroutine = null;
        }
    }

    private IEnumerator HideInventoryBlockEnum()
    {
        if (IsInventoryOpen)
        {
            IsInventoryOpen = false;
            DisplayCompass(true);
            _inventoryObject.GetComponent<Animator>().Play("InventoryClose");
            PlayerManager.instance._playerWorld.GetComponent<Animator>().Play("SpotLightOff");
            yield return new WaitForSeconds(0.3f);
            InventoryManager.instance.CloseItemInfo();
            InventoryManager.instance.ResetPages();
            _inventoryObject.SetActive(false);
            _inventoryButton.RemoveFromClassList("menu-button-active");
            IsInventoryOpen = false;
            DisplayInventoryBlockCoroutine = null;

        }
    }

    Coroutine DisplayCompassCoroutine;

    public void DisplayCompass(bool show = true)
    {
        if (DisplayCompassCoroutine != null) StopCoroutine(DisplayCompassCoroutine);

        if (show)
        {
            DisplayCompassCoroutine = StartCoroutine(ShowCompassEnum());
        }
        else
        {
            DisplayCompassCoroutine = StartCoroutine(HideCompassEnum());
        }
    }

    private IEnumerator ShowCompassEnum()
    {
        _compassBlock.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(.1f);
        _compassBlock.style.top = 0f;
        DisplayCompassCoroutine = null;

    }

    private IEnumerator HideCompassEnum()
    {
        _compassBlock.style.top = -500f;
        yield return new WaitForSeconds(.3f);
        _compassBlock.style.display = DisplayStyle.None;
        DisplayCompassCoroutine = null;
    }

    public void RotateCompass(float angle)
    {
        _compassButton.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void ResetCompass()
    {
        ShowLocationName();
        UIManager.instance.ResetCompass();
    }

    Coroutine ShowLocationNameCoroutine;

    public void ShowLocationName(float delay = 0f)
    {
        if (ShowLocationNameCoroutine != null) return;

        _locationText.text = SceneManager.GetActiveScene().name;
        ShowLocationNameCoroutine = StartCoroutine(ShowLocationNameEnum(delay));
    }

    IEnumerator ShowLocationNameEnum(float delay = 0f)
    {
        _locationText.style.opacity = 0f;
        yield return new WaitForSeconds(delay);
        _locationText.style.display = DisplayStyle.Flex;
        _locationText.style.opacity = 1f;
        yield return new WaitForSeconds(6f);
        _locationText.style.opacity = 0f;
        yield return new WaitForSeconds(0.5f);
        _locationText.style.display = DisplayStyle.None;
        ShowLocationNameCoroutine = null;
    }

    public void SetActionData(ActionData data)
    {
        actionData = data;
        _actionButton.text = data.buttonText();
        DisplayActionButton(true);
    }

    public void ClearActionData()
    {
        DisplayActionButton(false);
        actionData = null;
    }

    public void DisplayActionButton(bool show = true)
    {
        _actionButton.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void HandleAction()
    {
        if (actionData != null)
        {
            switch (actionData.type())
            {
                case "location":
                    LoadLocation();
                    break;
                case "battle":
                    // TODO
                    break;
            }
        }
    }

    private void LoadLocation()
    {
        if (actionData.location.message != "")
        {
            _messageBlock.text = actionData.location.message;
            DisplayActionMessage(true);
        }
        else
        {
            PlayerPrefs.SetString("forceSpawnCords", actionData.location.spawnPosition.ToString());
            GameManager.instance.LoadLevel(actionData.location.sceneName);
        }
    }

    public void CloseActionMessage()
    {
        DisplayActionMessage(false);
    }

    Coroutine DisplayActionMessageCoroutine;

    public void DisplayActionMessage(bool show = true)
    {
        if (DisplayActionMessageCoroutine != null) StopCoroutine(DisplayActionMessageCoroutine);

        if (show)
        {
            DisplayActionMessageCoroutine = StartCoroutine(ShowActionMessageEnum());
        }
        else
        {
            DisplayActionMessageCoroutine = StartCoroutine(HideActionMessageEnum());
        }
    }

    private IEnumerator ShowActionMessageEnum()
    {
        _actionButton.style.display = DisplayStyle.None;
        _messageBlock.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(5f);
        _messageBlock.style.display = DisplayStyle.None;
        DisplayActionMessageCoroutine = null;
    }

    private IEnumerator HideActionMessageEnum()
    {
        _messageBlock.style.display = DisplayStyle.None;
        yield return null;
        DisplayActionMessageCoroutine = null;
    }

    public void DisplayLoading(bool loading)
    {
        UIManager.instance.isLoading = loading;
        _loadingBlock.style.display = loading ? DisplayStyle.Flex : DisplayStyle.None;
        if (loading) DisplayActionButton(false);
        DisplayCompass(!loading);
        DisplayBottomBar(!loading);
    }

    public void Logout()
    {
        PlayerManager.instance.SyncPlayerData();
        PlayerPrefs.SetString("isLogout", "true");
        GameManager.instance.LoadLevel("Login", false);
    }
}
